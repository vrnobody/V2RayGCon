using System.IO;

namespace V2RayGCon.Constants
{
    public static class Strings
    {
        public static string MainUserSettingsFilename { get; private set; } =
            GetUserSettingsFullFileName(false);

        public static string BackupUserSettingsFilename { get; private set; } =
            GetUserSettingsFullFileName(true);

        #region helper functions
        static string GetUserSettingsFullFileName(bool isBackup)
        {
            var appDir = VgcApis.Misc.Utils.GetAppDir();
            var fileName = isBackup
                ? Properties.Resources.PortableUserSettingsBackup
                : Properties.Resources.PortableUserSettingsFilename;
            var fullFileName = Path.Combine(appDir, fileName);
            return fullFileName;
        }
        #endregion
    }
}
