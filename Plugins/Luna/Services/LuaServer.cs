using System;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Services
{
    public class LuaServer :
        VgcApis.Models.BaseClasses.Disposable
    {
        public EventHandler OnLuaCoreCtrlListChange;

        Settings settings;
        List<Controllers.LuaCoreCtrl> luaCoreCtrls;
        Models.Apis.LuaApis luaApis;

        public LuaServer() { }

        public void Run(
           Settings settings,
           VgcApis.Models.IServices.IApiService api)
        {
            this.settings = settings;
            this.luaApis = new Models.Apis.LuaApis(settings, api);
            this.luaApis.Prepare();

            luaCoreCtrls = InitLuaCores(settings, luaApis);
            WakeUpAutoRunScripts();
        }

        #region public methods

        public List<string[]> GetAllScripts()
        {
            var scripts = new List<string[]>();
            foreach (var luaCore in luaCoreCtrls)
            {
                scripts.Add(new string[] {
                    luaCore.name,
                    luaCore.GetScript(),
                });
            }
            return scripts;
        }

        public List<Controllers.LuaCoreCtrl> GetAllLuaCoreCtrls()
        {
            var list = luaCoreCtrls ?? new List<Controllers.LuaCoreCtrl>();
            return list.OrderBy(c => c.name).ToList();
        }

        public void RemoveAllScripts()
        {
            foreach (var coreCtrl in luaCoreCtrls)
            {
                coreCtrl.Kill();
            }
            luaCoreCtrls.Clear();
            settings.GetLuaCoreSettings().Clear();
            Save();
            InvokeOnLuaCoreCtrlListChangeIgnoreError();
        }

        public bool RemoveScriptByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var coreCtrl = luaCoreCtrls.FirstOrDefault(c => c.name == name);
            if (coreCtrl == null)
            {
                return false;
            }

            coreCtrl.Kill();
            luaCoreCtrls.Remove(coreCtrl);

            settings.GetLuaCoreSettings().RemoveAll(s => s.name == name);
            Save();
            InvokeOnLuaCoreCtrlListChangeIgnoreError();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scripts">List(string[]{name, script})</param>
        public void ImportScripts(IEnumerable<string[]> scripts)
        {
            if (scripts == null || scripts.Count() <= 0)
            {
                return;
            }

            var isRequireRefresh = false;
            foreach (var script in scripts)
            {
                var result = AddOrReplaceScriptQuiet(script[0], script[1]);
                isRequireRefresh = isRequireRefresh || result;
            }

            if (isRequireRefresh)
            {
                Save();
                InvokeOnLuaCoreCtrlListChangeIgnoreError();
            }
        }

        public bool AddOrReplaceScript(string name, string script)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (AddOrReplaceScriptQuiet(name, script))
            {
                Save();
                InvokeOnLuaCoreCtrlListChangeIgnoreError();
            }
            return true;
        }

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            if (luaCoreCtrls == null)
            {
                return;
            }

            foreach (var ctrl in luaCoreCtrls)
            {
                ctrl.Cleanup();
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// true: require refresh false: not need refresh
        /// </summary>
        /// <param name="name"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        bool AddOrReplaceScriptQuiet(string name, string script)
        {
            var coreCtrl = luaCoreCtrls.FirstOrDefault(c => c.name == name);
            if (coreCtrl != null)
            {
                coreCtrl.ReplaceScript(script);
                return false;
            }

            var coreState = new Models.Data.LuaCoreSetting
            {
                name = name,
                script = script,
            };

            settings.GetLuaCoreSettings().Add(coreState);
            coreCtrl = new Controllers.LuaCoreCtrl();
            luaCoreCtrls.Add(coreCtrl);
            coreCtrl.Run(settings, coreState, luaApis);
            return true;
        }

        void InvokeOnLuaCoreCtrlListChangeIgnoreError()
        {
            try
            {
                OnLuaCoreCtrlListChange?.Invoke(null, null);
            }
            catch { }
        }

        void Save() => settings.SaveUserSettingsNow();

        void WakeUpAutoRunScripts()
        {
            var list = luaCoreCtrls.Where(c => c.isAutoRun).ToList();
            if (list.Count() <= 0)
            {
                return;
            }
            foreach (var core in list)
            {
                core.Start();
            }
        }

        List<Controllers.LuaCoreCtrl> InitLuaCores(
            Settings settings,
            Models.Apis.LuaApis luaApis)
        {
            var cores = new List<Controllers.LuaCoreCtrl>();
            foreach (var luaCoreState in settings.GetLuaCoreSettings())
            {
                var luaCtrl = new Controllers.LuaCoreCtrl();
                luaCtrl.Run(settings, luaCoreState, luaApis);
                cores.Add(luaCtrl);
            }
            return cores;
        }
        #endregion
    }
}
