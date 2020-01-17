using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Service
{
    internal sealed class ShareLinkMgr :
        Model.BaseClass.SingletonService<ShareLinkMgr>,
        VgcApis.Models.IServices.IShareLinkMgrService
    {
        Setting setting;
        Servers servers;

        ShareLinkComponents.Codecs codecs;

        public ShareLinkMgr()
        {
            codecs = new ShareLinkComponents.Codecs();
        }

        #region properties

        #endregion

        #region IShareLinkMgrService methods

        /// <summary>
        /// return null if fail!
        /// </summary>
        public string DecodeShareLinkToConfig(string shareLink)
        {
            var linkType = VgcApis.Libs.Utils.DetectLinkType(shareLink);
            switch (linkType)
            {
                case VgcApis.Models.Datas.Enum.LinkTypes.v:
                    return codecs.Decode<ShareLinkComponents.VeeDecoder>(shareLink);
                case VgcApis.Models.Datas.Enum.LinkTypes.vmess:
                    return codecs.Decode<ShareLinkComponents.VmessDecoder>(shareLink);
                case VgcApis.Models.Datas.Enum.LinkTypes.v2cfg:
                    return codecs.Decode<ShareLinkComponents.V2cfgDecoder>(shareLink);
                default:
                    return null;
            }
        }

        /// <summary>
        /// return null if fail!
        /// </summary>
        public string EncodeConfigToShareLink(
            string config,
            VgcApis.Models.Datas.Enum.LinkTypes linkType)
        {
            switch (linkType)
            {
                case VgcApis.Models.Datas.Enum.LinkTypes.v:
                    return codecs.Encode<ShareLinkComponents.VeeDecoder>(config);
                case VgcApis.Models.Datas.Enum.LinkTypes.vmess:
                    return codecs.Encode<ShareLinkComponents.VmessDecoder>(config);
                case VgcApis.Models.Datas.Enum.LinkTypes.v2cfg:
                    return codecs.Encode<ShareLinkComponents.V2cfgDecoder>(config);
                default:
                    return null;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// <para>linkList=List(string[]{0: text, 1: mark}>)</para>
        /// <para>decoders = List(IShareLinkDecoder)</para>
        /// </summary>
        public List<string[]> ImportLinksBatchMode(
            IEnumerable<string[]> linkList,
            IEnumerable<VgcApis.Models.Interfaces.IShareLinkDecoder> decoders) =>
            ImportLinksBatchModeSync(linkList, decoders);

        public List<VgcApis.Models.Interfaces.IShareLinkDecoder> GenDecoderList(
            bool isIncludeV2cfgDecoder)
        {
            var decoders = new List<VgcApis.Models.Interfaces.IShareLinkDecoder>
            {
                codecs.GetComponent<ShareLinkComponents.VmessDecoder>(),
                codecs.GetComponent<ShareLinkComponents.VeeDecoder>(),
            };

            if (setting.CustomDefImportSsShareLink)
            {
                decoders.Add(codecs.GetComponent<ShareLinkComponents.SsDecoder>());
            }

            if (isIncludeV2cfgDecoder)
            {
                decoders.Add(codecs.GetComponent<ShareLinkComponents.V2cfgDecoder>());
            }

            return decoders;
        }

        public int UpdateSubscriptions(int proxyPort)
        {
            var subs = setting.GetSubscriptionItems();
            var links = Lib.Utils.FetchLinksFromSubcriptions(subs, proxyPort);
            var decoders = GenDecoderList(false);
            var results = ImportLinksBatchModeSync(links, decoders);
            var count = results.Where(r => VgcApis.Libs.Utils.IsImportResultSuccess(r)).Count();
            return count;
        }

        public void ImportLinkWithOutV2cfgLinksBatchMode(
            IEnumerable<string[]> linkList)
        {
            var decoders = GenDecoderList(false);
            ImportLinksBatchModeThen(linkList, decoders, true);
        }

        public void ImportLinkWithOutV2cfgLinks(string text)
        {
            var pair = new string[] { text, "" };
            var linkList = new List<string[]> { pair };
            var decoders = GenDecoderList(false);
            ImportLinksBatchModeThen(linkList, decoders, true);
        }

        public void ImportLinkWithV2cfgLinks(string text)
        {
            var pair = new string[] { text, "" };
            var linkList = new List<string[]> { pair };
            var decoders = GenDecoderList(true);

            ImportLinksBatchModeThen(linkList, decoders, true);
        }

        public void Run(
           Setting setting,
           Servers servers,
           Cache cache)
        {
            this.setting = setting;
            this.servers = servers;

            codecs.Run(cache, setting);
        }

        #endregion

        #region private methods
        void ImportLinksBatchModeThen(
            IEnumerable<string[]> linkList,
            IEnumerable<VgcApis.Models.Interfaces.IShareLinkDecoder> decoders,
            bool isShowImportResults)
        {
            VgcApis.Libs.Utils.RunInBackground(() =>
            {
                var results = ImportLinksBatchModeSync(linkList, decoders);
                if (isShowImportResults)
                {
                    ShowImportResults(results);
                }

            });
        }

        /// <summary>
        /// <para>linkList=List(string[]{0: text, 1: mark}>)</para>
        /// <para>decoders = List(IShareLinkDecoder)</para>
        /// </summary>
        List<string[]> ImportLinksBatchModeSync(
            IEnumerable<string[]> linkList,
            IEnumerable<VgcApis.Models.Interfaces.IShareLinkDecoder> decoders)
        {
            var jobs = new List<Tuple<string, string, VgcApis.Models.Interfaces.IShareLinkDecoder>>();

            foreach (var link in linkList)
            {
                foreach (var decoder in decoders)
                {
                    jobs.Add(new Tuple<string, string, VgcApis.Models.Interfaces.IShareLinkDecoder>(
                        link[0], link[1], decoder));
                }
            }

            List<string[]> worker(Tuple<string, string, VgcApis.Models.Interfaces.IShareLinkDecoder> job)
            {
                return ImportShareLinks(job.Item1, job.Item2, job.Item3);
            }

            var results = Lib.Utils.ExecuteInParallel(jobs, worker);
            return results.SelectMany(r => r).ToList();
        }

        void ShowImportResults(IEnumerable<string[]> results)
        {
            var list = results.ToList();

            if (list.Count <= 0)
            {
                MessageBox.Show(I18N.NoLinkFound);
                return;
            }

            if (IsAddNewServer(list))
            {
                servers.UpdateAllServersSummaryBg();
            }

            var form = new Views.WinForms.FormImportLinksResult(list);
            form.Show();
            Application.Run();
        }

        private List<string[]> ImportShareLinks(
            string text,
            string mark,
            VgcApis.Models.Interfaces.IShareLinkDecoder decoder)
        {
            var links = decoder.ExtractLinksFromText(text);

            // Do not use ExecuteInParallel here!
            // Because server's order may changes!

            var results = new List<string[]>();
            foreach (var link in links)
            {
                var decodedConfig = codecs.Decode(link, decoder);
                var msg = AddLinkToServerList(mark, decodedConfig);
                var result = GenImportResult(link, msg.Item1, msg.Item2, mark);
                results.Add(result);
            }

            return results;
        }

        bool IsAddNewServer(IEnumerable<string[]> importResults)
        {
            foreach (var result in importResults)
            {
                if (VgcApis.Libs.Utils.IsImportResultSuccess(result))
                {
                    return true;
                }
            }
            return false;
        }

        private Tuple<bool, string> AddLinkToServerList(
            string mark,
            string decodedConfig)
        {
            if (string.IsNullOrEmpty(decodedConfig))
            {
                return new Tuple<bool, string>(false, I18N.DecodeFail);
            }
            var isSuccess = servers.AddServer(decodedConfig, mark, true);
            var reason = isSuccess ? I18N.Success : I18N.DuplicateServer;
            return new Tuple<bool, string>(isSuccess, reason);
        }

        string[] GenImportResult(
            string link,
            bool success,
            string reason,
            string mark)
        {
            var importSuccessMark = success ?
                VgcApis.Models.Consts.Import.MarkImportSuccess :
                VgcApis.Models.Consts.Import.MarkImportFail;


            return new string[]
            {
                string.Empty,  // reserved for index
                link,
                mark,
                importSuccessMark,  // be aware of IsImportResultSuccess()
                reason,
            };
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            codecs?.Dispose();
        }
        #endregion
    }
}
