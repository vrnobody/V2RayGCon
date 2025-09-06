using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Misc
{
    internal static class Utils
    {
        public static string TrimComments(string s)
        {
            var lines = ParseMultiLineString(s);
            return string.Join(" ", lines);
        }

        public static Dictionary<string, string> ToEnvVarDict(string envs)
        {
            var lines = ParseMultiLineString(envs);
            var r = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var parts = line.Split(new char[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    r.Add(parts[0], parts[1]);
                }
            }
            return r;
        }

        static List<string> ParseMultiLineString(string s)
        {
            return (s ?? "")
                .Replace("\r", "")
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !line.StartsWith("//"))
                .ToList();
        }
    }
}
