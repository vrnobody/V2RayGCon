using System;
using System.Collections.Generic;

namespace VgcApis.Interfaces.Services
{
    public interface IUtilsService
    {
        #region misc
        string GetAppVersion();

        string ScanQrcode();
        #endregion
    }
}
