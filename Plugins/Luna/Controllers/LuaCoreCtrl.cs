using Luna.Resources.Langs;
using Newtonsoft.Json;
using NLua;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Luna.Controllers
{
    internal class LuaCoreCtrl :
        VgcApis.BaseClasses.Disposable
    {
        public EventHandler OnStateChange;

        Services.Settings settings;
        Models.Data.LuaCoreSetting coreSetting;
        Models.Apis.LuaApis luaApis;
        Models.Apis.LuaSignal luaSignal;
        Models.Apis.LuaSys luaSys = null;

        string result = null;
        ManualResetEvent coreStopBar = new ManualResetEvent(true);

        Thread luaCoreThread;
        private readonly bool enableTracebackFeature;

        public LuaCoreCtrl(bool enableTracebackFeature)
        {
            this.enableTracebackFeature = enableTracebackFeature;
        }

        public void Run(
            Models.Apis.LuaApis luaApis,
            Models.Data.LuaCoreSetting luaCoreState)
        {
            this.settings = luaApis.formMgr.settings;
            this.coreSetting = luaCoreState;
            this.luaApis = luaApis;
            this.luaSignal = new Models.Apis.LuaSignal(settings);

            isRunning = false;
        }

        #region properties 
        public bool isWarnOnExit = false;

        public string name
        {
            get => coreSetting.name;
            set
            {
                if (coreSetting.name == value)
                {
                    return;
                }
                coreSetting.name = value;
                Save();
                InvokeOnStateChangeIgnoreError();
            }
        }

        public string script
        {
            get => coreSetting.script;
            set
            {
                coreSetting.script = value;
                Save();
            }
        }

        public double index
        {
            get => coreSetting.index;
            set
            {
                if (coreSetting.index == value)
                {
                    return;
                }
                coreSetting.index = value;
                Save();
            }
        }

        public bool isLoadClr
        {
            get => coreSetting.isLoadClr;
            set
            {
                if (value == coreSetting.isLoadClr)
                {
                    return;
                }
                coreSetting.isLoadClr = value;
                Save();
                InvokeOnStateChangeIgnoreError();
            }
        }

        public bool isHidden
        {
            get => coreSetting.isHidden;
            set
            {
                if (coreSetting.isHidden == value)
                {
                    return;
                }
                coreSetting.isHidden = value;
                Save();
                InvokeOnStateChangeIgnoreError();
            }
        }

        public bool isAutoRun
        {
            get => coreSetting.isAutorun;
            set
            {
                if (coreSetting.isAutorun == value)
                {
                    return;
                }

                coreSetting.isAutorun = value;
                Save();
                InvokeOnStateChangeIgnoreError();
            }
        }

        public bool isRunning
        {
            get => coreSetting.isRunning;
            private set
            {
                if (value)
                {
                    coreStopBar.Reset();
                }
                else
                {
                    coreStopBar.Set();
                }

                if (coreSetting.isRunning == value)
                {
                    return;
                }

                coreSetting.isRunning = value;
                if (value == false && !string.IsNullOrEmpty(name))
                {
                    SendLog($"{coreSetting.name} {I18N.Stopped}");
                }
                InvokeOnStateChangeIgnoreError();
            }
        }

        void InvokeOnStateChangeIgnoreError()
        {
            try
            {
                OnStateChange?.Invoke(null, null);
            }
            catch { }
        }
        #endregion

        #region public methods
        public Models.Data.LuaCoreSetting GetCoreSettings() =>
            coreSetting;

        public string GetResult() => result;

        public void Wait(int ms)
        {
            if (ms > 0)
            {
                coreStopBar.WaitOne(ms);
            }
            else
            {
                coreStopBar.WaitOne();
            }
        }

        public void Stop()
        {
            if (!isRunning)
            {
                return;
            }

            // disable warn is user click stop button
            isWarnOnExit = false;

            if (!string.IsNullOrEmpty(name))
            {
                SendLog($"{I18N.SendStopSignalTo} {coreSetting.name}");
            }
            luaSignal.SetStopSignal(true);
            luaSys?.OnSignalStop();
        }

        public void Abort() => KillCore(2000);

        public void AbortNow() => KillCore(1);

        public void Start()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;
            isWarnOnExit = false;

            if (!string.IsNullOrEmpty(name))
            {
                SendLog($"{I18N.Start} {coreSetting.name}");
            }

            luaCoreThread = new Thread(RunLuaScript)
            {
                IsBackground = true,
                Name = "LuaStateContainer",
            };

            luaCoreThread.Start();
        }
        #endregion

        #region private methods
        void KillCore(int timeout)
        {
            if (!isRunning)
            {
                return;
            }

            Stop();

            if (!luaCoreThread.Join(timeout))
            {
                SendLog($"{I18N.Terminate} {coreSetting.name}");
                try
                {
                    luaCoreThread.Abort();
                }
                catch { }
            }

            luaSys?.Dispose();
            luaSys = null;

            isRunning = false;
        }

        List<Type> assemblies = null;
        List<Type> GetAllAssemblies()
        {
            if (assemblies == null)
            {
                assemblies = VgcApis.Misc.Utils.GetAllAssembliesType();
            }
            return assemblies;
        }

        void SendLog(string content) => luaApis.SendLog(content);

        void RunLuaScript()
        {
            result = null;

            luaSys?.Dispose();
            luaSys = new Models.Apis.LuaSys(this, luaApis, GetAllAssemblies);

            luaSignal.ResetAllSignals();

            using (Lua core = CreateLuaCore(luaSys))
            {
                try
                {
                    var results = core.DoString(coreSetting.script);
                    if (results != null && results.Length > 0)
                    {
                        result = JsonConvert.SerializeObject(results);
                    }
                }
                catch (Exception e)
                {
                    var isTraceOn = core.UseTraceback;
                    var coreErr = core.GetDebugTraceback();
                    ShowErrorMessageToUser(isTraceOn, coreErr, e.ToString());
                }
            }

            luaSys?.Dispose();
            luaSys = null;

            isRunning = false;
        }

        private void ShowErrorMessageToUser(bool isTraceOn, string coreErr, string ex)
        {
            SendLog($"[{coreSetting.name}] {ex}");
            if (isTraceOn)
            {
                SendLog(coreErr);
            }

            if (isWarnOnExit)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Luna script: [{coreSetting.name}]");
                sb.AppendLine(ex);
                sb.AppendLine(coreErr);
                VgcApis.Misc.UI.MsgBoxAsync(sb.ToString());
            }
        }

        Lua CreateLuaCore(Models.Apis.LuaSys luaSys)
        {
            var lua = new Lua()
            {
                UseTraceback = enableTracebackFeature,
            };

            if (isLoadClr)
            {
                lua.LoadCLRPackage();
            }

            lua.State.Encoding = Encoding.UTF8;

            // bug: lua can access all public functions
            var misc = luaApis.GetChild<VgcApis.Interfaces.Lua.ILuaMisc>();

            lua["Signal"] = luaSignal;
            lua["Sys"] = luaSys;

            lua["Misc"] = misc;
            lua["Server"] = luaApis.GetChild<VgcApis.Interfaces.Lua.ILuaServer>();
            lua["Web"] = luaApis.GetChild<VgcApis.Interfaces.Lua.ILuaWeb>();

            lua.DoString(misc.PredefinedFunctions());
            return lua;
        }

        void Save() => settings.SaveUserSettingsLater();

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            if (isRunning)
            {
                AbortNow();
            }
        }
        #endregion
    }
}
