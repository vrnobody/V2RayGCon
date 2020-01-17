namespace VgcApis.Models.Consts
{
    static public class Intervals
    {
        public const int GetStartCoreTokenInterval = 213;
        public const int GetStopCoreTokenInterval = 67;

        // Service.Setting 
        public const int LazyGcDelay = 10 * 60 * 1000; // 10 minutes
        public const int LazySaveUserSettingsDelay = 30 * 1000;
        public const int LazySaveServerListIntreval = 30 * 1000;
        public const int LazySaveStatisticsDatadelay = 1000 * 60 * 5;

        public const int SpeedTestTimeout = 20 * 1000;
        public const int FetchDefaultTimeout = 30 * 1000;


        public const int NotifierTextUpdateIntreval = 1500;

        public const int SiFormLogRefreshInterval = 500;
        public const int LuaPluginLogRefreshInterval = 500;

        public const int FormConfigerMenuUpdateDelay = 1500;
        public const int FormQrcodeMenuUpdateDelay = 200;
    }
}
