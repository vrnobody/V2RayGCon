using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Resources.Langs;

namespace Commander.Misc
{
    internal class UI
    {
        public static void CopyToClipboardAndPrompt(string content)
        {
            var ok = VgcApis.Misc.Utils.CopyToClipboard(content);
            VgcApis.Misc.UI.MsgBox(ok ? I18N.CopySuccess : I18N.CopyFail);
        }
    }
}
