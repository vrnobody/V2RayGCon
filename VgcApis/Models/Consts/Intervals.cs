namespace VgcApis.Models.Consts
{
    public static class Intervals
    {
        public static int AnalizeLuaScriptDelay = 3000;

        public static int GetStartCoreTokenInterval = 391;

        public static int LazyGcDelay = 5 * 60 * 1000; // 10 minutes
        public static int LazySaveUserSettingsDelay = 5 * 60 * 1000;
        public static int LazySaveServerListIntreval = 3 * 60 * 1000;
        public static int LazySaveStatisticsDatadelay = 5 * 60 * 1000;

        public static int LazySaveLunaSettingsInterval = 3 * 60 * 1000;

        public static int DefaultSpeedTestTimeout = 20 * 1000;
        public static int DefaultFetchTimeout = 60 * 1000;

        public static int NotifierMenuUpdateIntreval = 1000;

        public static int SiFormLogRefreshInterval = 500;
        public static int LuaPluginLogRefreshInterval = 500;

        public static int FormQrcodeMenuUpdateDelay = 1000;
    }
}
