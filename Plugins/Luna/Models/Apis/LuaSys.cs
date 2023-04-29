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

    internal class LuaSys :
        VgcApis.BaseClasses.Disposable,
        VgcApis.Interfaces.Lua.ILuaSys
    {
        readonly object procLocker = new object();

        private readonly LuaApis luaApis;
        private readonly Func<List<Type>> getAllAssemblies;

        List<Process> processes = new List<Process>();

        List<VgcApis.Interfaces.Lua.ILuaMailBox>
            mailboxs = new List<VgcApis.Interfaces.Lua.ILuaMailBox>();

        ConcurrentDictionary<string, VgcApis.Interfaces.Lua.ILuaMailBox>
            hotkeys = new ConcurrentDictionary<string, VgcApis.Interfaces.Lua.ILuaMailBox>();

        ConcurrentDictionary<string, CoreEvHook> coreEvHooks = new ConcurrentDictionary<string, CoreEvHook>();
        ConcurrentDictionary<string, GlobalEvHook> globalEvHooks = new ConcurrentDictionary<string, GlobalEvHook>();

        VgcApis.Interfaces.Services.IServersService vgcServerService;

        SysCmpos.PostOffice postOffice;

        public LuaSys(
            LuaApis luaApis,
            Func<List<Type>> getAllAssemblies)
        {
            this.luaApis = luaApis;
            this.getAllAssemblies = getAllAssemblies;
            this.postOffice = luaApis.GetPostOffice();
            this.vgcServerService = luaApis.GetVgcServerService();
        }

        #region ILuaSys.Net
        List<SysCmpos.HttpServer> httpServs = new List<SysCmpos.HttpServer>();

        public VgcApis.Interfaces.Lua.IRunnable CreateHttpServer(
            string url,
            VgcApis.Interfaces.Lua.ILuaMailBox inbox,
            VgcApis.Interfaces.Lua.ILuaMailBox outbox) =>
            CreateHttpServer(url, inbox, outbox, null);

        public VgcApis.Interfaces.Lua.IRunnable CreateHttpServer(
            string url,
            VgcApis.Interfaces.Lua.ILuaMailBox inbox,
            VgcApis.Interfaces.Lua.ILuaMailBox outbox,
            string source)
        {
            try
            {
                var serv = new SysCmpos.HttpServer(url, inbox, outbox, source);
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
                inputEncoding, outputEncoding);

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
                null, null);

        public Process Run(string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput,
            Encoding inputEncoding, Encoding outputEncoding) =>
            RunProcWrapper(true, exePath, args, stdin,
                envs, hasWindow, redirectOutput,
                inputEncoding, outputEncoding);

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
        DataReceivedEventHandler CreateSendLogHandler(Encoding encoding)
        {
            var ec = encoding;

            return (s, a) =>
            {
                try
                {
                    var d = a.Data;
                    if (!string.IsNullOrEmpty(d))
                    {
                        var bin = Encoding.Default.GetBytes(d);
                        var msg = ec.GetString(bin);
                        luaApis?.SendLog(msg);
                    }
                }
                catch { }
            };
        }

        void SendLogHandler(object sender, DataReceivedEventArgs args)
        {
            try
            {
                string msg = null;
                msg = args.Data; if (!string.IsNullOrEmpty(msg))
                {
                    luaApis?.SendLog(msg);
                }
            }
            catch { }
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
           Encoding inputEncoding, Encoding outputEncoding)
        {
            try
            {
                return RunProcWorker(
                    isTracking, exePath, args, stdin,
                    envs, hasWindow, redirectOutput,
                    inputEncoding, outputEncoding);
            }
            catch { }
            return null;
        }

        Process RunProcWorker(
            bool isTracking, string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput,
            Encoding inputEncoding, Encoding outputEncoding)
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

            DataReceivedEventHandler logHandler = SendLogHandler;
            if (outputEncoding != null)
            {
                logHandler = CreateSendLogHandler(outputEncoding);
            }

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
            KillAllProcesses();
        }

        #endregion
    }
}
