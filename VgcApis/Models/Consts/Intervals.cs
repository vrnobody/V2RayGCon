namespace VgcApis.Models.Consts
{
    static public class Intervals
    {
        public const int GetStartCoreTokenInterval = 391;

        // Service.Setting 
        public const int LazyGcDelay = 10 * 60 * 1000; // 10 minutes
        public const int LazySaveUserSettingsDelay = 60 * 1000;
        public const int LazySaveServerListIntreval = 3 * 1000;
        public const int LazySaveStatisticsDatadelay = 1000 * 60 * 5;

        public const int DefaultSpeedTestTimeout = 20 * 1000;
        public const int DefaultFetchTimeout = 30 * 1000;


        public const int NotifierMenuUpdateIntreval = 600;

        public const int SiFormLogRefreshInterval = 500;
        public const int LuaPluginLogRefreshInterval = 500;

        public const int FormConfigerMenuUpdateDelay = 1500;
        public const int FormQrcodeMenuUpdateDelay = 1000;
    }
}
