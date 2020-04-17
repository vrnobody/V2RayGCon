using Luna.Resources.Langs;
using NLua;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luna.Controllers
{
    public class LuaCoreCtrl
    {
        public EventHandler OnStateChange, OnIsHiddenChanged;

        Services.Settings settings;
        Models.Data.LuaCoreSetting coreSetting;
        Models.Apis.LuaApis luaApis;
        Models.Apis.LuaSignal luaSignal;
        Models.Apis.LuaSys luaSys = null;

        Thread luaCoreThread;
        Task luaCoreTask;

        public LuaCoreCtrl() { }

        public void Run(
            Services.Settings settings,
            Models.Data.LuaCoreSetting luaCoreState,
            Models.Apis.LuaApis luaApis)
        {
            this.settings = settings;
            this.coreSetting = luaCoreState;
            this.luaApis = luaApis;
            this.luaSignal = new Models.Apis.LuaSignal();
        }

        #region properties 
        public string name => coreSetting.name;

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
                InvokeOnIsHiddenChangeIgnoreError();
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
                }
                InvokeOnStateChangeIgnoreError();
            }
        }

        void InvokeOnIsHiddenChangeIgnoreError()
        {
            try
            {
                OnIsHiddenChanged?.Invoke(null, null);
            }
            catch { }
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
            if (!isRunning)
            {
                return;
            }

            SendLog($"{I18N.SendStopSignalTo} {coreSetting.name}");
            luaSignal.SetStopSignal(true);
            luaSys?.CloseAllMailBox();
        }

        public void Kill()
        {
            if (!isRunning)
            {
                return;
            }

            Stop();
            if (luaCoreTask?.Wait(2000) == true)
            {
                return;
            }

            SendLog($"{I18N.Terminate} {coreSetting.name}");
            luaSys?.Dispose();
            try
            {
                luaCoreThread?.Abort();
            }
            catch { }
            isRunning = false;
        }

        public void Start()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;

            SendLog($"{I18N.Start} {coreSetting.name}");
            luaCoreTask = VgcApis.Misc.Utils.RunInBackground(() => RunLuaScript());
        }

        public void Cleanup()
        {
            Kill();
        }
        #endregion

        #region private methods
        List<Type> assemblies = null;
        readonly object assemblisLocker = new object();
        List<Type> GetAllAssemblies()
        {
            // cache until controller is destroyed
            lock (assemblisLocker)
            {
                if (assemblies == null)
                {
                    assemblies = VgcApis.Misc.Utils.GetAllAssembliesType();
                }
            }
            return assemblies;
        }

        void SendLog(string content)
            => luaApis.SendLog(content);

        void RunLuaScript()
        {
            luaSys?.Dispose();
            luaSys = new Models.Apis.LuaSys(luaApis, () => GetAllAssemblies());

            luaSignal.ResetAllSignals();
            luaCoreThread = Thread.CurrentThread;

            try
            {
                var core = CreateLuaCore(luaSys);
                var script = coreSetting.script;
                core.DoString(script);
            }
            catch (Exception e)
            {
                SendLog($"[{coreSetting.name}] {e}");
            }

            isRunning = false;
            luaSys?.Dispose();
        }

        Lua CreateLuaCore(Models.Apis.LuaSys luaSys)
        {
            var lua = new Lua();

            lua.LoadCLRPackage();

            lua.State.Encoding = Encoding.UTF8;

            // bug: lua can access all public functions
            var misc = luaApis.GetChild<VgcApis.Interfaces.Lua.ILuaMisc>();

            lua["Signal"] = luaSignal;
            lua["Sys"] = luaSys;

            lua["Json"] = luaApis.GetChild<VgcApis.Interfaces.Lua.ILuaJson>();
            lua["Misc"] = misc;
            lua["Server"] = luaApis.GetChild<VgcApis.Interfaces.Lua.ILuaServer>();
            lua["Web"] = luaApis.GetChild<VgcApis.Interfaces.Lua.ILuaWeb>();

            lua.DoString(misc.PredefinedFunctions());
            return lua;
        }

        void Save() => settings.SaveUserSettingsNow();

        #endregion
    }
}
