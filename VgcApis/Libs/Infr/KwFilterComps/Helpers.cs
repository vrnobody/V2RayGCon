using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    internal static class Helpers
    {
        public static readonly long MiB = 1024 * 1024;

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

        // credit: https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        // usage: var parsedText = ParseText(streamR, ' ', '"');
        internal static string[] Tokenize(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new string[0];
            }

            var ZERO = '\0';

            char delimiter = ' ';
            char textQualifier = '"';
            var seps = new HashSet<char>($"()|&{delimiter}".Select(c => c));

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
                        r.Add(token.ToString());
                        token = token.Remove(0, token.Length);
                    }
                    if (currentChar != delimiter)
                    {
                        r.Add(currentChar.ToString());
                    }
                    continue;
                }
                token = token.Append(currentChar);
            }
            if (token.Length > 0)
            {
                r.Add(token.ToString());
                token = token.Remove(0, token.Length);
            }
            return r.ToArray();
        }
    }
}
