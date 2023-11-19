namespace V2RayGCon.Models.Datas
{
    public class ImportSharelinkOptions
    {
        // obsolete 预计2024-06删除
        // ----------------------------------
        public int Mode { get; set; } // Models.Datas.Enum.ProxyTypes

        // ----------------------------------

        public bool IsImportTrojanShareLink { get; set; }

        public bool IsImportSocksShareLink { get; set; } = false;
        public bool IsImportSsShareLink { get; set; }
        public bool IsInjectGlobalImport { get; set; }
        public bool IsBypassCnSite { get; set; }

        public string DefaultCoreName { get; set; } = "";

        public string DefaultInboundName { get; set; } = "http";

        public string Ip { get; set; }
        public int Port { get; set; }

        public ImportSharelinkOptions()
        {
            IsImportTrojanShareLink = false;
            IsImportSsShareLink = false;
            IsInjectGlobalImport = false;
            IsBypassCnSite = false;

            Mode = (int)Enums.ProxyTypes.HTTP;
            Ip = VgcApis.Models.Consts.Webs.LoopBackIP;
            Port = VgcApis.Models.Consts.Webs.DefaultProxyPort;
        }
    }
}
