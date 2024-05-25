namespace V2RayGCon.Models.Datas
{
    public class ImportSharelinkOptions
    {
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
