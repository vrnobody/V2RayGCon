namespace V2RayGCon.Models.Datas
{
    public class ImportSharelinkOptions
    {
        public bool IsImportTrojanShareLink { get; set; }

        public bool IsImportSsShareLink { get; set; }
        public bool IsInjectGlobalImport { get; set; }
        public bool IsBypassCnSite { get; set; }

        public int Mode { get; set; } // Models.Datas.Enum.ProxyTypes
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

        public bool Equals(ImportSharelinkOptions target)
        {
            if (
                target == null
                || IsImportTrojanShareLink != target.IsImportTrojanShareLink
                || IsImportSsShareLink != target.IsImportSsShareLink
                || IsInjectGlobalImport != target.IsInjectGlobalImport
                || IsBypassCnSite != target.IsBypassCnSite
                || Mode != target.Mode
                || !Ip.Equals(target.Ip)
                || Port != target.Port
            )
            {
                return false;
            }
            return true;
        }
    }
}
