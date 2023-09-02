using Neo.IronLua;
using NeoLuna.Resources.Langs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NeoLuna.Controllers
{
    internal class LuaCoreCtrl : VgcApis.BaseClasses.Disposable
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

        public void Run(Models.Apis.LuaApis luaApis, Models.Data.LuaCoreSetting luaCoreState)
        {
            this.settings = luaApis.formMgr.settings;
            this.coreSetting = luaCoreState;
            this.luaApis = luaApis;
            this.luaSignal = new Models.Apis.LuaSignal(settings);

            isRunning = false;
        }

        #region properties
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
        public Models.Data.LuaCoreSetting GetCoreSettings() => coreSetting;

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

            try
            {
                using (Lua core = new Lua())
                {
                    var g = CreateLuaCore(core, luaSys);

                    var sb = new StringBuilder();
                    sb.AppendLine(Resources.Files.Datas.LuaPredefinedFunctions);
                    sb.AppendLine(script);
                    var src = sb.ToString();

                    var chunk = core.CompileChunk(
                        src,
                        $" [{this.name}] ",
                        new LuaCompileOptions()
                        {
                            ClrEnabled = isLoadClr,
                            DebugEngine = enableTracebackFeature
                                ? LuaStackTraceDebugger.Default
                                : null,
                        }
                    );

                    var results = g.DoChunk(chunk);
                    if (results != null && results.Count > 0)
                    {
                        result = results[0].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                var str = TraceException(e);
                ShowErrorMessageToUser(str);
            }

            luaSys?.Dispose();
            luaSys = null;
            isRunning = false;
        }

        private string TraceException(Exception e)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"script [{coreSetting.name}]");
            sb.AppendLine($"{e}");
            sb.AppendLine(e.GetType().Name + ": ");
            sb.AppendLine(e.Message);
            sb.AppendLine();

            var eData = LuaExceptionData.GetData(e);
            sb.AppendLine(eData.FormatStackTrace(0, true));
            sb.AppendLine();
            if (e.InnerException != null)
            {
                sb.AppendLine(">>> INNER EXCEPTION <<<");
                return TraceException(e.InnerException);
            }
            return sb.ToString();
        }

        private void ShowErrorMessageToUser(string ex)
        {
            SendLog(ex);

            if (!luaSignal.Stop() && isAutoRun)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Luna script: [{coreSetting.name}]");
                sb.AppendLine(ex);
                VgcApis.Misc.UI.MsgBoxAsync(sb.ToString());
            }
        }

        LuaGlobal CreateLuaCore(Lua state, Models.Apis.LuaSys luaSys)
        {
            var g = state.CreateEnvironment<LuaGlobal>();
            g["std"] = new LuaTable()
            {
                { "Signal", luaSignal },
                { "Sys", luaSys },
                { "Misc", luaApis.GetChild<Interfaces.ILuaMisc>() },
                { "Server", luaApis.GetChild<Interfaces.ILuaServer>() },
                { "Web", luaApis.GetChild<Interfaces.ILuaWeb>() },
            };
            Misc.Patches.FixTableStringMath(g);
            return g;
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
