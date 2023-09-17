namespace VgcApis.Interfaces.Services
{
    public interface IShareLinkMgrService
    {
        Models.Datas.DecodeResult DecodeShareLinkToConfig(string shareLink);

        string EncodeConfigToShareLink(string name, string config);

        string EncodeConfigToShareLink(
            string name,
            string config,
            Models.Datas.Enums.LinkTypes linkType
        );

        int ImportLinksWithOutV2cfgLinksSync(string links, string mark);

        int UpdateSubscriptions(int proxyPort);
    }
}
