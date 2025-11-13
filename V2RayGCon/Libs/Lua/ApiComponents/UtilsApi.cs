using System.Threading;

namespace V2RayGCon.Libs.Lua.ApiComponents
{
    public class UtilsApi
        : VgcApis.BaseClasses.Disposable,
            VgcApis.Interfaces.Services.IUtilsService
    {
        #region misc

        public string GetAppVersion() => Misc.Utils.GetAssemblyVersion().ToString();

        public string AddLinkPrefix(string linkBody, VgcApis.Models.Datas.Enums.LinkTypes type) =>
            VgcApis.Misc.Utils.AddLinkPrefix(linkBody, type);

        public string GetLinkBody(string link)
        {
            try
            {
                return VgcApis.Misc.Utils.GetLinkBody(link);
            }
            catch { }
            return string.Empty;
        }

        public string ScanQrcode()
        {
            var shareLink = @"";
            AutoResetEvent are = new AutoResetEvent(false);

            void Set()
            {
                try
                {
                    are.Set();
                }
                catch { }
            }

            void Success(string result)
            {
                shareLink = result;
                Set();
            }

            QRCode.QRCode.ScanQRCode(Success, Set);
            are.WaitOne(10000);
            are.Dispose();

            return shareLink;
        }

        #endregion
    }
}
