using System.Diagnostics;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaSys
    {
        #region process
        void Cleanup(Process proc);
        bool CloseMainWindow(Process proc);
        bool HasExited(Process proc);
        Process Run(string exePath, string args, bool hasWindow, string stdin, NLua.LuaTable envs);
        bool Stop(Process proc);
        void WaitForExit(Process proc);
        #endregion

        #region system
        string GetOsVersion();

        string GetOsReleaseInfo();

        int SetWallpaper(string filename);

        #endregion

        #region file system
        string GetImageResolution(string filename);

        string PickRandomLine(string filename);

        bool CreateFolder(string path);

        bool IsFileExists(string path);
        bool IsDirExists(string path);
        #endregion
    }
}
