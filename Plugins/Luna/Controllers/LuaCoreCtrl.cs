using Luna.Resources.Langs;
using NLua;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luna.Controllers
{
    public class LuaCoreCtrl
    {
        public EventHandler OnStateChange;

        Services.Settings settings;
        Models.Data.LuaCoreSetting coreSetting;
        Models.Apis.LuaApis luaApis;
        VgcApis.Models.BaseClasses.LuaSignal luaSignal;

        Thread luaCoreThread;
        Task luaCoreTask;

        readonly object coreStateLocker = new object();

        public LuaCoreCtrl() { }

        public void Run(
            Services.Settings settings,
            Models.Data.LuaCoreSetting luaCoreState,
            Models.Apis.LuaApis luaApis)
        {
            this.settings = settings;
            this.coreSetting = luaCoreState;
            this.luaApis = luaApis;
            this.luaSignal = new VgcApis.Models.BaseClasses.LuaSignal();
        }

        #region properties 
        public string name => coreSetting.name;

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
            }
        }

        bool _isRunning = false;
        public bool isRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value)
                {
                    return;
                }

                _isRunning = value;
                if (_isRunning == false)
                {
                    SendLog($"{coreSetting.name} {I18N.Stopped}");
                    luaCoreTask = null;
                    luaCoreThread = null;
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
        public string GetScript() => coreSetting.script;

        public void SetScriptName(string name)
        {
            coreSetting.name = name;
        }

        public void ReplaceScript(string script)
        {
            coreSetting.script = script;
            Save();
        }

        public void Stop()
        {
            lock (coreStateLocker)
            {
                if (!isRunning)
                {
                    return;
                }
            }

            SendLog($"{I18N.SendStopSignalTo} {coreSetting.name}");
            luaSignal.SetStopSignal(true);
        }

        public void Kill()
        {
            if (luaCoreTask == null)
            {
                return;
            }

            SendLog($"{I18N.Terminate} {coreSetting.name}");

            luaSignal.SetStopSignal(true);
            if (luaCoreTask.Wait(2000))
            {
                return;
            }

            try
            {
                luaCoreThread?.Abort();
            }
            catch { }
            isRunning = false;
        }

        public void Start()
        {
            lock (coreStateLocker)
            {
                if (isRunning)
                {
                    return;
                }
                isRunning = true;
            }

            SendLog($"{I18N.Start} {coreSetting.name}");
            luaCoreTask = VgcApis.Libs.Utils.RunInBackground(
                RunLuaScript);
        }

        public void Cleanup()
        {
            Kill();
        }
        #endregion

        #region private methods
        void SendLog(string content)
            => luaApis.SendLog(content);

        void RunLuaScript()
        {
            luaSignal.ResetAllSignals();
            luaCoreThread = Thread.CurrentThread;

            try
            {
                var core = CreateLuaCore();
                var script = coreSetting.script;

                core.DoString(script);
            }
            catch (Exception e)
            {
                SendLog($"[{coreSetting.name}] {e.ToString()}");
            }
            isRunning = false;
        }

        Lua CreateLuaCore()
        {
            var lua = new Lua();

            lua.State.Encoding = Encoding.UTF8;

            // bug: lua can access all public functions
            var misc = luaApis.GetComponent<VgcApis.Models.Interfaces.Lua.ILuaMisc>();

            lua["Signal"] = luaSignal;

            lua["Json"] = luaApis.GetComponent<VgcApis.Models.Interfaces.Lua.ILuaJson>();
            lua["Misc"] = misc;
            lua["Server"] = luaApis.GetComponent<VgcApis.Models.Interfaces.Lua.ILuaServer>();
            lua["Web"] = luaApis.GetComponent<VgcApis.Models.Interfaces.Lua.ILuaWeb>();

            lua.DoString(misc.PredefinedFunctions());
            return lua;
        }

        void Save() => settings.SaveUserSettingsNow();

        #endregion
    }
}
