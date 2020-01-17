using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2RayGCon.Model.Data
{
    public class ImportSharelinkOptions
    {
        public bool IsFold { get; set; } // single line mode
        public bool IsImportSsShareLink { get; set; }
        public bool IsInjectGlobalImport { get; set; }
        public bool IsBypassCnSite { get; set; }

        public int Mode { get; set; } // Model.Data.Enum.ProxyTypes
        public string Ip { get; set; }
        public int Port { get; set; }

        public ImportSharelinkOptions()
        {
            IsFold = false;
            IsImportSsShareLink = true;
            IsInjectGlobalImport = false;
            IsBypassCnSite = false;

            Mode = (int)Enum.ProxyTypes.HTTP;
            Ip = VgcApis.Models.Consts.Webs.LoopBackIP;
            Port = VgcApis.Models.Consts.Webs.DefaultProxyPort;
        }

        public bool Equals(ImportSharelinkOptions target)
        {
            if (target == null
                || IsFold != target.IsFold
                || IsImportSsShareLink != target.IsImportSsShareLink
                || IsInjectGlobalImport != target.IsInjectGlobalImport
                || IsBypassCnSite != target.IsBypassCnSite
                || Mode != target.Mode
                || !Ip.Equals(target.Ip)
                || Port != target.Port)
            {
                return false;
            }
            return true;
        }
    }
}
