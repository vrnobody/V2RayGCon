using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    internal class ServerConfigs
    {
        public string proto;
        public string addr;
        public string auth1;
        public string auth2;
        public string method;
        public bool useOta;
        public bool useTls;
        public string streamType;
        public string streamParam;

        public ServerConfigs() { }

        public ServerConfigs(string config)
        {
            try
            {
                var j = JObject.Parse(config);
                ParseConfig(j);
            }
            catch { }
        }
        public ServerConfigs(JObject config)
        {
            ParseConfig(config);
        }

        public string ToConfig()
        {
            return "";
        }

        #region private methods

        int GetIndexByNetwork(string network)
        {
            if (string.IsNullOrEmpty(network))
            {
                return -1;
            }

            foreach (var item in Models.Datas.Table.streamSettings)
            {
                if (item.Value.network == network)
                {
                    return item.Key;
                }
            }

            return -1;
        }

        void ParseConfig(JObject config)
        {
            if (config == null)
            {
                return;
            }

            var root = Misc.Utils.GetConfigRoot(false, true);
            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            ParseStreamSettings(root, GetStr);

            proto = Misc.Utils.GetValue<string>(config, root, "protocol");
            switch (proto)
            {
                case "vmess":
                case "vless":
                    auth1 = GetStr(root + ".settings.vnext.0.users.0", "id");
                    addr = Misc.Utils.GetAddr(config, root + ".settings.vnext.0", "address", "port");
                    break;
                case "shadowsocks":
                    var ssPrefix = root + ".settings.servers.0";
                    useOta = Misc.Utils.GetValue<bool>(config, ssPrefix, "ota");
                    auth1 = GetStr(ssPrefix, "password");
                    addr = Misc.Utils.GetAddr(config, ssPrefix, "address", "port");
                    method = GetStr(ssPrefix, "method");
                    break;
                case "http":
                case "socks":
                    var httpPrefix = root + ".settings.servers.0";
                    addr = Misc.Utils.GetAddr(config, httpPrefix, "address", "port");
                    auth1 = GetStr(httpPrefix, "users.0.user");
                    auth2 = GetStr(httpPrefix, "users.0.pass");
                    break;
                default:
                    break;
            }
        }

        private void ParseStreamSettings(string root, Func<string, string, string> GetStr)
        {

            var dict = new Dictionary<string, string>
            {
                {"kcp","mkcp" },
                {"h2","http/2" },
                {"ws","websocket" },
                {"ds","domainsocket" },
            };

            var sp = root + ".streamSettings";
            var st = GetStr(sp, "network");
            streamType = dict.ContainsKey(st) ? dict[st] : st;
            var index = GetIndexByNetwork(st);
            streamParam = index < 0 ? string.Empty : GetStr(sp, Models.Datas.Table.streamSettings[index].optionPath);
            useTls = GetStr(sp, "security") == "tls";
        }

        #endregion



    }
}
