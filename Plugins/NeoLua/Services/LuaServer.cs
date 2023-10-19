using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoLuna.Services
{
    internal class LuaServer : VgcApis.BaseClasses.Disposable
    {
        public EventHandler OnRequireFlyPanelUpdate,
            OnRequireMenuUpdate;
        readonly Settings settings;
        readonly List<Controllers.LuaCoreCtrl> luaCoreCtrls = new List<Controllers.LuaCoreCtrl>();
        Models.Apis.LuaApis luaApis;

        readonly object rwLock = new object();

        public LuaServer(Settings settings)
        {
            this.settings = settings;
        }

        public void Run(FormMgrSvc formMgr)
        {
            this.luaApis = new Models.Apis.LuaApis(formMgr);
            this.luaApis.Prepare();

            InitLuaCores();
            Save();
        }

        #region public methods
        public void WakeUpAutoRunScripts(TimeSpan delay)
        {
            var list = GetAllLuaCoreCtrls().Where(c => c.isAutoRun).ToList();
            if (list.Count() <= 0)
            {
                return;
            }

            VgcApis.Misc.Utils.RunInBgSlim(() =>
            {
                VgcApis.Misc.Utils.Sleep(delay);
                foreach (var core in list)
                {
                    core.Start();
                    VgcApis.Misc.Utils.Sleep(1000);
                }
            });
        }

        public List<string[]> GetAllScripts()
        {
            var scripts = new List<string[]>();
            var ctrls = GetAllLuaCoreCtrls();
            foreach (var luaCore in ctrls)
            {
                var cs = luaCore.GetCoreSettings();
                scripts.Add(new string[] { cs.name, cs.script });
            }
            return scripts;
        }

        public void ResetIndex()
        {
            var controls = GetAllLuaCoreCtrls();
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].index = i + 1;
            }
            InvokeOnRequireMenuUpdate();
        }

        public int Count()
        {
            lock (rwLock)
            {
                return luaCoreCtrls.Count;
            }
        }

        public List<Controllers.LuaCoreCtrl> GetAllLuaCoreCtrls()
        {
            lock (rwLock)
            {
                return luaCoreCtrls.OrderBy(c => c.index).ToList();
            }
        }

        public List<Controllers.LuaCoreCtrl> GetVisibleCoreCtrls()
        {
            return GetAllLuaCoreCtrls().Where(c => !c.isHidden).ToList();
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

        public void RefreshPanel() => InvokeOnLuaCoreCtrlListChangeIgnoreError();

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            var coreCtrls = GetAllLuaCoreCtrls();
            foreach (var ctrl in coreCtrls)
            {
                ctrl.Dispose();
            }
        }
        #endregion

        #region private methods
        void RemoveCoreCtrl(Controllers.LuaCoreCtrl coreCtrl)
        {
            var name = coreCtrl.name;
            coreCtrl.OnStateChange -= OnRequireMenuUpdateHandler;
            lock (rwLock)
            {
                luaCoreCtrls.Remove(coreCtrl);
            }
            settings.GetLuaCoreSettings().RemoveAll(s => s.name == name);

            coreCtrl.Dispose();
        }

        bool AddOrReplaceScriptQuiet(string name, string script)
        {
            var cores = GetAllLuaCoreCtrls();
            var coreCtrl = cores.FirstOrDefault(c => c.name == name);
            if (coreCtrl != null)
            {
                coreCtrl.script = script;
                return false;
            }

            var coreState = new Models.Data.LuaCoreSetting
            {
                index = cores.Count + 1,
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
            lock (rwLock)
            {
                luaCoreCtrls.Add(coreCtrl);
            }
            coreCtrl.Run(luaApis, coreState);
            coreCtrl.OnStateChange += OnRequireMenuUpdateHandler;
        }

        void InvokeOnRequireMenuUpdate() => OnRequireMenuUpdateHandler(this, EventArgs.Empty);

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

        void Save() => settings.SaveUserSettingsLater();

        void InitLuaCores()
        {
            var coreStates = settings.GetLuaCoreSettings();
            foreach (var coreState in coreStates)
            {
                coreState.isRunning = false;
                AddNewLuaCoreCtrl(coreState);
            }
            ResetIndex();
        }
        #endregion
    }
}
