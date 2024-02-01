namespace VgcApis.Models.Consts
{
    public static class Intervals
    {
        public static int AnalizeLuaScriptDelay = 3000;

        public static int GetStartCoreTokenInterval = 391;

        public static int LazySaveUserSettingsDelay = 20 * 60 * 1000;
        public static int LazySaveServerListIntreval = 5 * 60 * 1000;
        public static int LazySaveLunaSettingsInterval = 5 * 60 * 1000;

        public static int DefaultSpeedTestTimeout = 30 * 1000;
        public static int DefaultFetchTimeout = 60 * 1000;

        public static int NotifierMenuUpdateIntreval = 2000;

        public static int SiFormLogRefreshInterval = 500;
        public static int LuaPluginLogRefreshInterval = 500;

        public static int FormQrcodeMenuUpdateDelay = 1000;
    }
}
