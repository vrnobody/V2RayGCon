using System;
using System.Collections.Generic;
using System.Threading;

namespace V2RayGCon.Libs.Lua.ApiComponents
{
    public class UtilsApi
        : VgcApis.BaseClasses.Disposable,
            VgcApis.Interfaces.Services.IUtilsService
    {
        #region misc

        public string GetAppVersion() => Misc.Utils.GetAssemblyVersion();

        public string AddLinkPrefix(string linkBody, VgcApis.Models.Datas.Enums.LinkTypes type) =>
            Misc.Utils.AddLinkPrefix(linkBody, type);

        public string GetLinkBody(string link)
        {
            try
            {
                return Misc.Utils.GetLinkBody(link);
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

        public void ExecuteInParallel<TParam>(IEnumerable<TParam> source, Action<TParam> worker) =>
            Misc.Utils.ExecuteInParallel(source, worker);

        public void ExecuteInParallel<TParam, TResult>(
            IEnumerable<TParam> source,
            Func<TParam, TResult> worker
        ) => Misc.Utils.ExecuteInParallel(source, worker);
        #endregion
    }
}
