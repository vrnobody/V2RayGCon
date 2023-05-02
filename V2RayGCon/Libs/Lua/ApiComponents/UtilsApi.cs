using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace V2RayGCon.Libs.Lua.ApiComponents
{
    public class UtilsApi :
        VgcApis.BaseClasses.Disposable,
        VgcApis.Interfaces.Services.IUtilsService
    {
        #region misc
        public string GetAppVersion() => Misc.Utils.GetAssemblyVersion();

        public string AddLinkPrefix(string linkBody, VgcApis.Models.Datas.Enums.LinkTypes type) =>
            Misc.Utils.AddLinkPrefix(linkBody, type);

        public string Base64Encode(string plainText)
        {
            try
            {
                return Misc.Utils.Base64Encode(plainText);
            }
            catch { }
            return null;
        }

        public string Base64Decode(string b64String)
        {
            try
            {
                return Misc.Utils.Base64Decode(b64String);
            }
            catch { }
            return null;
        }

        public string GetLinkBody(string link) =>
            Misc.Utils.GetLinkBody(link);

        public string ScanQrcode()
        {
            var shareLink = @"";
            AutoResetEvent are = new AutoResetEvent(false);

            void Success(string result)
            {
                shareLink = result;
                are.Set();
            }

            void Fail()
            {
                are.Set();
            }

            Libs.QRCode.QRCode.ScanQRCode(Success, Fail);
            are.WaitOne(10000);
            return shareLink;
        }

        public void ExecuteInParallel<TParam>(
            IEnumerable<TParam> source, Action<TParam> worker) =>
            Misc.Utils.ExecuteInParallel(source, worker);

        public void ExecuteInParallel<TParam, TResult>(
            IEnumerable<TParam> source, Func<TParam, TResult> worker) =>
            Misc.Utils.ExecuteInParallel(source, worker);
        #endregion

    }
}
