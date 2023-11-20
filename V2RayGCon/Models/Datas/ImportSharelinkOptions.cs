namespace V2RayGCon.Models.Datas
{
    public class ImportSharelinkOptions
    {
        // obsolete 预计2024-06删除
        // ----------------------------------
        public int Mode { get; set; } = (int)Enums.ProxyTypes.HTTP; // Models.Datas.Enum.ProxyTypes
        public bool IsBypassCnSite { get; set; } = false;
        public bool IsInjectGlobalImport { get; set; } = false;

        // ----------------------------------

        public bool IsImportVmessShareLink { get; set; } = true;
        public bool IsImportVlessShareLink { get; set; } = true;

        public bool IsImportTrojanShareLink { get; set; } = true;

        public bool IsImportSocksShareLink { get; set; } = true;

        public bool IsImportSsShareLink { get; set; } = true;

        public string DefaultCoreName { get; set; } = "";

        public string DefaultInboundName { get; set; } = "http";

        public string Ip { get; set; } = VgcApis.Models.Consts.Webs.LoopBackIP;
        public int Port { get; set; } = VgcApis.Models.Consts.Webs.DefaultProxyPort;

        public ImportSharelinkOptions() { }
    }
}
