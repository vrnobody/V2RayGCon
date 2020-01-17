namespace VgcApis.Models.IServices

{
    public interface IApiService
    {
        INotifierService GetNotifierService();

        ISettingsService GetSettingService();
        IServersService GetServersService();
        IConfigMgrService GetConfigMgrService();
        IWebService GetWebService();
        IUtilsService GetUtilsService();
        IShareLinkMgrService GetShareLinkMgrService();
    }
}
