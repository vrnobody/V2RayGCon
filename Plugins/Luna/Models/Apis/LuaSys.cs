﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLua;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Luna.Models.Apis
{
    enum CoreEvTypes
    {
        CoreStart = 1,
        CoreClosing = 2,
        CoreStop = 3,
        PropertyChanged = 4,
    }

    class GlobalEvHook
    {
        public CoreEvTypes evType { get; set; }

        public VgcApis.Interfaces.Lua.ILuaMailBox mailBox { get; set; }
        public EventHandler evHandler { get; set; }
        public GlobalEvHook(
            CoreEvTypes evType,
            VgcApis.Interfaces.Lua.ILuaMailBox mailBox,
            EventHandler evHandler)
        {
            this.evType = evType;
            this.mailBox = mailBox;
            this.evHandler = evHandler;
        }
    }

    class CoreEvHook
    {
        public CoreEvTypes evType { get; set; }
        public VgcApis.Interfaces.ICoreServCtrl coreServCtrl { get; set; }
        public VgcApis.Interfaces.Lua.ILuaMailBox mailBox { get; set; }
        public EventHandler evHandler { get; set; }
        public CoreEvHook(
            CoreEvTypes evType,
            VgcApis.Interfaces.ICoreServCtrl coreServCtrl,
            VgcApis.Interfaces.Lua.ILuaMailBox mailBox,
            EventHandler evHandler)
        {
            this.evType = evType;
            this.coreServCtrl = coreServCtrl;
            this.mailBox = mailBox;
            this.evHandler = evHandler;
        }
    }

    class LuaVm
    {
        public VgcApis.Libs.Sys.QueueLogger logger;
        public Controllers.LuaCoreCtrl coreCtrl;
        public string lastLogSend = "";
    }

    internal class LuaSys :
        VgcApis.BaseClasses.Disposable,
        VgcApis.Interfaces.Lua.ILuaSys
    {
        readonly object procLocker = new object();

        private readonly LuaApis luaApis;
        private readonly Func<List<Type>> getAllAssemblies;
        private readonly bool isLoadClr;
        List<Process> processes = new List<Process>();

        List<VgcApis.Interfaces.Lua.ILuaMailBox>
            mailboxs = new List<VgcApis.Interfaces.Lua.ILuaMailBox>();

        ConcurrentDictionary<string, VgcApis.Interfaces.Lua.ILuaMailBox>
            hotkeys = new ConcurrentDictionary<string, VgcApis.Interfaces.Lua.ILuaMailBox>();

        ConcurrentDictionary<string, CoreEvHook> coreEvHooks = new ConcurrentDictionary<string, CoreEvHook>();
        ConcurrentDictionary<string, GlobalEvHook> globalEvHooks = new ConcurrentDictionary<string, GlobalEvHook>();
        ConcurrentDictionary<string, LuaVm> luaVms = new ConcurrentDictionary<string, LuaVm>();

        VgcApis.Interfaces.Services.IServersService vgcServerService;

        SysCmpos.PostOffice postOffice;

        Services.LuaServer luaServer;
        Services.AstServer astServer;

        public LuaSys(
            LuaApis luaApis,
            Func<List<Type>> getAllAssemblies,
            bool isLoadClr)
        {
            this.luaApis = luaApis;
            this.getAllAssemblies = getAllAssemblies;
            this.isLoadClr = isLoadClr;
            this.postOffice = luaApis.GetPostOffice();
            this.vgcServerService = luaApis.GetVgcServerService();

            this.luaServer = luaApis.formMgr.luaServer;
            this.astServer = luaApis.formMgr.astServer;
        }

        #region ILuaSys.LuaVm
        private void BuildOneModuleSnippets(string varName, JObject ast, List<Dictionary<string, string>> snippets)
        {
            var fds = new Dictionary<string, string>() {
                { Services.AstServer.KEY_FUNCTION,"." },
                { Services.AstServer.KEY_METHODS ,":" },
            };

            snippets.Add(ToSnippet(varName, "keyword"));

            foreach (var kv in ast)
            {
                if (kv.Value is JArray && kv.Key == Services.AstServer.KEY_PROPERTY)
                {
                    foreach (string prop in kv.Value as JArray)
                    {
                        var snp = ToSnippet($"{varName}.{prop}", "snippet");
                        snippets.Add(snp);
                    }
                    continue;
                }

                if (kv.Value is JObject && fds.Keys.Contains(kv.Key))
                {
                    foreach (var skv in kv.Value as JObject)
                    {
                        var ps = string.Join(", ", skv.Value as JArray);
                        var sep = fds[kv.Key];
                        snippets.Add(ToSnippet($"{varName}{sep}{skv.Key}({ps})", "snippet"));
                    }
                }
            }
        }

        public string LuaGenModuleSnippets(string code)
        {
            var ast = astServer.AnalyzeCode(code);
            if (ast == null)
            {
                return null;
            }

            if (!(ast[Services.AstServer.KEY_MODULES] is JObject))
            {
                return null;
            }

            var snippets = new List<Dictionary<string, string>>();

            foreach (var kv in ast[Services.AstServer.KEY_MODULES] as JObject)
            {
                var mn = kv.Value.ToString();
                var mAst = astServer.AnalyzeModule(mn, false);
                if (mAst == null)
                {
                    continue;
                }

                BuildOneModuleSnippets(kv.Key, mAst, snippets);
            }
            return JsonConvert.SerializeObject(snippets);
        }

        Dictionary<string, string> ToSnippet(string caption, string meta)
        {
            /*
             {
                 caption: 'kvp', // 匹配关键词
                 value: 'for k, v in ipairs(t) do\n    print(k, v)\nend',   // 把匹配到的替换为这个
                 score: 100, // 越大越靠前
                 meta: "snippet" // 随便写
             }
             */

            return new Dictionary<string, string>
                {
                    {"caption", caption },
                    {"value", caption },
                    {"meta", meta },
                };
        }

        public string LuaGetStaticSnippets()
        {
            var snippets = astServer.GetRequireModuleNames()
                .Select(name => ToSnippet(name, "snippet"))
                .Concat(astServer.GetWebUiLuaStaticSnippets())
                .OrderBy(dict => dict["caption"])
                .ToList();

            return JsonConvert.SerializeObject(snippets);
        }

        Controllers.LuaCoreCtrl GetLuaCoreCtrlByName(string name)
        {
            return luaServer.GetAllLuaCoreCtrls()
                .Where(core => core.name == name)
                .FirstOrDefault();
        }

        public string LuaAnalyzeCode(string code)
        {
            var ast = astServer.AnalyzeCode(code);
            return ast?.ToString(Formatting.None);
        }

        public string LuaAnalyzeModule(string name)
        {
            var ast = astServer.AnalyzeModule(name, false);
            return ast?.ToString(Formatting.None);
        }

        public string LuaAnalyzeModuleEx(string name)
        {
            // this function is disabled for safety reason
            // var ast = astServer.AnalyzeModule(name, true);
            JObject ast = null;
            return ast?.ToString(Formatting.None);
        }

        public string LuaServGetAllCoreInfos()
        {
            var settings = luaServer.GetAllLuaCoreCtrls()
                .Select(ctrl =>
                {
                    var s = ctrl.GetCoreSettings();
                    return s;
                })
                .ToList();
            return JsonConvert.SerializeObject(settings) ?? @"[]";
        }

        public string LuaServGetCoreInfo(string name)
        {
            var core = GetLuaCoreCtrlByName(name);
            if (core != null)
            {
                var settings = core.GetCoreSettings();
                return JsonConvert.SerializeObject(settings);
            }
            return string.Empty;
        }

        public bool LuaServChangeSettings(string name, string settings)
        {
            var core = GetLuaCoreCtrlByName(name);
            var s = JsonConvert.DeserializeObject<Data.LuaCoreSetting>(settings);
            if (core != null && s != null)
            {
                core.isHidden = s.isHidden;
                core.isLoadClr = s.isLoadClr;
                core.isAutoRun = s.isAutorun;
                core.index = s.index;
                core.name = s.name;
                luaServer.ResetIndex();
                luaServer.RefreshPanel();
                return true;
            }
            return false;
        }

        public bool LusServSetIndex(string name, double index)
        {
            var core = GetLuaCoreCtrlByName(name);
            if (core != null)
            {
                core.index = index;
                luaServer.ResetIndex();
                luaServer.RefreshPanel();
                return true;
            }
            return false;
        }

        public bool LuaServRemove(string name)
        {
            return luaServer.RemoveScriptByName(name);
        }

        public void LuaServRestart(string name)
        {
            var core = GetLuaCoreCtrlByName(name);
            if (core == null)
            {
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                core.Abort();
                core.Start();
            });
        }
        public void LuaServStart(string name)
        {
            GetLuaCoreCtrlByName(name)?.Start();
        }

        public void LuaServAbort(string name)
        {
            GetLuaCoreCtrlByName(name)?.Abort();
        }

        public void LuaServStop(string name)
        {
            GetLuaCoreCtrlByName(name)?.Stop();
        }
        public bool LuaServAdd(string name, string script)
        {
            return luaServer.AddOrReplaceScript(name, script);
        }

        public string LuaServGetAllScripts()
        {
            var scripts = luaServer.GetAllScripts();
            var r = new Dictionary<string, string>();
            foreach (var s in scripts)
            {
                r[s[0]] = s[1];
            }
            return JsonConvert.SerializeObject(r);
        }

        public void LuaVmRemoveStopped()
        {
            var uids = luaVms.Keys.ToList();
            foreach (var uid in uids)
            {
                if (luaVms.TryGetValue(uid, out var vm))
                {
                    if (vm.coreCtrl == null || !vm.coreCtrl.isRunning)
                    {
                        LuaVmRemove(uid);
                    }
                }
            }
        }

        public string LuaVmGetScript(string luavm)
        {
            if (luaVms.TryGetValue(luavm, out var vm))
            {
                return vm.coreCtrl?.script ?? "";
            }
            return "";
        }

        public bool LuaVmRemove(string luavm)
        {
            if (luaVms.TryRemove(luavm, out var vm))
            {
                vm.coreCtrl?.Cleanup();
                return true;
            }
            return false;
        }

        public void LuaVmWait(string luavm) => LuaVmWait(luavm, 1000);

        public void LuaVmWait(string luavm, int delay)
        {
            Controllers.LuaCoreCtrl core = null;
            if (luaVms.TryGetValue(luavm, out var vm))
            {
                core = vm.coreCtrl;
            }

            if (core == null)
            {
                return;
            }
            while (core?.isRunning == true)
            {
                VgcApis.Misc.Utils.Sleep(delay);
            }
        }

        public string LuaVmCreate()
        {
            var vm = new LuaVm()
            {
                logger = new VgcApis.Libs.Sys.QueueLogger(),
            };

            // 开始无限套娃
            var formMgr = luaApis.formMgr;
            var core = Misc.Utils.CreateLuaCoreCtrl(formMgr, vm.logger.Log);
            if (core == null)
            {
                return null;
            }
            vm.coreCtrl = core;
            var luavm = Guid.NewGuid().ToString();
            if (!luaVms.TryAdd(luavm, vm))
            {
                return null;
            }
            return luavm;
        }

        public bool LuaVmRun(string luavm, string name, string script)
        {
            if (luaVms.TryGetValue(luavm, out var vm))
            {
                return Misc.Utils.DoString(vm.coreCtrl, name, script, isLoadClr);
            }
            return false;
        }

        public string LuaVmGetAllVmsInfo()
        {
            // uid: name, script, isRunning
            var infos = new Dictionary<string, Dictionary<string, object>>();
            var uids = luaVms.Keys.ToList();
            foreach (var uid in uids)
            {
                if (luaVms.TryGetValue(uid, out var vm))
                {
                    var ctrl = vm.coreCtrl;
                    infos[uid] = new Dictionary<string, object>() {
                        {"name", ctrl.name},
                        {"on", ctrl.isRunning },
                    };
                }
            }
            return JsonConvert.SerializeObject(infos);
        }

        public void LuaVmAbort(string luavm)
        {
            if (luaVms.TryGetValue(luavm, out var vm))
            {
                vm.coreCtrl?.Abort();
            }
        }

        public void LuaVmStop(string luavm)
        {
            if (luaVms.TryGetValue(luavm, out var vm))
            {
                vm.coreCtrl?.Stop();
            }
        }

        public bool LuaVmIsRunning(string luavm)
        {
            if (luaVms.TryGetValue(luavm, out var vm))
            {
                var isRunning = vm.coreCtrl?.isRunning;
                return isRunning == true;
            }
            return false;
        }

        public void LuaVmClearLog(string luavm)
        {
            if (luaVms.TryGetValue(luavm, out var vm))
            {
                vm.logger?.Clear();
            }
        }


        public string LuaVmGetLog(string luavm)
        {

            if (luaVms.TryGetValue(luavm, out var vm))
            {
                var log = VgcApis.Misc.Utils.TrimTrailingNewLine(vm.logger?.GetLogAsString(false));
                if (!vm.coreCtrl.isRunning && vm.lastLogSend == log)
                {
                    return null;
                }

                vm.lastLogSend = log;
                return string.IsNullOrEmpty(log) ? " " : log;
            }

            // return null or string.Empty will terminate WebUI log updater
            return null;
        }

        #endregion

        #region ILuaSys.Net
        List<SysCmpos.HttpServer> httpServs = new List<SysCmpos.HttpServer>();

        public VgcApis.Interfaces.Lua.IRunnable CreateHttpServer(
            string url,
            VgcApis.Interfaces.Lua.ILuaMailBox inbox,
            VgcApis.Interfaces.Lua.ILuaMailBox outbox) =>
            CreateHttpServer(url, inbox, outbox, null, false);

        public VgcApis.Interfaces.Lua.IRunnable CreateHttpServer(
            string url,
            VgcApis.Interfaces.Lua.ILuaMailBox inbox,
            VgcApis.Interfaces.Lua.ILuaMailBox outbox,
            string source,
            bool allowCORS)
        {
            try
            {
                var serv = new SysCmpos.HttpServer(url, inbox, outbox, source, allowCORS);
                lock (httpServs)
                {
                    httpServs.Add(serv);
                }
                return serv;
            }
            catch (Exception ex)
            {
                luaApis.SendLog(ex.ToString());
                throw;
            }
        }

        #endregion

        #region ILuaSys.EventHooks
        public int CoreEvStart { get; } = (int)CoreEvTypes.CoreStart;
        public int CoreEvClosing { get; } = (int)CoreEvTypes.CoreClosing;
        public int CoreEvStop { get; } = (int)CoreEvTypes.CoreStop;
        public int CoreEvPropertyChanged { get; } = (int)CoreEvTypes.PropertyChanged;

        public bool UnregisterCoreEvent(VgcApis.Interfaces.Lua.ILuaMailBox mailbox, string handle)
        {
            if (!postOffice.ValidateMailBox(mailbox))
            {
                return false;
            }

            if (coreEvHooks.TryRemove(handle, out var evhook))
            {
                try
                {
                    var coreServ = evhook.coreServCtrl;
                    var handler = evhook.evHandler;
                    switch (evhook.evType)
                    {
                        case CoreEvTypes.CoreStart:
                            coreServ.OnCoreStart -= handler;
                            break;
                        case CoreEvTypes.CoreStop:
                            coreServ.OnCoreStop -= handler;
                            break;
                        case CoreEvTypes.CoreClosing:
                            coreServ.OnCoreStop -= handler;
                            break;
                        case CoreEvTypes.PropertyChanged:
                            coreServ.OnPropertyChanged -= handler;
                            break;
                        default:
                            return false;
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        public bool UnregisterGlobalEvent(VgcApis.Interfaces.Lua.ILuaMailBox mailbox, string handle)
        {
            if (postOffice.ValidateMailBox(mailbox)
               && globalEvHooks.TryRemove(handle, out var evhook))
            {
                try
                {
                    var handler = evhook.evHandler;
                    switch (evhook.evType)
                    {
                        case CoreEvTypes.CoreStart:
                            vgcServerService.OnCoreStart -= handler;
                            break;
                        case CoreEvTypes.CoreStop:
                            vgcServerService.OnCoreStop -= handler;
                            break;
                        default:
                            return false;
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        public string RegisterGlobalEvent(
            VgcApis.Interfaces.Lua.ILuaMailBox mailbox,
            int evType, int evCode)
        {
            if (!postOffice.ValidateMailBox(mailbox))
            {
                return null;
            }

            try
            {
                var handle = Guid.NewGuid().ToString();
                var addr = mailbox.GetAddress();
                EventHandler handler = (s, a) =>
                {
                    var coreServ = s as VgcApis.Interfaces.ICoreServCtrl;
                    if (coreServ == null)
                    {
                        return;
                    }
                    var uid = coreServ.GetCoreStates().GetUid();
                    mailbox.SendCode(addr, evCode, uid);
                };
                switch ((CoreEvTypes)evType)
                {
                    case CoreEvTypes.CoreStart:
                        vgcServerService.OnCoreStart += handler;
                        break;
                    case CoreEvTypes.CoreStop:
                        vgcServerService.OnCoreStop += handler;
                        break;
                    default:
                        return null;
                }
                var item = new GlobalEvHook((CoreEvTypes)evType, mailbox, handler);
                globalEvHooks.TryAdd(handle, item);
                return handle;
            }
            catch { }
            return null;
        }

        public string RegisterCoreEvent(
            VgcApis.Interfaces.ICoreServCtrl coreServ,
            VgcApis.Interfaces.Lua.ILuaMailBox mailbox,
            int evType,
            int evCode)
        {
            // 无权访问
            if (!postOffice.ValidateMailBox(mailbox))
            {
                return null;
            }

            try
            {
                var handle = Guid.NewGuid().ToString();
                var addr = mailbox.GetAddress();
                EventHandler handler = (s, a) => mailbox.SendCode(addr, evCode);
                switch ((CoreEvTypes)evType)
                {
                    case CoreEvTypes.CoreStart:
                        coreServ.OnCoreStart += handler;
                        break;
                    case CoreEvTypes.CoreStop:
                        coreServ.OnCoreStop += handler;
                        break;
                    case CoreEvTypes.CoreClosing:
                        coreServ.OnCoreStop += handler;
                        break;
                    case CoreEvTypes.PropertyChanged:
                        coreServ.OnPropertyChanged += handler;
                        break;
                    default:
                        return null;
                }
                var item = new CoreEvHook((CoreEvTypes)evType, coreServ, mailbox, handler);
                coreEvHooks.TryAdd(handle, item);
                return handle;
            }
            catch { }
            return null;
        }

        #endregion

        #region ILuaSys.Hotkey
        public string GetAllKeyNames()
        {
            return string.Join(@", ", Enum.GetNames(typeof(Keys)));
        }

        public bool UnregisterHotKey(VgcApis.Interfaces.Lua.ILuaMailBox mailbox, string handle)
        {
            if (postOffice.ValidateMailBox(mailbox)
                && hotkeys.TryGetValue(handle, out var mb)
                && ReferenceEquals(mb, mailbox)
                && hotkeys.TryRemove(handle, out _))
            {
                return luaApis.UnregisterHotKey(handle);
            }
            return false;
        }

        public string RegisterHotKey(
            VgcApis.Interfaces.Lua.ILuaMailBox mailbox, int evCode,
            string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {
            // 无权访问
            if (!postOffice.ValidateMailBox(mailbox))
            {
                return null;
            }

            var addr = mailbox.GetAddress();
            Action handler = () => mailbox.SendCode(addr, evCode);

            var hkHandle = luaApis.RegisterHotKey(handler, keyName, hasAlt, hasCtrl, hasShift);
            if (!string.IsNullOrEmpty(hkHandle))
            {
                hotkeys.TryAdd(hkHandle, mailbox);
                return hkHandle;
            }

            return null;
        }
        #endregion

        #region ILuaSys.Reflection
        public string GetPublicInfosOfType(Type type)
        {
            var nl = Environment.NewLine;
            var evs = VgcApis.Misc.Utils.GetPublicEventsInfoOfType(type)
                .Select(infos => $"{infos.Item1} {infos.Item2}")
                .ToList();
            var props = VgcApis.Misc.Utils.GetPublicPropsInfoOfType(type)
                .Select(infos => $"{infos.Item1} {infos.Item2}")
                .ToList();

            var evi = string.Join(nl, evs);
            var propi = string.Join(nl, props);
            var pmi = VgcApis.Misc.Utils.GetPublicFieldsInfoOfType(type);
            var pfi = VgcApis.Misc.Utils.GetPublicMethodsInfoOfType(type);
            return $"{propi}{nl}{evi}{nl}{pmi}{nl}{pfi}";
        }

        public string GetPublicInfosOfObject(object @object)
        {
            return GetPublicInfosOfType(@object.GetType());
        }

        public string GetPublicInfosOfAssembly(string @namespace, string asm)
        {
            var types = getAllAssemblies();
            foreach (var type in types)
            {
                if (type.Namespace == @namespace && type.Name == asm)
                {
                    return GetPublicInfosOfType(type);
                }
            }
            return null;
        }

        public string GetChildrenInfosOfNamespace(string @namespace)
        {
            List<string> mbs = new List<string>();

            var asms = getAllAssemblies();
            foreach (var asm in asms)
            {
                if (asm.Namespace != @namespace || mbs.Contains(asm.Name))
                {
                    continue;
                }
                mbs.Add(asm.FullName);
            }
            return string.Join("\n", mbs);
        }


        #endregion

        #region ILuaSys.PostOffice
        public bool ValidateMailBox(VgcApis.Interfaces.Lua.ILuaMailBox mailbox) =>
            postOffice.ValidateMailBox(mailbox);

        public bool RemoveMailBox(VgcApis.Interfaces.Lua.ILuaMailBox mailbox)
        {
            if (!postOffice.ValidateMailBox(mailbox))
            {
                return false;
            }

            lock (procLocker)
            {
                if (mailboxs.Contains(mailbox))
                {
                    mailboxs.Remove(mailbox);
                }
            }

            return postOffice.RemoveMailBox(mailbox);
        }


        public VgcApis.Interfaces.Lua.ILuaMailBox ApplyRandomMailBox()
        {
            for (int failsafe = 0; failsafe < 10000; failsafe++)
            {
                var name = Guid.NewGuid().ToString();
                var mailbox = CreateMailBox(name);
                if (mailbox != null)
                {
                    return mailbox;
                }
            }

            // highly unlikely
            return null;
        }


        public VgcApis.Interfaces.Lua.ILuaMailBox CreateMailBox(string name)
        {
            var mailbox = postOffice.CreateMailBox(name);
            if (mailbox == null)
            {
                return null;
            }

            lock (procLocker)
            {
                if (!mailboxs.Contains(mailbox))
                {
                    mailboxs.Add(mailbox);
                    return mailbox;
                }
            }

            return null;
        }

        #endregion

        #region ILuaSys.Process
        public void DoEvents() => Application.DoEvents();

        public void WaitForExit(Process proc) => proc?.WaitForExit();

        public void Cleanup(Process proc) => proc?.Close();

        public void Kill(Process proc)
        {
            try
            {
                if (!HasExited(proc))
                {
                    proc.Kill();
                    // VgcApis.Misc.Utils.KillProcessAndChildrens(proc.Id);
                }
            }
            catch { }
        }

        public bool CloseMainWindow(Process proc)
        {
            if (proc == null)
            {
                return false;
            }
            return proc.CloseMainWindow();
        }

        public bool HasExited(Process proc)
        {
            if (proc == null)
            {
                return true;
            }

            try
            {
                return proc.HasExited;
            }
            catch { }
            return true;
        }

        public bool SendStopSignal(Process proc) => VgcApis.Misc.Utils.SendStopSignal(proc);

        public string Start(string param)
        {
            try
            {
                Process.Start(param);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return string.Empty;
        }

        public Process RunAndForgot(string exePath) =>
          RunAndForgot(exePath, null);

        public Process RunAndForgot(string exePath, string args) =>
            RunAndForgot(exePath, args, null);

        public Process RunAndForgot(string exePath, string args, string stdin) =>
            RunAndForgot(exePath, args, stdin, null, true, false);

        public Process RunAndForgot(string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput) =>
            RunAndForgot(exePath, args, stdin,
                envs, hasWindow, redirectOutput,
                null, null);

        public Process RunAndForgot(string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput,
            Encoding inputEncoding, Encoding outputEncoding) =>
            RunProcWrapper(false, exePath, args, stdin,
                envs, hasWindow, redirectOutput,
                inputEncoding, outputEncoding, null);

        public Process Run(string exePath) =>
            Run(exePath, null);

        public Process Run(string exePath, string args) =>
            Run(exePath, args, null);

        public Process Run(string exePath, string args, string stdin) =>
            Run(exePath, args, stdin, null, true, false);

        public Process Run(string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput) =>
            Run(exePath, args, stdin,
                envs, hasWindow, redirectOutput,
                null, null, null);

        public Process Run(string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput,
            Encoding inputEncoding, Encoding outputEncoding,
            VgcApis.Interfaces.ILogable logable) =>
            RunProcWrapper(true, exePath, args, stdin,
                envs, hasWindow, redirectOutput,
                inputEncoding, outputEncoding, logable);

        #endregion

        #region ILuaSys.Encoding
        public Encoding GetEncoding(int codepage) => Encoding.GetEncoding(codepage);

        public Encoding EncodingCmd936 { get; } = Encoding.GetEncoding(936);

        public Encoding EncodingUtf8 { get; } = Encoding.UTF8;

        public Encoding EncodingAscII { get; } = Encoding.ASCII;

        public Encoding EncodingUnicode { get; } = Encoding.Unicode;

        public Encoding EncodingDefault { get; } = Encoding.Default;

        #endregion

        #region ILuaSys.System
        public void SetTimeout(VgcApis.Interfaces.Lua.ILuaMailBox mailbox, int timeout, double id)
        {
            Task.Delay(timeout).ContinueWith((task) =>
            {
                var addr = mailbox?.GetAddress();
                if (!string.IsNullOrEmpty(addr))
                {
                    mailbox?.SendCode(addr, id);
                }
            });
        }

        public void GarbageCollect() => GC.Collect();
        public void VolumeUp() => Libs.Sys.VolumeChanger.VolumeUp();

        public void VolumeDown() => Libs.Sys.VolumeChanger.VolumeDown();

        public void VolumeMute() => Libs.Sys.VolumeChanger.Mute();

        static string osReleaseId;
        public string GetOsReleaseInfo()
        {
            if (string.IsNullOrEmpty(osReleaseId))
            {
                // https://stackoverflow.com/questions/39778525/how-to-get-windows-version-as-in-windows-10-version-1607/39778770
                var root = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                var name = Microsoft.Win32.Registry.GetValue(root, @"ProductName", @"")?.ToString();
                var arch = Environment.Is64BitOperatingSystem ? @"x64" : @"x86";
                var id = Microsoft.Win32.Registry.GetValue(root, @"ReleaseId", "")?.ToString();
                var build = Microsoft.Win32.Registry.GetValue(root, @"CurrentBuildNumber", @"")?.ToString();

                osReleaseId = $"{name} {arch} {id} build {build}";
            }
            return osReleaseId;
        }

        public string GetAppVersion()
        {
            var vgcUtils = luaApis.GetVgcUtilsService();
            return vgcUtils.GetAppVersion();
        }

        public string GetOsVersion() => Environment.OSVersion.VersionString;

        public int SetWallpaper(string filename) => Libs.Sys.WinApis.SetWallpaper(filename);

        public uint EmptyRecycle() => Libs.Sys.WinApis.EmptyRecycle();
        #endregion

        #region ILuaSys.File
        public string GetImageResolution(string filename) =>
           VgcApis.Misc.Utils.GetImageResolution(filename);

        public string PickRandomLine(string filename) =>
            VgcApis.Misc.Utils.PickRandomLine(filename);

        public bool IsFileExists(string path) => File.Exists(path);
        public bool IsDirExists(string path) => Directory.Exists(path);

        public bool CreateFolder(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch { }
            return false;
        }

        public string CombinePath(string root, string path)
        {
            return Path.Combine(root, path);
        }

        public string Ls(string path) => Ls(path, null);

        public string Ls(string path, string exts)
        {
            var spliters = new char[] { '\\', '/' };
            var folders = new List<string>();
            var files = new List<string>();

            if (string.IsNullOrEmpty(path))
            {
                folders = DriveInfo.GetDrives()
                    .Select(di => di.Name.Split(spliters).FirstOrDefault())
                    .ToList();
            }
            else if (IsDirExists(path))
            {

                try
                {
                    folders = Directory.GetDirectories(path)
                        .Select(f => f.Split(spliters).LastOrDefault())
                        .ToList();
                }
                catch { }
                try
                {
                    files = ListFiles(path, exts, spliters);
                }
                catch { }
            }

            var r = new Dictionary<string, List<string>>() {
                {"folders", folders },
                {"files", files },
            };
            return JsonConvert.SerializeObject(r);
        }

        List<string> ListFiles(string path, string exts, char[] spliters)
        {
            List<string> files;
            var extList = exts?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
              .Select(e => "." + e.Trim().ToLower())
              .Where(e => e.Length > 1)
              .ToList();

            if (extList != null && extList.Count > 0)
            {
                files = Directory.GetFiles(path)
                    .Where(f => !string.IsNullOrEmpty(
                        extList.FirstOrDefault(
                            ext => f.ToLower().EndsWith(ext))))
                    .Select(f => f.Split(spliters).LastOrDefault())
                    .ToList();
            }
            else
            {
                files = Directory.GetFiles(path)
                    .Select(f => f.Split(spliters).LastOrDefault())
                    .ToList();
            }

            return files;
        }
        #endregion

        #region public methods

        public void OnSignalStop()
        {
            RemoveAllGlobalEventHooks();
            RemoveAllCoreEventHooks();
            RemoveAllKeyboardHooks();
            CloseAllHttpServers();
            CloseAllMailBox();
        }

        #endregion

        #region private methods
        DataReceivedEventHandler CreateLogHandler(Encoding encoding, VgcApis.Interfaces.ILogable logable)
        {
            var ec = encoding;
            Func<string, string> decode = (s) => s;
            if (ec != null)
            {
                decode = (s) =>
                {
                    var bin = Encoding.Default.GetBytes(s);
                    return ec.GetString(bin);
                };
            }

            Action<string> log = luaApis.SendLog;
            if (logable != null)
            {
                log = logable.Log;
            }

            return (s, a) =>
            {
                try
                {
                    var msg = a.Data;
                    if (string.IsNullOrEmpty(msg))
                    {
                        return;
                    }
                    log(decode(msg));
                }
                catch { }
            };
        }

        void TrackdownProcess(Process proc)
        {
            lock (procLocker)
            {
                if (processes.Contains(proc))
                {
                    return;
                }
                processes.Add(proc);
            }
            VgcApis.Libs.Sys.ChildProcessTracker.AddProcess(proc);
        }

        Process RunProcWrapper(
            bool isTracking, string exePath, string args, string stdin,
           LuaTable envs, bool hasWindow, bool redirectOutput,
           Encoding inputEncoding, Encoding outputEncoding, VgcApis.Interfaces.ILogable logable)
        {
            try
            {
                return RunProcWorker(
                    isTracking, exePath, args, stdin,
                    envs, hasWindow, redirectOutput,
                    inputEncoding, outputEncoding, logable);
            }
            catch { }
            return null;
        }

        Process RunProcWorker(
            bool isTracking, string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput,
            Encoding inputEncoding, Encoding outputEncoding, VgcApis.Interfaces.ILogable logable)
        {
            var useStdIn = !string.IsNullOrEmpty(stdin);
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args,
                    CreateNoWindow = !hasWindow,
                    UseShellExecute = false,
                    RedirectStandardInput = useStdIn,
                    RedirectStandardError = redirectOutput,
                    RedirectStandardOutput = redirectOutput,
                }
            };

            DataReceivedEventHandler logHandler = CreateLogHandler(outputEncoding, logable);

            if (redirectOutput)
            {
                p.Exited += (s, a) =>
                {
                    p.ErrorDataReceived -= logHandler;
                    p.OutputDataReceived -= logHandler;
                };

                p.ErrorDataReceived += logHandler;
                p.OutputDataReceived += logHandler;
            }

            if (envs != null)
            {
                VgcApis.Misc.Utils.SetProcessEnvs(p, Misc.Utils.LuaTableToDictionary(envs));
            }

            p.Start();

            if (isTracking)
            {
                TrackdownProcess(p);
            }

            if (redirectOutput)
            {
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
            }

            if (useStdIn)
            {
                var ie = inputEncoding == null ? EncodingCmd936 : inputEncoding;
                var buff = ie.GetBytes(stdin);

                var input = p.StandardInput;
                input.BaseStream.Write(buff, 0, buff.Length);
                input.WriteLine();
                input.Close();
            }

            return p;
        }

        private void KillAllProcesses()
        {
            List<Process> ps;
            lock (procLocker)
            {
                ps = processes.ToList();
                processes.Clear();
            }
            foreach (var p in ps)
            {
                try
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                        // VgcApis.Misc.Utils.KillProcessAndChildrens(p.Id);
                    }
                }
                catch { }
            }
        }

        void CloseAllHttpServers()
        {
            List<SysCmpos.HttpServer> servs;
            lock (httpServs)
            {
                servs = httpServs.ToList();
                httpServs.Clear();
            }

            foreach (var s in servs)
            {
                s.Stop();
            }
        }

        void CloseAllMailBox()
        {
            List<VgcApis.Interfaces.Lua.ILuaMailBox> boxes;
            lock (procLocker)
            {
                boxes = mailboxs.ToList();
                mailboxs.Clear();
            }

            foreach (var box in boxes)
            {
                postOffice.RemoveMailBox(box);
            }
        }

        void RemoveAllGlobalEventHooks()
        {
            var handles = globalEvHooks.Keys;
            foreach (var handle in handles)
            {
                if (globalEvHooks.TryGetValue(handle, out var hook))
                {
                    UnregisterGlobalEvent(hook.mailBox, handle);
                }
            }
        }
        void RemoveAllCoreEventHooks()
        {
            var handles = coreEvHooks.Keys;
            foreach (var handle in handles)
            {
                // Do not remove handle here, UnregisterCoreEvent() will take care of it.
                if (coreEvHooks.TryGetValue(handle, out var hook))
                {
                    UnregisterCoreEvent(hook.mailBox, handle);
                }
            }
        }

        void RemoveAllKeyboardHooks()
        {
            var handles = hotkeys.Keys;
            foreach (var handle in handles)
            {
                if (hotkeys.TryRemove(handle, out _))
                {
                    luaApis.UnregisterHotKey(handle);
                }
            }
        }

        void RemoveAllLuaVms()
        {
            var handles = luaVms.Keys;
            foreach (var handle in handles)
            {
                if (luaVms.TryRemove(handle, out var luaVm))
                {
                    luaVm.coreCtrl?.Cleanup();
                }
            }
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            // 注意要在OnSignalStop中添加相关代码
            RemoveAllGlobalEventHooks();
            RemoveAllCoreEventHooks();
            RemoveAllKeyboardHooks();
            CloseAllHttpServers();
            CloseAllMailBox();
            RemoveAllLuaVms();
            KillAllProcesses();
        }

        #endregion
    }
}
