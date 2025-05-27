using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services.ShareLinkComponents;
using VgcApis.Models.Datas;

namespace V2RayGCon.Services
{
    public sealed class ShareLinkMgr
        : BaseClasses.SingletonService<ShareLinkMgr>,
            VgcApis.Interfaces.Services.IShareLinkMgrService
    {
        Settings settings;
        Servers servers;
        readonly Codecs codecs;

        public ShareLinkMgr()
        {
            codecs = new Codecs();
        }

        #region properties

        #endregion

        #region IShareLinkMgrService methods
        public string ToServerConfig(SharelinkMetaData meta)
        {
            return this.codecs.ToServerConfig(meta);
        }

        public string DecodeShareLinkToMetadata(string shareLink)
        {
            var r = DecodeShareLinkToConfig(shareLink);
            if (Comm.TryParseConfig(r.config, out var meta) && meta != null)
            {
                meta.name = r.name;
                return JsonConvert.SerializeObject(meta);
            }
            return null;
        }

        public bool TryParseConfig(string config, out SharelinkMetaData meta)
        {
            var r = Comm.TryParseConfig(config, out meta);
            return r;
        }

        public string EncodeMetadataToShareLink(string meta)
        {
            try
            {
                var m = JsonConvert.DeserializeObject<SharelinkMetaData>(meta);
                return m.ToShareLink();
            }
            catch { }
            return null;
        }

        /// <summary>
        /// return null if fail!
        /// </summary>
        public DecodeResult DecodeShareLinkToConfig(string shareLink)
        {
            var linkType = VgcApis.Misc.Utils.DetectLinkType(shareLink);
            switch (linkType)
            {
                case Enums.LinkTypes.ss:
                    return codecs.Decode<SsDecoder>(shareLink);
                case Enums.LinkTypes.vmess:
                    return codecs.Decode<VmessDecoder>(shareLink);
                case Enums.LinkTypes.v2cfg:
                    return codecs.Decode<V2cfgDecoder>(shareLink);
                case Enums.LinkTypes.vless:
                    return codecs.Decode<VlessDecoder>(shareLink);
                case Enums.LinkTypes.trojan:
                    return codecs.Decode<TrojanDecoder>(shareLink);
                case Enums.LinkTypes.socks:
                    return codecs.Decode<SocksDecoder>(shareLink);
                default:
                    break;
            }
            return null;
        }

        public string EncodeConfigToShareLink(string name, string config)
        {
            foreach (var linkType in linkTypes)
            {
                var link = EncodeConfigToShareLink(name, config, linkType);
                if (!string.IsNullOrEmpty(link))
                {
                    return link;
                }
            }
            return null;
        }

        /// <summary>
        /// return null if fail!
        /// </summary>
        public string EncodeConfigToShareLink(string name, string config, Enums.LinkTypes linkType)
        {
            switch (linkType)
            {
                case Enums.LinkTypes.socks:
                    return codecs.Encode<SocksDecoder>(name, config);
                case Enums.LinkTypes.ss:
                    return codecs.Encode<SsDecoder>(name, config);
                case Enums.LinkTypes.vmess:
                    return codecs.Encode<VmessDecoder>(name, config);
                case Enums.LinkTypes.v2cfg:
                    return codecs.Encode<V2cfgDecoder>(name, config);
                case Enums.LinkTypes.vless:
                    return codecs.Encode<VlessDecoder>(name, config);
                case Enums.LinkTypes.trojan:
                    return codecs.Encode<TrojanDecoder>(name, config);
                default:
                    return null;
            }
        }
        #endregion

        #region public methods
        public T GetCodec<T>()
            where T : VgcApis.BaseClasses.ComponentOf<Codecs>
        {
            return codecs.GetChild<T>();
        }

        public int UpdateSubscriptions(bool isSocks5, int proxyPort)
        {
            var enabledSubs = settings.GetSubscriptionItems().Where(s => s.isUse).ToList();

            var links = Misc.Utils.FetchLinksFromSubcriptions(enabledSubs, isSocks5, proxyPort);
            var decoders = codecs.GetDecoders(false);
            var results = ImportLinksBatchModeSync(links, decoders);
            var n = CountImportSuccessResult(results);
            VgcApis.Misc.Utils.ClearRegexCache();
            return n;
        }

        public int ImportLinksWithOutV2cfgLinksSync(string links, string mark)
        {
            if (string.IsNullOrEmpty(links))
            {
                return 0;
            }

            var pair = new string[] { links, mark ?? "" };
            var linkList = new List<string[]> { pair };
            var decoders = codecs.GetDecoders(false);
            var results = ImportLinksBatchModeSync(linkList, decoders);

            // servers.UpdateAllServersSummary();

            return CountImportSuccessResult(results);
        }

        public void ImportLinkWithOutV2cfgLinksBatchModeSync(IEnumerable<string[]> linkList)
        {
            var decoders = codecs.GetDecoders(false);
            var results = ImportLinksBatchModeSync(linkList, decoders);
            ShowImportResults(results);
        }

        public void ImportLinkWithOutV2cfgLinks(string text)
        {
            var pair = new string[] { text, "" };
            var linkList = new List<string[]> { pair };
            var decoders = codecs.GetDecoders(false);
            ImportLinksBatchModeAsync(linkList, decoders, true);
        }

        public void ImportLinkWithV2cfgLinks(string text)
        {
            var pair = new string[] { text, "" };
            var linkList = new List<string[]> { pair };
            var decoders = codecs.GetDecoders(true);
            ImportLinksBatchModeAsync(linkList, decoders, true);
        }

        public void Run(Settings settings, Servers servers)
        {
            this.settings = settings;
            this.servers = servers;

            codecs.Run(settings);
        }

        #endregion

        #region private methods
        static readonly List<Enums.LinkTypes> linkTypes = new List<Enums.LinkTypes>
        {
            Enums.LinkTypes.vmess,
            Enums.LinkTypes.vless,
            Enums.LinkTypes.trojan,
            Enums.LinkTypes.ss,
            Enums.LinkTypes.socks,
        };

        int CountImportSuccessResult(IEnumerable<string[]> result)
        {
            return result.Where(r => VgcApis.Misc.Utils.IsImportResultSuccess(r)).Count();
        }

        void ImportLinksBatchModeAsync(
            IEnumerable<string[]> linkList,
            IEnumerable<VgcApis.Interfaces.IShareLinkDecoder> decoders,
            bool isShowImportResults
        )
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
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
        IEnumerable<string[]> ImportLinksBatchModeSync(
            IEnumerable<string[]> linkList,
            IEnumerable<VgcApis.Interfaces.IShareLinkDecoder> decoders
        )
        {
            var jobs = new List<Tuple<string, string, VgcApis.Interfaces.IShareLinkDecoder>>();

            foreach (var link in linkList)
            {
                foreach (var decoder in decoders)
                {
                    jobs.Add(
                        new Tuple<string, string, VgcApis.Interfaces.IShareLinkDecoder>(
                            link[0],
                            link[1],
                            decoder
                        )
                    );
                }
            }

            List<string[]> worker(Tuple<string, string, VgcApis.Interfaces.IShareLinkDecoder> job)
            {
                return ImportShareLinks(job.Item1, job.Item2, job.Item3);
            }

            var results = VgcApis.Misc.Utils.ExecuteInParallel(jobs, worker).SelectMany(r => r);
            servers.RequireFormMainReload();
            return results;
        }

        void ShowImportResults(IEnumerable<string[]> results)
        {
            var c = results.Count();

            if (c <= 0)
            {
                VgcApis.Misc.UI.MsgBox(I18N.NoLinkFound);
                return;
            }

            if (IsImportAnyServer(results))
            {
                servers.RequireFormMainReload();
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                Views.WinForms.FormImportLinksResult.ShowResult(results);
            });
        }

        private List<string[]> ImportShareLinks(
            string text,
            string mark,
            VgcApis.Interfaces.IShareLinkDecoder decoder
        )
        {
            var links = decoder.ExtractLinksFromText(text);
            var results = links
                .Select(link =>
                {
                    var r = codecs.Decode(link, decoder);
                    var msg = AddLinkToServerList(r, mark);
                    return GenImportResult(link, msg.Item1, msg.Item2, mark);
                })
                .ToList();
            return results;
        }

        bool IsImportAnyServer(IEnumerable<string[]> importResults)
        {
            return importResults.Any(r => VgcApis.Misc.Utils.IsImportResultSuccess(r));
        }

        private Tuple<bool, string> AddLinkToServerList(DecodeResult r, string mark)
        {
            if (r == null || string.IsNullOrEmpty(r.config))
            {
                return new Tuple<bool, string>(false, I18N.DecodeFail);
            }
            var uid = servers.AddServer(r.name, r.config, mark, true);
            var ok = !string.IsNullOrEmpty(uid);
            var reason = ok ? I18N.Success : I18N.DuplicateServer;
            return new Tuple<bool, string>(ok, reason);
        }

        string[] GenImportResult(string link, bool success, string reason, string mark)
        {
            var importSuccessMark = success
                ? VgcApis.Models.Consts.Import.MarkImportSuccess
                : VgcApis.Models.Consts.Import.MarkImportFail;

            return new string[]
            {
                string.Empty, // reserved for index
                link,
                mark,
                importSuccessMark, // be aware of IsImportResultSuccess()
                reason,
            };
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            VgcApis.Libs.Sys.FileLogger.Info("ShareLinkMgr.Cleanup() begin");
            codecs?.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info("ShareLinkMgr.Cleanup() done");
        }
        #endregion
    }
}
