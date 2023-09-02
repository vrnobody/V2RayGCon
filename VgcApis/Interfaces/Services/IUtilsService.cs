using System;
using System.Collections.Generic;

namespace VgcApis.Interfaces.Services
{
    public interface IUtilsService
    {
        #region misc
        string GetAppVersion();

        string AddLinkPrefix(string linkBody, VgcApis.Models.Datas.Enums.LinkTypes type);

        string GetLinkBody(string link);

        void ExecuteInParallel<TParam>(IEnumerable<TParam> source, Action<TParam> worker);

        void ExecuteInParallel<TParam, TResult>(
            IEnumerable<TParam> source,
            Func<TParam, TResult> worker
        );

        string ScanQrcode();
        #endregion
    }
}
