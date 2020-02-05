using System.Collections.Generic;

namespace VgcApis.Models.Consts
{
    static public class Core
    {
        public static long SpeedtestAbort = -1;
        public static long SpeedtestTimeout = long.MaxValue;

        public const int GetStatisticsTimeout = 2 * 1000;
        public const int WaitUntilReadyTimeout = 5 * 1000;
        public const int SendCtrlCTimeout = 5 * 1000;
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
