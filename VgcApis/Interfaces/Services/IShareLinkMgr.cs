using System.Collections.Generic;

namespace VgcApis.Interfaces.Services
{
    public interface IShareLinkMgrService
    {
        string DecodeShareLinkToConfig(string shareLink);

        string EncodeConfigToShareLink(
            string config, Models.Datas.Enum.LinkTypes linkType);

        List<Interfaces.IShareLinkDecoder> GenDecoderList(
            bool isIncludeV2cfgDecoder);

        List<string[]> ImportLinksBatchMode(
           IEnumerable<string[]> linkList,
           IEnumerable<Interfaces.IShareLinkDecoder> decoders);


        int UpdateSubscriptions(int proxyPort);
    }
}
