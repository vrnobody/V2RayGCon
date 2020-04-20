﻿using System;
using System.Diagnostics;

namespace VgcApis.Interfaces.Lua
{
    public interface ILuaSys
    {
        #region keyboard hotkey
        string GetAllKeyNames();
        bool UnregisterHotKey(VgcApis.Interfaces.Lua.ILuaMailBox mailbox);
        ILuaMailBox RegisterHotKey(string keyName, bool hasAlt, bool hasCtrl, bool hasShift);
        #endregion

        #region reflection
        string GetPublicInfosOfType(Type type);
        string GetPublicInfosOfObject(object @object);
        string GetChildrenInfosOfNamespace(string @namespace);
        string GetPublicInfosOfAssembly(string @namespace, string asm);
        #endregion

        #region post office
        ILuaMailBox CreateMailBox(string name);
        #endregion

        #region process
        void DoEvents();

        void Cleanup(Process proc);
        bool CloseMainWindow(Process proc);
        bool HasExited(Process proc);

        void Kill(Process proc);

        Process Run(string exePath);

        Process Run(string exePath, string args);

        Process Run(string exePath, string args, string stdin);

        Process Run(string exePath, string args, string stdin,
            NLua.LuaTable envs, bool hasWindow, bool redirectOutput);
        bool SendStopSignal(Process proc);
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
