namespace Pacman.Libs
{
    public static class UI
    {
        public static void MsgBox(string content)
            => VgcApis.Libs.UI.MsgBox(
                Properties.Resources.Name,
                content);

        public static void MsgBoxAsync(string content)
            => VgcApis.Libs.UI.MsgBoxAsync(
                Properties.Resources.Name,
                content);
    }
}
