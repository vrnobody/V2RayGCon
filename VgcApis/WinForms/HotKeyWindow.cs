﻿using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VgcApis.WinForms
{

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class HotKeyWindow : NativeWindow, IDisposable
    {

        public delegate void OnMessageEventHandle(Message m);
        public event OnMessageEventHandle OnMessage;

        #region WinAPI
        const int WM_HOTKEY = 0x0312;


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        ConcurrentDictionary<long, Action> handlers = new ConcurrentDictionary<long, Action>();
        ConcurrentDictionary<string, Tuple<int, long>> contexts = new ConcurrentDictionary<string, Tuple<int, long>>();
        int currentEvCode = 0;

        public HotKeyWindow(Control control)
        {
            var cp = new CreateParams()
            {
                Parent = control.Handle,
            };

            CreateHandle(cp);
        }

        #region private methods


        #endregion

        #region public methods
        public string RegisterHotKey(
            Action hotKeyHandler,
            string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {

            var evCode = currentEvCode++;

            if (!Misc.Utils.TryParseKeyMesssage(keyName, hasAlt, hasCtrl, hasShift,
                out uint modifier, out uint key))
            {
                return null;
            }

            long hkMsg = (key << 16) | modifier;
            var handlerKeys = handlers.Keys;
            if (handlerKeys.Contains(hkMsg) || !RegisterHotKey(Handle, evCode, modifier, key))
            {
                return null;
            }

            for (int failsafe = 0; failsafe < 1000; failsafe++)
            {
                var hkHandle = Guid.NewGuid().ToString();
                var keys = contexts.Keys;
                if (!keys.Contains(hkHandle))
                {
                    var hkParma = new Tuple<int, long>(evCode, hkMsg);
                    contexts.TryAdd(hkHandle, hkParma);
                    handlers.TryAdd(hkMsg, hotKeyHandler);
                    return hkHandle;
                }
            }

            return null;
        }

        void RemoveAllHotKeys()
        {
            Libs.Sys.FileLogger.Info("HotKey window remove all hotkey begin");
            var handles = contexts.Keys;
            foreach (var handle in handles)
            {
                UnregisterHotKey(handle);
            }
            Libs.Sys.FileLogger.Info("HotKey window remove all hotkey end");
        }

        public bool UnregisterHotKey(string hotKeyHandle)
        {
            try
            {
                if (!string.IsNullOrEmpty(hotKeyHandle)
                    && contexts.TryRemove(hotKeyHandle, out var context))
                {
                    var evCode = context.Item1;
                    var keyMsg = context.Item2;
                    var ok = UnregisterHotKey(this.Handle, evCode);
                    handlers.TryRemove(keyMsg, out _);
                    return ok;
                }
            }
            catch { }
            return false;
        }

        bool HandleHotKey(uint keyCode)
        {
            long key = keyCode & 0xffffffff;
            try
            {
                if (handlers.TryGetValue(key, out var handler))
                {
                    Misc.Utils.RunInBackground(() =>
                    {
                        try
                        {
                            handler?.Invoke();
                        }
                        catch (Exception e)
                        {
                            Libs.Sys.FileLogger.Error($"Handle hotkey event error\n key code:{key} \n {e}");
                        }
                    });
                    return true;
                }
            }
            catch (Exception e)
            {
                Libs.Sys.FileLogger.Error($"Handle wnd event error\n key code:{key} \n {e}");
            }
            return false;
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // check if we got a hot key pressed.
            if (m.Msg == WM_HOTKEY
                && HandleHotKey((uint)m.LParam))
            {
                return;
            }

            if (OnMessage != null)
            {
                OnMessage.Invoke(m);
                return;
            }

            base.WndProc(ref m);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    base.ReleaseHandle();
                    RemoveAllHotKeys();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~MsgWindow()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
