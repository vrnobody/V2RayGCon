﻿namespace VgcApis.Interfaces.Services
{
    public interface IShareLinkMgrService
    {
        string DecodeShareLinkToMetadata(string shareLink);
        string EncodeMetadataToShareLink(string meta);

        Models.Datas.DecodeResult DecodeShareLinkToConfig(string shareLink);

        string EncodeConfigToShareLink(string name, string config);

        string EncodeConfigToShareLink(
            string name,
            string config,
            Models.Datas.Enums.LinkTypes linkType
        );

        int ImportLinksWithOutV2cfgLinksSync(string links, string mark);

        int UpdateSubscriptions(bool isSocks5, int proxyPort);
    }
}
