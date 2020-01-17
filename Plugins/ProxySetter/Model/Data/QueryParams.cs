using System.Collections.Generic;

namespace ProxySetter.Model.Data
{
    public class QueryParams
    {

        Dictionary<Enum.PacListModes, string> pacTypeName = new Dictionary<Enum.PacListModes, string>
        {
            { Enum.PacListModes.WhiteList, "whitelist" },
            { Enum.PacListModes.BlackList, "blacklist" },
        };

        Dictionary<Enum.PacProtocols, string> pacProtocolName = new Dictionary<Enum.PacProtocols, string> {
            { Enum.PacProtocols.HTTP, "http" },
            { Enum.PacProtocols.SOCKS, "socks" },
        };

        public string mime { get; set; } = null;
        public string debug { get; set; } = null;
        public string ip { get; set; } = null;
        public string port { get; set; } = null;
        public string type { get; set; } = null;
        public string proto { get; set; } = null;

        public QueryParams() { }

        public QueryParams(BasicSettings basicSetting)
        {
            port = basicSetting.proxyPort.ToString();
            type =
                basicSetting.pacMode == (int)Enum.PacListModes.WhiteList ?
                pacTypeName[Enum.PacListModes.WhiteList] :
                pacTypeName[Enum.PacListModes.BlackList];

            proto =
                basicSetting.pacProtocol == (int)Enum.PacProtocols.SOCKS ?
                pacProtocolName[Enum.PacProtocols.SOCKS] :
                pacProtocolName[Enum.PacProtocols.HTTP];
        }

        public void ReplaceNullValueWith(QueryParams defaultValues)
        {
            var v = defaultValues;

            mime = mime ?? v.mime;
            debug = debug ?? v.debug;
            ip = ip ?? v.ip;
            port = port ?? v.port;
            type = type ?? v.type;
            proto = proto ?? v.proto;
        }

        public PacUrlParams ToPacUrlParams()
        {
            return new PacUrlParams
            {
                ip = (Lib.Utils.IsIP(ip) ? ip : VgcApis.Models.Consts.Webs.LoopBackIP),
                port = VgcApis.Libs.Utils.Clamp(
                    VgcApis.Libs.Utils.Str2Int(port), 0, 65536),
                isSocks = proto?.ToLower() == pacProtocolName[Enum.PacProtocols.SOCKS],
                isWhiteList = type?.ToLower() != pacTypeName[Enum.PacListModes.BlackList],
                isDebug = debug?.ToLower() == "true",
                mime = mime ?? "",
            };
        }
    }
}
