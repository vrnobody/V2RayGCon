using ProxySetter.Resources.Langs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProxySetter.Lib
{
    public static class Utils
    {
        #region pac
        public static Model.Data.Enum.Overlaps Overlaps(long[] a, long[] b)
        {
            if (a.Length != b.Length
                || a.Length != 2
                || a[0] > a[1]
                || b[0] > b[1]
                || b[0] > a[1])
            {
                // skip if error happened
                return Model.Data.Enum.Overlaps.None;
            }

            if (b[0] > a[0])
            {
                if (b[1] >= a[1])
                {
                    return Model.Data.Enum.Overlaps.Right;
                }
                return Model.Data.Enum.Overlaps.Middle;
            }

            // b[0] <= a[0]

            if (b[1] >= a[1])
            {
                return Model.Data.Enum.Overlaps.All;
            }

            if (b[1] >= a[0])
            {
                return Model.Data.Enum.Overlaps.Left;
            }

            return Model.Data.Enum.Overlaps.None;
        }

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseUrlQueryIntoParams(string url)
        {
            // https://stackoverflow.com/questions/2884551/get-individual-query-parameters-from-uri

            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            // e.g. url = http://localhost:3000/pac/?&port=5678&ip=1.2.3.4&proto=socks&type=whitelist&key=rnd

            Dictionary<string, string> arguments = null;
            try
            {
                var query = new Uri(url).Query;
                arguments = query
                 .Substring(1) // Remove '?'
                 .Split('&')
                 .Select(q => q.Split('='))
                 .ToDictionary(q => q.FirstOrDefault(), q => q.Skip(1).FirstOrDefault());
            }
            catch { }

            return arguments;
        }

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Model.Data.QueryParams GetQureryParamsFrom(string url)
        {
            var arguments = ParseUrlQueryIntoParams(url);

            if (arguments == null)
            {
                return new Model.Data.QueryParams();
            }

            // e.g. http://localhost:3000/pac/?&port=5678&ip=1.2.3.4&proto=socks&type=whitelist&key=rnd
            arguments.TryGetValue("mime", out string mime);
            arguments.TryGetValue("debug", out string debug);
            arguments.TryGetValue("ip", out string ip);
            arguments.TryGetValue("port", out string portStr);
            arguments.TryGetValue("type", out string type);
            arguments.TryGetValue("proto", out string proto);

            return new Model.Data.QueryParams
            {
                mime = mime,
                debug = debug,
                ip = ip,
                port = portStr,
                type = type,
                proto = proto,
            };
        }

        public static bool IsCidr(string address)
        {
            var parts = address.Split('/');

            if (parts.Length != 2)
            {
                return false;
            }

            if (!IsIP(parts[0]))
            {
                return false;
            }
            var mask = VgcApis.Libs.Utils.Str2Int(parts[1]);
            return mask >= 0 && mask <= 32;
        }

        public static List<string> GetPacDomainList(bool isWhiteList)
        {
            return (isWhiteList ? StrConst.white : StrConst.black)
                .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        public static string Long2Ip(long number)
        {
            if (number < 0 || number >= 1L << 32)
            {
                return "0.0.0.0";
            }

            var ips = new List<long>();
            for (int i = 0; i < 4; i++)
            {
                ips.Add(number % 256);
                number /= 256;
            }
            ips.Reverse();
            return string.Join(".", ips);
        }

        public static long[] Cidr2RangeArray(string cidrString)
        {
            var cidr = cidrString.Split('/');
            var begin = IP2Long(cidr[0]);
            var end = (1L << (32 - VgcApis.Libs.Utils.Str2Int(cidr[1]))) + begin;
            return new long[] { begin, Math.Max(Math.Min(end - 1, (1L << 32) - 1), begin) };
        }

        public static List<long[]> GetPacCidrList(bool isWhiteList)
        {
            var result = new List<long[]>();
            foreach (var item in
                (isWhiteList ? StrConst.white_cidr : StrConst.black_cidr)
                .Split(new char[] { '\n', '\r' },
                    StringSplitOptions.RemoveEmptyEntries))
            {
                result.Add(Cidr2RangeArray(item));
            }
            return result;
        }

        public static List<long[]> CompactCidrList(ref List<long[]> cidrList)
        {
            if (cidrList == null || cidrList.Count <= 0)
            {
                return new List<long[]>();
                // throw new System.ArgumentException("Range list is empty!");
            }

            cidrList.Sort((a, b) => a[0].CompareTo(b[0]));

            var result = new List<long[]>();
            var curRange = cidrList[0];
            foreach (var range in cidrList)
            {
                if (range.Length != 2)
                {
                    throw new System.ArgumentException("Range must have 2 number.");
                }

                if (range[0] > range[1])
                {
                    throw new ArgumentException("range[0] > range[1]. ");
                }

                if (range[0] <= curRange[1] + 1)
                {
                    curRange[1] = Math.Max(range[1], curRange[1]);
                }
                else
                {
                    result.Add(curRange);
                    curRange = range;
                }
            }
            result.Add(curRange);
            return result;
        }

        public static long IP2Long(string ip)
        {
            var c = ip.Split('.')
                .Select(el => (long)VgcApis.Libs.Utils.Str2Int(el))
                .ToList();

            if (c.Count < 4)
            {
                throw new System.ArgumentException("Not a valid ip.");
            }

            return 256 * (256 * (256 * +c[0] + +c[1]) + +c[2]) + +c[3];
        }

        public static bool IsIP(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return false;
            }

            return Regex.IsMatch(address, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        }

        #endregion
    }
}
