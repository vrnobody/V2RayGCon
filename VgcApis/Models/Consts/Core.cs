using System.Collections.Generic;

namespace VgcApis.Models.Consts
{
    static public class Core
    {

        // new api v2ctl.exe api --server="127.0.0.1:3537" StatsService.QueryStats "reset: true"

        /* return values
stat: <
  name: "inbound>>>agentin>>>traffic>>>uplink"
>
stat: <
  name: "inbound>>>agentin>>>traffic>>>downlink"
>
stat: <
  name: "inbound>>>StatsApiInb>>>traffic>>>uplink"
  value: 222
>
stat: <
  name: "inbound>>>StatsApiInb>>>traffic>>>downlink"
  value: 325
>
         */

        // public static string StatsQueryParamTpl = "api --server=\"127.0.0.1:{0}\" StatsService.GetStats \"name: \"\"\"\"inbound>>>agentin>>>traffic>>>{1}\"\"\"\" reset: true\"";
        public static string StatsQueryParamTpl = "api --server=\"127.0.0.1:{0}\" StatsService.QueryStats \"reset: true\"";

        public static string V2RayCtlExeFileName = "v2ctl.exe";
        public static string V2RayCoreExeFileName = "v2ray.exe";

        public static string[] SourceUrls = new string[]
        {
            @"https://github.com/v2ray/v2ray-core/releases",
            @"https://github.com/v2fly/v2ray-core/releases",
        };

        public static int GetIndexBySourceUrl(string url)
        {
            for (int i = 0; i < SourceUrls.Length; i++)
            {
                if (SourceUrls[i] == url)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string GetSourceUrlByIndex(int index)
        {
            index = Misc.Utils.Clamp(index, 0, SourceUrls.Length);
            return SourceUrls[index];
        }

        public static string StdIn = @"stdin:";
        public static string ConfigArg = @"config";

        public static long SpeedtestAbort = -1;
        public static long SpeedtestTimeout = long.MaxValue;

        public const int GetStatisticsTimeout = 2 * 1000;
        public const int WaitUntilReadyTimeout = 5 * 1000;
        public const int SendCtrlCTimeout = 15 * 1000;
        public const int GetVersionTimeout = 2 * 1000;
        public const int KillCoreTimeout = 3 * 1000;

        public static List<string> ReadyLogMarks = new List<string>
        {
            "[Warning]",
            "started",
            "ore:",
            "V2Ray",
        };

    }
}
