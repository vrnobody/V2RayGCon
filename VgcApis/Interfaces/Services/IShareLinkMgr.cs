using System.Collections.Generic;

namespace VgcApis.Interfaces.Services
{
    public interface IShareLinkMgrService
    {
        string DecodeShareLinkToConfig(string shareLink);

        string EncodeConfigToShareLink(string config, Models.Datas.Enums.LinkTypes linkType);

        int ImportLinksWithOutV2cfgLinksSync(string links, string mark);

        int UpdateSubscriptions(int proxyPort);
    }
}
