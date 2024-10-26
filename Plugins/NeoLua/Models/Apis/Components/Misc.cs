using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Neo.IronLua;
using NeoLuna.Services;
using Newtonsoft.Json;
using VgcApis.Interfaces;

namespace NeoLuna.Models.Apis.Components
{
    internal sealed class Misc : VgcApis.BaseClasses.ComponentOf<LuaApis>, Interfaces.ILuaMisc
    {
        readonly Settings settings;
        private readonly FormMgrSvc formMgr;
        readonly VgcApis.Interfaces.Services.IUtilsService vgcUtils;
        readonly VgcApis.Interfaces.Services.IShareLinkMgrService vgcSlinkMgr;
        readonly VgcApis.Interfaces.Services.INotifierService vgcNotifier;
        readonly VgcApis.Interfaces.Services.IServersService vgcServer;
        readonly VgcApis.Interfaces.Services.ISettingsService vgcSettings;

        public Misc(
            VgcApis.Interfaces.Services.IApiService api,
            Settings settings,
            FormMgrSvc formMgr
        )
        {
            this.settings = settings;
            this.formMgr = formMgr;
            vgcNotifier = api.GetNotifierService();
            vgcUtils = api.GetUtilsService();
            vgcSlinkMgr = api.GetShareLinkMgrService();
            vgcServer = api.GetServersService();
            vgcSettings = api.GetSettingService();
        }

        #region ILuaMisc.WinForms
        public void SetNotifyIconTag(string tag) => vgcNotifier.SetNotifyIconTag(tag);

        public void Notify(string title, string content, int ms) =>
            vgcNotifier.Notify(title, content, ms);

        public string ReadFromClipboard()
        {
            string r = "";
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = VgcApis.Misc.Utils.ReadFromClipboard();
            });
            return r;
        }

        public bool CopyToClipboard(string content)
        {
            bool ok = false;
            VgcApis.Misc.UI.Invoke(() =>
            {
                ok = VgcApis.Misc.Utils.CopyToClipboard(content);
            });
            return ok;
        }

        public void ShowFormTextEditor(string config) => vgcNotifier.ShowFormTextEditor(config);

        public void ShowFormServerSettings(ICoreServCtrl coreServ) =>
            vgcNotifier.ShowFormServerSettings(coreServ);

        public void ShowFormOption() => vgcNotifier.ShowFormOption();

        public void ShowFormNeoLunaMgr() => formMgr.ShowFormMain();

        public void ShowFormNeoLunaEditor() => formMgr.ShowOrCreateFirstEditor();

        public void ShowFormWebUI() => vgcNotifier.ShowFormWebUI();

        public void ShowFormNeoLunaLog() => formMgr.ShowFormLog();

        public void ShowFormMain() => vgcNotifier.ShowFormMain();

        public void ShowFormLog() => vgcNotifier.ShowFormLog();

        #endregion

        #region ILuaMisc.ImportLinks
        public int ImportLinks(string links, string mark) =>
            vgcSlinkMgr.ImportLinksWithOutV2cfgLinksSync(links, mark);

        #endregion

        #region ILuaMisc.Forms

        public string ShowData(string title, LuaTable columns, LuaTable rows, int defColumn)
        {
            var dt = LuaTableToDataTable(columns, rows);
            return ShowDataGridDialog(title, dt, defColumn);
        }

        public string ShowData(string title, LuaTable columns, LuaTable rows) =>
            ShowData(title, columns, rows, -1);

        public string BrowseFolder() => VgcApis.Misc.UI.ShowSelectFolderDialog();

        public string BrowseFile() =>
            VgcApis.Misc.UI.ShowSelectFileDialog(VgcApis.Models.Consts.Files.AllExt);

        public string BrowseFile(string filter) => VgcApis.Misc.UI.ShowSelectFileDialog(filter);

        public string Input(string title) => Input(title, 3);

        public string Input(string title, int lines) => Input(title, string.Empty, lines);

        public string Input(string title, string content, int lines)
        {
            Views.WinForms.FormInput creater(AutoResetEvent done) =>
                new Views.WinForms.FormInput(done, title, content, lines);
            return GetResult<Views.WinForms.FormInput, string>(creater);
        }

        public List<int> Choices(string title, LuaTable choices) => Choices(title, choices, false);

        public List<int> Choices(string title, LuaTable choices, bool isShowKey)
        {
            var list = global::NeoLuna.Misc.Utils.LuaTableToList(choices, isShowKey);
            return GetResultFromChoicesDialog(title, list.ToArray());
        }

        public List<int> Choices(string title, params string[] choices) =>
            GetResultFromChoicesDialog(title, choices);

        public int Choice(string title, LuaTable choices) => Choice(title, choices, false, -1);

        public int Choice(string title, LuaTable choices, bool isShowKey) =>
            Choice(title, choices, isShowKey, -1);

        public int Choice(string title, LuaTable choices, bool isShowKey, int selected)
        {
            var list = global::NeoLuna.Misc.Utils.LuaTableToList(choices, isShowKey);
            return GetResultFromChoiceDialog(title, list.ToArray(), selected);
        }

        public int Choice(string title, params string[] choices) =>
            GetResultFromChoiceDialog(title, choices, -1);

        public bool Confirm(string content) => VgcApis.Misc.UI.Confirm(content);

        public void Alert(string content) => VgcApis.Misc.UI.MsgBox(content);

        #endregion

        #region ILuaMisc.Encodings
        public List<string> GetOutboundTags(string config) =>
            VgcApis.Misc.Utils.GetAllOutboundTagsFromJson(config);

        public string ToJson(object o)
        {
            return VgcApis.Misc.Utils.ToJson(o);
        }

        public LuaTable DecodeShareLinkToMetadata(string shareLink)
        {
            try
            {
                var str = vgcSlinkMgr.DecodeShareLinkToMetadata(shareLink);
                return JsonConvert.DeserializeObject<LuaTable>(str);
            }
            catch { }
            return null;
        }

        public string EncodeMetadataToShareLink(LuaTable meta)
        {
            try
            {
                var s = JsonConvert.SerializeObject(meta);
                return vgcSlinkMgr.EncodeMetadataToShareLink(s);
            }
            catch { }
            return null;
        }

        public bool IsCompressedBase64(string str) =>
            VgcApis.Libs.Infr.ZipExtensions.IsCompressedBase64(str);

        public string CompressToBase64(string str) =>
            VgcApis.Libs.Infr.ZipExtensions.CompressToBase64(str);

        public string DecompressFromBase64(string str) =>
            VgcApis.Libs.Infr.ZipExtensions.DecompressFromBase64(str);

        public string Md5(string str)
        {
            return VgcApis.Misc.Utils.Md5Hex(str);
        }

        public string Sha256(string str)
        {
            return VgcApis.Misc.Utils.Sha256Hex(str);
        }

        public string Sha512(string str)
        {
            return VgcApis.Misc.Utils.Sha512Hex(str);
        }
        #endregion

        #region other ILuaMisc stuff
        public void ClearRegexCache()
        {
            VgcApis.Misc.Utils.ClearRegexCache();
        }

        public string DetectConfigType(string config)
        {
            return VgcApis.Misc.Utils.DetectConfigType(config).ToString();
        }

        public void Log(string message) => settings.SendLog(message);

        public string GetLogAsString()
        {
            return VgcApis.Misc.Logger.GetTrimedContent();
        }

        public int GetMutexPoolCount() => VgcApis.Libs.Tasks.MrePool.Count;

        public int GetMutexPoolMaxCount() => VgcApis.Libs.Tasks.MrePool.HistMaxCount;

        public int GetSpeedtestQueueLength() => vgcSettings.GetSpeedtestQueueLength();

        public string Replace(string text, string oldStr, string newStr) =>
            text?.Replace(oldStr, newStr);

        public string RandomHex(int len) => VgcApis.Misc.Utils.RandomHex(len);

        public string NewGuid() => Guid.NewGuid().ToString();

        public string GetUserSettings() => GetUserSettings(null);

        public string GetUserSettings(string props) => vgcSettings.GetUserSettings(props);

        public bool SetUserSettings(string props) => vgcSettings.SetUserSettings(props);

        public string GetSubscriptionConfig() => vgcSettings.GetSubscriptionConfig();

        public void SetSubscriptionConfig(string cfgStr) =>
            vgcSettings.SetSubscriptionConfig(cfgStr);

        public long GetTimeoutValue() => VgcApis.Models.Consts.Core.SpeedtestTimeout;

        public void RefreshFormMain() => vgcServer.RequireFormMainReload();

        public void WriteLocalStorage(string key, string value) =>
            settings.SetLuaShareMemory(key, value);

        public string ReadLocalStorage(string key) => settings.GetLuaShareMemory(key);

        public bool RemoveLocalStorage(string key) => settings.RemoveShareMemory(key);

        public List<string> LocalStorageKeys() => settings.ShareMemoryKeys();

        public VgcApis.Models.Datas.SharelinkMetaData EncodeToShareLinkMetaData(string config)
        {
            if (vgcSlinkMgr.TryParseConfig(config, out var meta) && meta != null)
            {
                return meta;
            }
            return null;
        }

        public string EncodeToShareLink(string name, string config)
        {
            return vgcSlinkMgr.EncodeConfigToShareLink(name, config);
        }

        public string EncodeToV2cfgShareLink(string name, string config)
        {
            return vgcSlinkMgr.EncodeConfigToShareLink(
                name,
                config,
                VgcApis.Models.Datas.Enums.LinkTypes.v2cfg
            );
        }

        public LuaTable DecodeShareLink(string shareLink)
        {
            var kv = vgcSlinkMgr.DecodeShareLinkToConfig(shareLink);
            var r = new LuaTable { ["name"] = kv.name, ["config"] = kv.config };
            return r;
        }

        /// <summary>
        /// null: failed
        /// </summary>
        public string Base64Encode(string text) => VgcApis.Misc.Utils.Base64EncodeString(text);

        public string Basse64EncodeBytes(byte[] bytes) =>
            VgcApis.Misc.Utils.Base64EncodeBytes(bytes);

        public byte[] Base64DecodeToBytes(string b64Str) =>
            VgcApis.Misc.Utils.Base64DecodeToBytes(b64Str);

        /// <summary>
        /// null: failed
        /// </summary>
        public string Base64Decode(string b64Str) =>
            VgcApis.Misc.Utils.Base64DecodeToString(b64Str);

        public string GetLinkBody(string link) => VgcApis.Misc.Utils.GetLinkBody(link);

        public string PredefinedFunctions() => Resources.Files.Datas.LuaPredefinedFunctions;

        public string ScanQrcode() => vgcUtils.ScanQrcode();

        public string GetAppDir() => VgcApis.Misc.Utils.GetAppDir();

        public void Sleep(int ms) => VgcApis.Misc.Utils.Sleep(ms);

        public void Print(params object[] contents)
        {
            var text = "";
            foreach (var content in contents)
            {
                if (content == null)
                {
                    text += @"nil";
                }
                else
                {
                    text += content.ToString();
                }
            }
            GetParent().SendLog(text);
        }
        #endregion

        #region private methods
        List<Type> GetTypesFromRows(LuaTable rows)
        {
            if (rows == null)
            {
                return null;
            }

            var ts = new List<Type>();
            foreach (var row in rows.Values)
            {
                var cells = row.Value as LuaTable;
                foreach (var kv in cells.Values)
                {
                    ts.Add(kv.Value.GetType());
                }
                return ts;
            }
            return null;
        }

        DataTable LuaTableToDataTable(LuaTable columns, LuaTable rows)
        {
            var d = new DataTable();

            if (columns == null)
            {
                return d;
            }

            var ts = GetTypesFromRows(rows);

            var idx = 0;
            foreach (var kv in columns.Values)
            {
                var name = kv.Value.ToString();
                if (ts == null)
                {
                    d.Columns.Add(name);
                }
                else
                {
                    d.Columns.Add(name, ts[idx++]);
                }
            }

            if (rows == null)
            {
                return d;
            }

            foreach (var row in rows.Values)
            {
                var cells = row.Value as LuaTable;
                var items = cells?.Select(c => c.Value).ToArray();
                if (items != null)
                {
                    d.Rows.Add(items);
                }
            }
            return d;
        }

        string ShowDataGridDialog(string title, DataTable dataSource, int defColumn)
        {
            Views.WinForms.FormDataGrid creater(AutoResetEvent done) =>
                new Views.WinForms.FormDataGrid(done, title, dataSource, defColumn);
            return GetResult<Views.WinForms.FormDataGrid, string>(creater);
        }

        private static List<int> GetResultFromChoicesDialog(string title, string[] choices)
        {
            Views.WinForms.FormChoices creater(AutoResetEvent done) =>
                new Views.WinForms.FormChoices(done, title, choices);
            return GetResult<Views.WinForms.FormChoices, List<int>>(creater);
        }

        static TResult GetResult<TForm, TResult>(Func<AutoResetEvent, TForm> creater)
            where TForm : Form, Interfaces.IWinFormControl<TResult>
        {
            AutoResetEvent done = new AutoResetEvent(false);
            TResult r = default;
            TForm form = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                form = creater.Invoke(done);
                form.Show();
                form.Activate();
            });
            done.WaitOne();
            done.Dispose();
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = form.GetResult();
                form.Dispose();
            });
            return r;
        }

        private static int GetResultFromChoiceDialog(string title, string[] choices, int selected)
        {
            Views.WinForms.FormChoice creater(AutoResetEvent done) =>
                new Views.WinForms.FormChoice(done, title, choices, selected);

            return GetResult<Views.WinForms.FormChoice, int>(creater);
        }

        #endregion
    }
}
