using VgcApis.Models.Datas;

namespace VgcApis.Interfaces.Services
{
    public interface IShareLinkMgrService
    {
        string DecodeShareLinkToMetadata(string shareLink);
        string EncodeMetadataToShareLink(string meta);

        DecodeResult DecodeShareLinkToConfig(string shareLink);

        string EncodeConfigToShareLink(string name, string config);

        bool TryParseConfig(string config, out SharelinkMetaData meta);

        string EncodeConfigToShareLink(string name, string config, Enums.LinkTypes linkType);

        int ImportLinksWithOutV2cfgLinksSync(string links, string mark);

        int UpdateSubscriptions(bool isSocks5, int proxyPort);
    }
}
