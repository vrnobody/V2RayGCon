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
