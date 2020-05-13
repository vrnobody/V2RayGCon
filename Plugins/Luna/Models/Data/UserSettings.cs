using System.Collections.Generic;

namespace Luna.Models.Data
{
    class UserSettings
    {
        public Dictionary<string, string> luaShareMemory;
        public List<LuaCoreSetting> luaServers;

        public bool isLoadClr;
        public bool isEnableCodeAnalyze;

        public UserSettings()
        {
            isLoadClr = false;
            isEnableCodeAnalyze = false;
            NormalizeData();
        }

        public void NormalizeData()
        {
            if (luaShareMemory == null)
            {
                luaShareMemory = new Dictionary<string, string>();
            }

            if (luaServers == null)
            {
                luaServers = new List<LuaCoreSetting>();
            }
        }
    }
}
