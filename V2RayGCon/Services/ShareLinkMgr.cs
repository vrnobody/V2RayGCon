using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services.ShareLinkComponents;
using VgcApis.Interfaces;
using VgcApis.Libs.Infr;
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
        public string GenServerSideConfig(SharelinkMetaData meta)
        {
            return this.codecs.GenServerSideConfig(meta);
        }

        public string DecodeShareLinkToMetadata(string shareLink)
        {
            var r = DecodeShareLinkToConfig(shareLink);
            if (VgcApis.Misc.OutbMeta.TryParseConfig(r.config, out var meta) && meta != null)
            {
                meta.name = r.name;
                return JsonConvert.SerializeObject(meta);
            }
            return null;
        }

        public bool TryParseConfig(string config, out SharelinkMetaData meta)
        {
            var r = VgcApis.Misc.OutbMeta.TryParseConfig(config, out meta);
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
                case Enums.LinkTypes.hy2:
                    return codecs.Decode<Hy2Decoder>(shareLink);
                case Enums.LinkTypes.ss:
                    return codecs.Decode<SsDecoder>(shareLink);
                case Enums.LinkTypes.vmess:
                    return codecs.Decode<VmessDecoder>(shareLink);
                case Enums.LinkTypes.v2cfg:
                    return codecs.Decode<V2cfgDecoder>(shareLink);
                case Enums.LinkTypes.vless:
                    return codecs.Decode<VlessDecoder>(shareLink);
                case Enums.LinkTypes.mob:
                    return codecs.Decode<MobDecoder>(shareLink);
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
                case Enums.LinkTypes.hy2:
                    return codecs.Encode<Hy2Decoder>(name, config);
                case Enums.LinkTypes.trojan:
                    return codecs.Encode<TrojanDecoder>(name, config);
                case Enums.LinkTypes.mob:
                    return codecs.Encode<MobDecoder>(name, config);
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

        public ImportResultRecorder UpdateSubsCore(
            List<Models.Datas.SubscriptionItem> validSubs,
            bool isSocks5,
            int proxyPort,
            bool withDetail
        )
        {
            var decoders = codecs.GetDecoders(false);
            var recorder = new ImportResultRecorder();

            void dispatchSubs(Models.Datas.SubscriptionItem sub)
            {
                var mark = sub.isSetMark ? sub.alias : "";
                var url = sub.url;
                if (url.ToLower().EndsWith(".zip"))
                {
                    handleZipSub(mark, url);
                }
                else
                {
                    handleTextSub(mark, url);
                }
                servers.RequireFormMainReload();
            }

            void decode(string mark, string text, Action onAddNew, CancellationToken token)
            {
                AddServersFromText(decoders, recorder, mark, text, onAddNew, withDetail, token);
            }

            void handleZipSub(string mark, string url)
            {
                var zipTask = new ImportComponents.ZipTask(mark, url);
                zipTask.Exec(decode, isSocks5, proxyPort);
            }

            void handleTextSub(string mark, string url)
            {
                var textTask = new ImportComponents.TextTask(mark, url);
                textTask.Exec(decode, isSocks5, proxyPort);
            }

            VgcApis.Misc.Utils.ExecuteInParallel(validSubs, dispatchSubs);
            VgcApis.Misc.Utils.ClearRegexCache();
            return recorder;
        }

        public int UpdateSubscriptions(bool isSocks5, int proxyPort)
        {
            var validSubs = settings
                .GetSubscriptionItems()
                .Where(s => s.isUse && !string.IsNullOrEmpty(s.url))
                .ToList();
            var recoder = UpdateSubsCore(validSubs, isSocks5, proxyPort, false);
            return recoder.GetSuccessCount();
        }

        // username is proxy username so as password
        public ImportResultRecorder ImportZipPackageSync(
            string url,
            string mark,
            int maxCount,
            int timeout,
            bool isSocks5,
            int proxyPort,
            string proxyUsername,
            string proxyPassword
        )
        {
            var decoders = codecs.GetDecoders(false);
            var recorder = new ImportResultRecorder();

            void decode(string mrk, string text, Action onAddNew, CancellationToken token)
            {
                AddServersFromText(decoders, recorder, mrk, text, onAddNew, false, token);
            }

            var zipTask = new ImportComponents.ZipTask(mark, url, maxCount, timeout);
            zipTask.Exec(decode, isSocks5, proxyPort);
            recorder.ErrorMessage = zipTask.GetErrorMessage();
            servers.RequireFormMainReload();
            return recorder;
        }

        public int ImportLinksWithOutV2cfgSync(string links, string mark)
        {
            if (string.IsNullOrEmpty(links))
            {
                return 0;
            }

            var decoders = codecs.GetDecoders(false);
            var recorder = new ImportResultRecorder();
            AddServersFromText(
                decoders,
                recorder,
                mark,
                links,
                null,
                false,
                CancellationToken.None
            );
            return recorder.GetSuccessCount();
        }

        public void ImportLinkWithOutV2cfgUi(string text)
        {
            ImportLinksUiCoreBg(text, false);
        }

        public void ImportLinkWithV2cfgUi(string text)
        {
            ImportLinksUiCoreBg(text, true);
        }

        public void Run(Settings settings, Servers servers)
        {
            this.settings = settings;
            this.servers = servers;

            codecs.Run(settings);
        }

        #endregion

        #region private methods
        void AddServersFromText(
            List<IShareLinkDecoder> decoders,
            ImportResultRecorder recorder,
            string mark,
            string text,
            Action onAddNew,
            bool withDetail,
            CancellationToken token
        )
        {
            void record(string link, bool success, string reason)
            {
                if (withDetail)
                {
                    recorder.Record(mark, link, success, reason);
                }
                else
                {
                    recorder.Record(success);
                }
            }

            foreach (var decoder in decoders)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                var links = decoder.ExtractLinksFromText(text);
                foreach (var link in links)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    var r = codecs.Decode(link, decoder);
                    if (r == null || string.IsNullOrEmpty(r.config))
                    {
                        record(link, false, I18N.DecodeFail);
                        continue;
                    }
                    var uid = servers.AddServer(r.name, r.config, mark, true);
                    if (string.IsNullOrEmpty(uid))
                    {
                        record(link, false, I18N.DuplicateServer);
                    }
                    else
                    {
                        onAddNew?.Invoke();
                        record(link, true, I18N.Success);
                    }
                }
            }
        }

        void ImportLinksUiCoreBg(string links, bool includeV2cfgLinks)
        {
            if (string.IsNullOrEmpty(links))
            {
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var decoders = codecs.GetDecoders(includeV2cfgLinks);
                var recorder = new ImportResultRecorder();
                AddServersFromText(
                    decoders,
                    recorder,
                    "",
                    links,
                    null,
                    true,
                    CancellationToken.None
                );
                ShowImportResults(recorder);
            });
        }

        static readonly List<Enums.LinkTypes> linkTypes = new List<Enums.LinkTypes>
        {
            Enums.LinkTypes.vmess,
            Enums.LinkTypes.vless,
            Enums.LinkTypes.trojan,
            Enums.LinkTypes.hy2,
            Enums.LinkTypes.ss,
            Enums.LinkTypes.socks,
            Enums.LinkTypes.mob,
        };

        public void ShowImportResults(ImportResultRecorder recoder)
        {
            var c = recoder.GetTotalCount();
            if (c <= 0)
            {
                VgcApis.Misc.UI.MsgBox(I18N.NoLinkFound);
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                servers.RequireFormMainReload();
                Views.WinForms.FormImportLinksResult.ShowResult(recoder);
            });
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
