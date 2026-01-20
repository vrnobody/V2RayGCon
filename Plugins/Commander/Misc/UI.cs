using System.Collections.Generic;
using System.Windows.Forms;
using Commander.Resources.Langs;

namespace Commander.Misc
{
    internal class UI
    {
        public static bool HasDropableCmdNameControl(DragEventArgs e)
        {
            var name = typeof(Views.CmdParamsUC).FullName;
            foreach (string ty in e.Data.GetFormats())
            {
                if (ty == name)
                {
                    return true;
                }
            }
            return false;
        }

        public static void CopyToClipboardAndPrompt(string content)
        {
            var ok = VgcApis.Misc.Utils.CopyToClipboard(content);
            VgcApis.Misc.UI.MsgBox(ok ? I18N.CopySuccess : I18N.CopyFail);
        }
    }
}
