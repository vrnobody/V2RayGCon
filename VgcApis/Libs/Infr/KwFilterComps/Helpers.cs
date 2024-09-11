using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VgcApis.Libs.Infr.KwFilterComps.BoolExprComps;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal static class Helpers
    {
        public static readonly string NOT = "not";
        public static readonly long MiB = 1024 * 1024;

        #region functions

        public static bool IsAdvSearchKeyword(string kw)
        {
            if (string.IsNullOrEmpty(kw))
            {
                return false;
            }
            try
            {
                if (Regex.IsMatch(kw, @"^[\( ]*#[a-zA-Z]+"))
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        public static Dictionary<string, TEnum> CreateEnumLookupTable<TEnum>()
            where TEnum : struct
        {
            var r = new Dictionary<string, TEnum>();
            foreach (TEnum cname in Enum.GetValues(typeof(TEnum)))
            {
                var key = cname.ToString().ToLower();
                r[key] = cname;
            }
            return r;
        }

        #endregion


        #region expr parser
        // credits:
        // https://stackoverflow.com/questions/17568067/how-to-parse-a-boolean-expression-and-load-it-into-a-class
        // https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes

        internal static BoolExpr MakeExpr(ref List<List<ExprToken>>.Enumerator pe)
        {
            var cur = pe.Current;
            if (cur == null)
            {
                return null;
            }

            var first = cur.First();
            if (first.type == ExprTokenTypes.LITERAL)
            {
                BoolExpr leaf = new LeafExpr(cur);
                pe.MoveNext();
                return leaf;
            }

            var op = first.value;
            if (op == "&")
            {
                pe.MoveNext();
                BoolExpr left = MakeExpr(ref pe);
                BoolExpr right = MakeExpr(ref pe);
                return new AndExpr(left, right);
            }

            if (op == "|")
            {
                pe.MoveNext();
                BoolExpr left = MakeExpr(ref pe);
                BoolExpr right = MakeExpr(ref pe);
                return new OrExpr(left, right);
            }

            if (op == "!")
            {
                pe.MoveNext();
                BoolExpr left = MakeExpr(ref pe);
                BoolExpr right = MakeExpr(ref pe);
                return new NotExpr(left, right);
            }
            return null;
        }

        internal static List<List<ExprToken>> TransformToPolishNotation(List<ExprToken> tokens)
        {
            Queue<List<ExprToken>> r = new Queue<List<ExprToken>>();
            Stack<List<ExprToken>> stack = new Stack<List<ExprToken>>();

            int index = 0;
            var len = tokens.Count;
            while (len > index)
            {
                ExprToken tk = tokens[index];
                switch (tk.type)
                {
                    case ExprTokenTypes.LITERAL:
                        var kws = new List<ExprToken>() { };
                        do
                        {
                            tk = tokens[index];
                            kws.Add(tk);
                            index++;
                        } while (len > index && tokens[index].type == ExprTokenTypes.LITERAL);
                        kws.Reverse();
                        r.Enqueue(kws);
                        index--;
                        break;
                    case ExprTokenTypes.BINARY_OP:
                    case ExprTokenTypes.CLOSE_PAREN: // in reverse order
                        stack.Push(new List<ExprToken>() { tk });
                        break;
                    case ExprTokenTypes.OPEN_PAREN:
                        while (stack.Peek().First().type != ExprTokenTypes.CLOSE_PAREN)
                        {
                            r.Enqueue(stack.Pop());
                        }
                        stack.Pop(); // pop OPEN_PAREN
                        break;
                    default:
                        break;
                }

                ++index;
            }
            while (stack.Count > 0)
            {
                r.Enqueue(stack.Pop());
            }

            return r.Reverse().ToList();
        }

        internal static Dictionary<char, KeyValuePair<ExprTokenTypes, string>> exprOperators =
            new Dictionary<char, KeyValuePair<ExprTokenTypes, string>>()
            {
                { '(', new KeyValuePair<ExprTokenTypes, string>(ExprTokenTypes.OPEN_PAREN, "(") },
                { ')', new KeyValuePair<ExprTokenTypes, string>(ExprTokenTypes.CLOSE_PAREN, ")") },
                { '&', new KeyValuePair<ExprTokenTypes, string>(ExprTokenTypes.BINARY_OP, "&") },
                { '|', new KeyValuePair<ExprTokenTypes, string>(ExprTokenTypes.BINARY_OP, "|") },
                { '!', new KeyValuePair<ExprTokenTypes, string>(ExprTokenTypes.BINARY_OP, "!") },
            };

        static void PatchParentheses(ref List<ExprToken> tokens)
        {
            var open = 0;
            var close = 0;
            var first = false;
            foreach (var token in tokens)
            {
                switch (token.type)
                {
                    case ExprTokenTypes.OPEN_PAREN:
                        first = true;
                        close++;
                        break;
                    case ExprTokenTypes.CLOSE_PAREN:
                        if (first)
                        {
                            close--;
                        }
                        else
                        {
                            open++;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (close < 0)
            {
                open -= close;
            }

            for (int i = 0; i < open; i++)
            {
                tokens.Insert(0, new ExprToken(ExprTokenTypes.OPEN_PAREN, "("));
            }

            for (int i = 0; i < close; i++)
            {
                tokens.Add(new ExprToken(ExprTokenTypes.CLOSE_PAREN, ")"));
            }
        }

        internal static List<ExprToken> ParseExprToken(string line)
        {
            var exprEnd = @"0b56d0b6-91db-4dd0-8678-f040fe1ffd97"; // any not empty string
            if (string.IsNullOrWhiteSpace(line))
            {
                return new List<ExprToken>() { new ExprToken(ExprTokenTypes.EXPR_END, exprEnd) };
            }

            var ZERO = '\0';

            char delimiter = ' ';
            char textQualifier = '"';
            var seps = new HashSet<char>(
                new List<char>() { delimiter }
                    .Concat(exprOperators.Keys)
                    .Select(c => c)
            );

            var r = new List<ExprToken>();

            char prevChar = ZERO;
            char nextChar = ZERO;
            char currentChar = ZERO;
            bool inString = false;

            StringBuilder token = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                prevChar = i > 0 ? line[i - 1] : ZERO;
                currentChar = line[i];
                nextChar = (i + 1 < line.Length) ? line[i + 1] : ZERO;

                if (
                    currentChar == textQualifier
                    && (prevChar == ZERO || seps.Contains(prevChar))
                    && !inString
                )
                {
                    inString = true;
                    continue;
                }

                if (
                    currentChar == textQualifier
                    && (nextChar == ZERO || seps.Contains(nextChar))
                    && inString
                )
                {
                    inString = false;
                    continue;
                }

                if (!inString && seps.Contains(currentChar))
                {
                    if (token.Length > 0)
                    {
                        r.Add(new ExprToken(ExprTokenTypes.LITERAL, token.ToString()));
                        token = token.Remove(0, token.Length);
                    }
                    if (currentChar != delimiter && exprOperators.ContainsKey(currentChar))
                    {
                        var ty = exprOperators[currentChar].Key;
                        var value = exprOperators[currentChar].Value;
                        r.Add(new ExprToken(ty, value));
                    }
                    continue;
                }
                token = token.Append(currentChar);
            }
            if (token.Length > 0)
            {
                r.Add(new ExprToken(ExprTokenTypes.LITERAL, token.ToString()));
            }
            PatchParentheses(ref r);
            r.Reverse(); // (#1 & #2) & #3
            r.Add(new ExprToken(ExprTokenTypes.EXPR_END, exprEnd));
            return r.ToList();
        }
        #endregion


        #region literal parser
        // credit: https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        // usage: var parsedText = ParseText(streamR, ' ', '"');
        internal static string[] ParseLiteral(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new string[0];
            }

            var ZERO = '\0';

            char delimiter = ' ';
            char textQualifier = '"';

            var r = new List<string>();

            char prevChar = ZERO;
            char nextChar = ZERO;
            char currentChar = ZERO;
            bool inString = false;

            StringBuilder token = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                prevChar = i > 0 ? line[i - 1] : ZERO;
                currentChar = line[i];
                nextChar = (i + 1 < line.Length) ? line[i + 1] : ZERO;

                if (
                    currentChar == textQualifier
                    && (prevChar == ZERO || delimiter == prevChar)
                    && !inString
                )
                {
                    inString = true;
                    continue;
                }

                if (
                    currentChar == textQualifier
                    && (nextChar == ZERO || delimiter == nextChar)
                    && inString
                )
                {
                    inString = false;
                    continue;
                }

                if (!inString && delimiter == currentChar)
                {
                    if (token.Length > 0)
                    {
                        r.Add(token.ToString());
                        token = token.Remove(0, token.Length);
                    }
                    continue;
                }
                token = token.Append(currentChar);
            }
            if (token.Length > 0)
            {
                r.Add(token.ToString());
            }
            return r.ToArray();
        }
        #endregion
    }
}
