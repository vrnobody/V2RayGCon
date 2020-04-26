using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luna.Services
{
    public class LuaServer :
        VgcApis.BaseClasses.Disposable
    {
        public EventHandler
            OnRequireFlyPanelUpdate,
            OnRequireMenuUpdate;

        Settings settings;
        List<Controllers.LuaCoreCtrl> luaCoreCtrls = new List<Controllers.LuaCoreCtrl>();
        Models.Apis.LuaApis luaApis;

        public LuaServer() { }

        public void Run(
           Settings settings,
           VgcApis.Interfaces.Services.IApiService api)
        {
            this.settings = settings;
            this.luaApis = new Models.Apis.LuaApis(settings, api);
            this.luaApis.Prepare();

            InitLuaCores();
        }

        #region public methods
        public void WakeUpAutoRunScripts(TimeSpan delay)
        {
            var list = GetAllLuaCoreCtrls().Where(c => c.isAutoRun).ToList();
            if (list.Count() <= 0)
            {
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                Task.Delay(1000).Wait();
                foreach (var core in list)
                {
                    core.Start();
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                }
            });
        }

        public List<string[]> GetAllScripts()
        {
            var scripts = new List<string[]>();
            var ctrls = GetAllLuaCoreCtrls();
            foreach (var luaCore in ctrls)
            {
                scripts.Add(new string[] {
                    luaCore.name,
                    luaCore.GetScript(),
                });
            }
            return scripts;
        }

        public void ResetIndex()
        {
            var controls = GetAllLuaCoreCtrls();
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].index = i;
            }
            InvokeOnRequireMenuUpdate();
        }

        public List<Controllers.LuaCoreCtrl> GetAllLuaCoreCtrls()
        {
            var list = luaCoreCtrls.ToList();
            return list
                .OrderBy(c => c.index)
                .ToList();
        }

        public List<Controllers.LuaCoreCtrl> GetVisibleCoreCtrls()
        {
            return GetAllLuaCoreCtrls()
                .Where(c => !c.isHidden)
                .ToList();
        }

        public void RemoveAllScripts()
        {
            var coreCtrls = GetAllLuaCoreCtrls();
            foreach (var coreCtrl in coreCtrls)
            {
                RemoveCoreCtrl(coreCtrl);
            }
            Save();
            InvokeOnLuaCoreCtrlListChangeIgnoreError();
        }

        public bool RemoveScriptByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var coreCtrl = GetAllLuaCoreCtrls().FirstOrDefault(c => c.name == name);
            if (coreCtrl == null)
            {
                return false;
            }

            RemoveCoreCtrl(coreCtrl);
            Save();
            InvokeOnLuaCoreCtrlListChangeIgnoreError();
            return true;
        }

        void RemoveCoreCtrl(Controllers.LuaCoreCtrl coreCtrl)
        {
            var name = coreCtrl.name;
            coreCtrl.OnStateChange -= OnRequireMenuUpdateHandler;
            coreCtrl.Abort();
            luaCoreCtrls.Remove(coreCtrl);
            settings.GetLuaCoreSettings().RemoveAll(s => s.name == name);
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
            var coreCtrls = GetAllLuaCoreCtrls();
            foreach (var ctrl in coreCtrls)
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
            var coreCtrl = GetAllLuaCoreCtrls().FirstOrDefault(c => c.name == name);
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
            AddNewLuaCoreCtrl(coreState);
            return true;
        }

        void AddNewLuaCoreCtrl(Models.Data.LuaCoreSetting coreState)
        {
            var coreCtrl = new Controllers.LuaCoreCtrl(false);
            luaCoreCtrls.Add(coreCtrl);
            coreCtrl.Run(settings, coreState, luaApis);
            coreCtrl.OnStateChange += OnRequireMenuUpdateHandler;
        }


        void InvokeOnRequireMenuUpdate() =>
            OnRequireMenuUpdateHandler(this, EventArgs.Empty);

        void OnRequireMenuUpdateHandler(object sender, EventArgs args)
        {
            try
            {
                OnRequireMenuUpdate?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }


        void InvokeOnLuaCoreCtrlListChangeIgnoreError()
        {
            try
            {
                OnRequireFlyPanelUpdate?.Invoke(this, EventArgs.Empty);
            }
            catch { }
            InvokeOnRequireMenuUpdate();
        }

        void Save() => settings.SaveUserSettingsNow();



        void InitLuaCores()
        {
            var coreStates = settings.GetLuaCoreSettings();
            foreach (var coreState in coreStates)
            {
                AddNewLuaCoreCtrl(coreState);
            }
        }
        #endregion
    }
}
