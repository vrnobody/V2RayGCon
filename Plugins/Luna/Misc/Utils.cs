using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NLua;
using Moq;
using System.Text;

namespace Luna.Misc
{
    public static class Utils
    {
        #region todo: lua ast should be a service 
        internal enum AnalyzeModes
        {
            SourceCode,
            Module,
            ModuleEx
        }

        private static Lua CreateAnalyser()
        {
            Lua anz = new Lua()
            {
                UseTraceback = true,
            };

            anz.State.Encoding = Encoding.UTF8;

            // phony
            anz["Misc"] = new Mock<VgcApis.Interfaces.Lua.ILuaMisc>().Object;
            anz["Signal"] = new Mock<VgcApis.Interfaces.Lua.ILuaSignal>().Object;
            anz["Sys"] = new Mock<VgcApis.Interfaces.Lua.ILuaSys>().Object;
            anz["Server"] = new Mock<VgcApis.Interfaces.Lua.ILuaServer>().Object;
            anz["Web"] = new Mock<VgcApis.Interfaces.Lua.ILuaWeb>().Object;

            anz.DoString(Resources.Files.Datas.LuaPredefinedFunctions);

            return anz;
        }
        #endregion

        #region lua vm

        internal static JObject Analyze(string code, AnalyzeModes analyzeMode)
        {
            try
            {
                Lua state = CreateAnalyser();
                state["code"] = code;

                var fn = "analyzeCode";
                if (analyzeMode == AnalyzeModes.Module)
                {
                    fn = "analyzeModule";
                }
                else if (analyzeMode == AnalyzeModes.ModuleEx)
                {
                    fn = "analyzeModuleEx";
                }

                string tpl = @"local analyzer = require('lua.libs.luacheck.analyzer').new();"
                    + @"return analyzer.{0}(code)";

                var script = string.Format(tpl, fn);
                string r = state.DoString(script)[0] as string;

                return JObject.Parse(r);
            }
            catch { }
            return null;
        }

        internal static bool DoString(Controllers.LuaCoreCtrl coreCtrl, string name, string script, bool isLoadClr)
        {
            if (coreCtrl == null)
            {
                return false;
            }
            coreCtrl.Abort();
            coreCtrl.SetScriptName(string.IsNullOrEmpty(name) ? $"({Resources.Langs.I18N.Empty})" : name);
            coreCtrl.ReplaceScript(script);
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
