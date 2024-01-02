using System.Drawing;

namespace VgcApis.Models.Consts
{
    public static class AutoEllipsis
    {
        public static string ellipsis = @"…";
        public static Font defFont = SystemFonts.DefaultFont;

        public static int ServerShortNameMaxLength = 24;
        public static int ServerLongNameMaxLength = 60;
        public static int ServerSummaryMaxLength = 80;
        public static int ServerTitleMaxLength = 60;

        public static int V2rayCoreTitleMaxLength = 48;

        public static int QrcodeTextMaxLength = 128;

        public static int NotifierSysProxyInfoMaxLength = 50;
        public static int NotifierTextMaxLength = 120;

        public static int MarkLabelTextMaxLength = 40;
    }
}
