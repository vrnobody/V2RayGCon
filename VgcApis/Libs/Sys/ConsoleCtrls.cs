using System;
using System.Runtime.InteropServices;

namespace VgcApis.Libs.Sys
{
    public static class ConsoleCtrls
    {
        #region support ctrl+c
        // https://stackoverflow.com/questions/283128/how-do-i-send-ctrlc-to-a-process-in-c
        [DllImport("kernel32.dll")]
        public static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool FreeConsole();

        // Delegate type to be used as the Handler Routine for SCCH
        public delegate Boolean ConsoleCtrlDelegate(uint CtrlType);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCtrlHandler(
            ConsoleCtrlDelegate HandlerRoutine,
            bool Add
        );
        #endregion
    }
}
