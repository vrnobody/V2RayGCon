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

        internal static string[] Tokenize(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new string[] { };
            }
            var r = ParseTextCore(str, ' ', '"').Where(t => !string.IsNullOrEmpty(t)).ToArray();
            return r;
        }

        // credit: https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        // usage: var parsedText = ParseText(streamR, ' ', '"');
        internal static IEnumerable<string> ParseTextCore(
            string line,
            char delimiter,
            char textQualifier
        )
        {
            if (line == null)
                yield break;
            else
            {
                char prevChar = '\0';
                char nextChar = '\0';
                char currentChar = '\0';

                bool inString = false;

                StringBuilder token = new StringBuilder();

                for (int i = 0; i < line.Length; i++)
                {
                    currentChar = line[i];

                    if (i > 0)
                        prevChar = line[i - 1];
                    else
                        prevChar = '\0';

                    if (i + 1 < line.Length)
                        nextChar = line[i + 1];
                    else
                        nextChar = '\0';

                    if (
                        currentChar == textQualifier
                        && (prevChar == '\0' || prevChar == delimiter)
                        && !inString
                    )
                    {
                        inString = true;
                        continue;
                    }

                    if (
                        currentChar == textQualifier
                        && (nextChar == '\0' || nextChar == delimiter)
                        && inString
                    )
                    {
                        inString = false;
                        continue;
                    }

                    if (currentChar == delimiter && !inString)
                    {
                        yield return token.ToString();
                        token = token.Remove(0, token.Length);
                        continue;
                    }

                    token = token.Append(currentChar);
                }

                yield return token.ToString();
            }
        }
    }
}
