using System;
using System.Collections.Generic;


namespace Luna.Misc
{
    public static class Utils
    {
        #region lua vm

        internal static bool DoString(Controllers.LuaCoreCtrl coreCtrl, string name, string script, bool isLoadClr)
        {
            if (coreCtrl == null)
            {
                return false;
            }
            name = string.IsNullOrEmpty(name) ? $"({Resources.Langs.I18N.Empty})" : name;
            coreCtrl.Abort();
            coreCtrl.name = name;
            coreCtrl.script = script;
            coreCtrl.isLoadClr = isLoadClr;
            coreCtrl.Start();
            return true;
        }

        internal static Controllers.LuaCoreCtrl CreateLuaCoreCtrl(
           Services.FormMgrSvc formMgr,
           Action<string> logger)
        {
            var luaApis = new Models.Apis.LuaApis(formMgr);
            luaApis.Prepare();
            luaApis.SetRedirectLogWorker(logger);

            var coreSettings = new Models.Data.LuaCoreSetting();
            var ctrl = new Controllers.LuaCoreCtrl(true);
            ctrl.Run(luaApis, coreSettings);
            return ctrl;
        }
        #endregion

        #region tools
        public static List<string> LuaTableToList(NLua.LuaTable table, bool includeKey)
        {
            var r = new List<string>();
            foreach (KeyValuePair<object, object> de in table)
            {
                var v = de.Value.ToString();
                if (includeKey)
                {
                    v = $"{de.Key}.{v}";
                }
                r.Add(v);
            }
            return r;
        }

        public static Dictionary<string, string> LuaTableToDictionary(NLua.LuaTable data)
        {
            var r = new Dictionary<string, string>();
            foreach (KeyValuePair<object, object> de in data)
            {
                r[de.Key.ToString()] = de.Value.ToString();
            }
            return r;
        }
        #endregion
    }
}
