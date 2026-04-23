using VgcApis.Libs.Infr;
using VgcApis.Models.Datas;

namespace VgcApis.Interfaces.Services
{
    public interface IShareLinkMgrService
    {
        string GenServerSideConfig(SharelinkMetaData meta);

        string DecodeShareLinkToMetadata(string shareLink);

        string EncodeMetadataToShareLink(string meta);

        DecodeResult DecodeShareLinkToConfig(string shareLink);

        string EncodeConfigToShareLink(string name, string config);

        bool TryParseConfig(string config, out SharelinkMetaData meta);

        string EncodeConfigToShareLink(string name, string config, Enums.LinkTypes linkType);

        int ImportLinksWithOutV2cfgSync(string links, string mark);

        ImportResultRecorder ImportZipPackageSync(
            string url,
            string mark,
            int maxCount,
            int timeout,
            bool isSocks5,
            int proxyPort,
            string proxyUsername,
            string proxyPassword
        );

        int UpdateSubscriptions(bool isSocks5, int proxyPort);
    }
}
