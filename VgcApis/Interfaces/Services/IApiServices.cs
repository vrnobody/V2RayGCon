namespace VgcApis.Interfaces.Services
{
    public interface IApiService
    {
        IPostOffice GetPostOfficeService();

        INotifierService GetNotifierService();

        ISettingsService GetSettingService();
        IServersService GetServersService();
        IConfigMgrService GetConfigMgrService();
        IWebService GetWebService();
        IUtilsService GetUtilsService();
        IShareLinkMgrService GetShareLinkMgrService();
    }
}
