using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VgcApis.WinForms
{
    public class HotKeyWindow : NativeWindow
    {
        public delegate void MessageHandler(string keyMsg);
        public MessageHandler OnHotKeyMessage;

        #region WinAPI
        const int WM_HOTKEY = 0x0312;
        private readonly ContextMenuStrip niMenuRoot;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        public HotKeyWindow(ContextMenuStrip niMenuRoot)
        {
            this.niMenuRoot = niMenuRoot;
            var cp = new CreateParams()
            {
                Parent = niMenuRoot.Handle,
            };
            CreateHandle(cp);
        }

        #region public methods
        public bool RegisterHotKey(Models.Datas.HotKeyContext context) =>
            RegisterHotKey(Handle, context.evCode, context.modifier, context.key);

        public bool UnregisterHotKey(Models.Datas.HotKeyContext context) =>
            UnregisterHotKey(Handle, context.evCode);
        #endregion

        #region protected methods
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // check if we got a hot key pressed.
            if (m.Msg == WM_HOTKEY)
            {
                var keyMsg = ((uint)m.LParam).ToString();
                OnHotKeyMessage?.Invoke(keyMsg);
                return;
            }

            base.WndProc(ref m);
        }
        #endregion
    }
}
