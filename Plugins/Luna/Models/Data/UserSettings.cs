using System.Collections.Generic;

namespace Luna.Models.Data
{
    class UserSettings
    {
        public Dictionary<string, string> luaShareMemory;
        public List<LuaCoreSetting> luaServers;

        public UserSettings()
        {
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
