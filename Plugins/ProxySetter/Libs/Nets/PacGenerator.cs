﻿using System;
using System.Collections.Generic;
using System.Text;
using ProxySetter.Resources.Langs;

namespace ProxySetter.Libs.Nets
{
    public class PacGenerator
    {
        public PacGenerator() { }

        public void Run() { }

        #region public method
        /// <summary>
        /// Item1: content, Item2: mime
        /// </summary>
        /// <param name="isUseCustomPac"></param>
        /// <param name="urlParam"></param>
        /// <param name="customLists"></param>
        /// <param name="customPacFileContent"></param>
        /// <returns></returns>
        public Tuple<string, string> GenPacFileResponse(
            bool isUseCustomPac,
            Model.Data.PacUrlParams urlParam,
            string[] customLists,
            string customPacFileContent
        )
        {
            // Don't addpend charset, IE will not work.
            // var mime = "application/x-ns-proxy-autoconfig; charset=UTF-8";
            var mime = "application/x-ns-proxy-autoconfig";

            StringBuilder content;
            if (isUseCustomPac)
            {
                content = GenCustomPacFile(urlParam, customLists, customPacFileContent);
            }
            else
            {
                content = GenDefaultPacFile(urlParam, customLists);
            }

            return new Tuple<string, string>(content.ToString(), mime);
        }

        public static string[] GenDefaultPacCacahe(
            List<string> domainList,
            List<long[]> cidrList,
            string customUrlList
        )
        {
            // generate if not in cache
            MergeCustomPacSetting(ref domainList, ref cidrList, customUrlList);
            var cidrSimList = Misc.Utils.CompactCidrList(ref cidrList);
            StringBuilder domainSB = ConvertDomainListIntoJsDict(domainList);
            StringBuilder cidrSB = ConvertCidrListIntoJsRangeArray(cidrSimList);
            return new string[] { domainSB.ToString(), cidrSB.ToString(), };
        }

        public static void ClearCache()
        {
            defaultPacCache = new Dictionary<bool, string[]> { { true, null }, { false, null }, };
        }
        #endregion

        #region properties
        static Dictionary<bool, string[]> defaultPacCache = new Dictionary<bool, string[]>
        {
            { true, null },
            { false, null },
        };
        #endregion

        #region private method
        static StringBuilder ConvertDomainListIntoJsDict(List<string> domainList)
        {
            var domainSB = new StringBuilder();
            foreach (var url in domainList)
            {
                domainSB.Append("'").Append(url).Append("':1,");
            }
            if (domainList.Count > 0)
            {
                domainSB.Length--;
            }

            return domainSB;
        }

        static StringBuilder ConvertCidrListIntoJsRangeArray(List<long[]> cidrSimList)
        {
            var cidrSB = new StringBuilder();
            foreach (var cidr in cidrSimList)
            {
                cidrSB.Append("[").Append(cidr[0]).Append(",").Append(cidr[1]).Append("],");
            }
            if (cidrSimList.Count > 0)
            {
                cidrSB.Length--;
            }

            return cidrSB;
        }

        StringBuilder GenCustomPacFile(
            Model.Data.PacUrlParams urlParam,
            string[] customLists,
            string customPacFileContent
        )
        {
            var header = new Model.Data.CustomPacHeader(urlParam, customLists[0], customLists[1]);

            return new StringBuilder("var customSettings = ")
                .Append(VgcApis.Misc.Utils.SerializeObject(header))
                .Append(";")
                .Append(customPacFileContent ?? string.Empty);
        }

        static StringBuilder GenDefaultPacFile(
            Model.Data.PacUrlParams urlParam,
            string[] customLists
        )
        {
            // dns leaks
            // var proxy = urlParam.isSocks ? "SOCKS5 {0}:{1}; SOCKS {0}:{1}; DIRECT" : "PROXY {0}:{1}; DIRECT";

            var proxy = urlParam.isSocks ? "SOCKS5 {0}:{1};" : "PROXY {0}:{1};";
            var isWhiteList = urlParam.isWhiteList;
            var mode = isWhiteList ? "white" : "black";

            // update cache
            if (defaultPacCache[isWhiteList] == null)
            {
                defaultPacCache[isWhiteList] = GenDefaultPacCacahe(
                    Misc.Utils.GetPacDomainList(isWhiteList),
                    Misc.Utils.GetPacCidrList(isWhiteList),
                    customLists[isWhiteList ? 0 : 1]
                );
            }

            var domainAndCidrs = defaultPacCache[isWhiteList];
            var content = new StringBuilder(StrConst.PacJsTpl)
                .Replace("__PROXY__", string.Format(proxy, urlParam.ip, urlParam.port))
                .Replace("__MODE__", mode)
                .Replace("__DOMAINS__", domainAndCidrs[0])
                .Replace("__CIDRS__", domainAndCidrs[1]);
            return content;
        }

        static void MergeCustomPacSetting(
            ref List<string> domainList,
            ref List<long[]> cidrList,
            string customList
        )
        {
            var list = customList.Split(
                new char[] { '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries
            );

            var isRemove = false;
            foreach (var line in list)
            {
                var item = line.Trim();

                // ignore single line comment
                if (item.StartsWith("//"))
                {
                    continue;
                }

                isRemove = false;
                if (item.StartsWith("-"))
                {
                    isRemove = true;
                    item = item.Substring(1);
                }
                ParseCustomPacSetting(ref domainList, ref cidrList, item, isRemove);
            }
        }

        static void RemoveRangeFromCidrList(ref List<long[]> cidrList, long[] range, bool isRemove)
        {
            if (!isRemove)
            {
                cidrList.Add(range);
                return;
            }

            // 跑两次少写点代码
            cidrList.RemoveAll(e => Misc.Utils.Overlaps(e, range) == Model.Data.Enum.Overlaps.All);

            for (int i = 0; i < cidrList.Count; i++)
            {
                var element = cidrList[i];
                switch (Misc.Utils.Overlaps(element, range))
                {
                    case Model.Data.Enum.Overlaps.Left:
                        element[0] = range[1] + 1;
                        break;
                    case Model.Data.Enum.Overlaps.Right:
                        element[1] = range[0] - 1;
                        break;
                    case Model.Data.Enum.Overlaps.Middle:
                        cidrList.Add(new long[] { range[1] + 1, element[1] });
                        element[1] = range[0] - 1;
                        break;
                    default:
                        break;
                }
            }
        }

        static void ParseCustomPacSetting(
            ref List<string> domainList,
            ref List<long[]> cidrList,
            string item,
            bool isRemove
        )
        {
            if (Misc.Utils.IsIP(item))
            {
                var v = Misc.Utils.IP2Long(item);
                RemoveRangeFromCidrList(ref cidrList, new long[] { v, v }, isRemove);
                return;
            }

            if (Misc.Utils.IsCidr(item))
            {
                RemoveRangeFromCidrList(ref cidrList, Misc.Utils.Cidr2RangeArray(item), isRemove);
                return;
            }

            if (isRemove)
            {
                domainList.RemoveAll(e => e == item);
                return;
            }

            if (!domainList.Contains(item))
            {
                domainList.Add(item);
            }
        }
        #endregion
    }
}
