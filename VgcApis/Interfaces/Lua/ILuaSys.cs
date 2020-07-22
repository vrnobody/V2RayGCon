using NLua;
using System;
using System.Diagnostics;
using System.Text;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaSys
    {
        #region Net
        IRunnable CreateHttpServer(string url, ILuaMailBox inbox, ILuaMailBox outbox);
        #endregion

        #region keyboard hotkey
        string GetAllKeyNames();
        bool UnregisterHotKey(ILuaMailBox mailbox, string handle);

        string RegisterHotKey(ILuaMailBox mailbox, int evCode,
            string keyName, bool hasAlt, bool hasCtrl, bool hasShift);
        #endregion

        #region reflection
        string GetPublicInfosOfType(Type type);
        string GetPublicInfosOfObject(object @object);
        string GetChildrenInfosOfNamespace(string @namespace);
        string GetPublicInfosOfAssembly(string @namespace, string asm);
        #endregion

        #region post office
        ILuaMailBox ApplyRandomMailBox();

        bool ValidateMailBox(ILuaMailBox mailbox);
        ILuaMailBox CreateMailBox(string name);

        bool RemoveMailBox(ILuaMailBox mailbox);
        #endregion

        #region encoding
        Encoding GetEncoding(int codepage);

        Encoding EncodingCmd936();
        Encoding EncodingUtf8();

        Encoding EncodingAscII();
        Encoding EncodingUnicode();
        #endregion

        #region process

        void DoEvents();

        void Cleanup(Process proc);
        bool CloseMainWindow(Process proc);
        bool HasExited(Process proc);

        void Kill(Process proc);

        Process RunAndForgot(string exePath);

        Process RunAndForgot(string exePath, string args);

        Process RunAndForgot(string exePath, string args, string stdin);

        Process RunAndForgot(string exePath, string args, string stdin,
            NLua.LuaTable envs, bool hasWindow, bool redirectOutput);

        Process RunAndForgot(string exePath, string args, string stdin,
            LuaTable envs, bool hasWindow, bool redirectOutput,
            Encoding inputEncoding, Encoding outputEncoding);

        Process Run(string exePath);

        Process Run(string exePath, string args);

        Process Run(string exePath, string args, string stdin);

        Process Run(string exePath, string args, string stdin,
            NLua.LuaTable envs, bool hasWindow, bool redirectOutput);

        Process Run(string exePath, string args, string stdin,
           LuaTable envs, bool hasWindow, bool redirectOutput,
           Encoding inputEncoding, Encoding outputEncoding);
        bool SendStopSignal(Process proc);
        void WaitForExit(Process proc);
        #endregion

        #region system
        void VolumeUp();

        void VolumeDown();

        void VolumeMute();

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
