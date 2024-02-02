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
