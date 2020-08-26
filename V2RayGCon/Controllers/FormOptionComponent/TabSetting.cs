using System.Windows.Forms;

namespace V2RayGCon.Controllers.OptionComponent
{
    class TabSetting : OptionComponentController
    {
        Services.Settings setting;
        Services.Servers servers;

        private readonly ComboBox
            cboxLanguage = null,
            cboxPageSize = null,
            cboxRandomSelectServerLatency = null;

        private readonly CheckBox
            chkServAutoTrack = null,
            chkPortableMode = null,
            chkSetUseV4 = null,
            chkSetEnableStat = null,
            chkSetUpdateUseProxy = null,
            chkSetCheckVgcUpdateWhenAppStart = null,
            chkSetIsSupportSelfSignedCert = null;
        private readonly CheckBox chkSetCheckV2RayCoreUpdateWhenAppStart;
        private readonly Button btnBrowseDebugLogFile;
        private readonly TextBox tboxDebugLogFilePath;
        private readonly CheckBox chkIsEnableDebugLogFile;
        private readonly TextBox tboxMaxCoreNum = null;

        public TabSetting(
            ComboBox cboxLanguage,
            ComboBox cboxPageSize,
            CheckBox chkServAutoTrack,
            TextBox tboxMaxCoreNum,
            ComboBox cboxRandomSelectServerLatency,
            CheckBox chkPortableMode,
            CheckBox chkSetUseV4,
            CheckBox chkSetIsSupportSelfSignedCert,
            CheckBox chkSetEnableStat,
            CheckBox chkSetUpdateUseProxy,
            CheckBox chkSetCheckVgcUpdateWhenAppStart,
            CheckBox chkSetCheckV2rayCoreUpdateWhenAppStart,

            Button btnBrowseDebugLogFile,
            TextBox tboxDebugLogFilePath,
            CheckBox chkIsEnableDebugLogFile)
        {
            this.setting = Services.Settings.Instance;
            this.servers = Services.Servers.Instance;

            // Do not put these lines of code into InitElement.
            this.cboxLanguage = cboxLanguage;
            this.cboxPageSize = cboxPageSize;
            this.chkServAutoTrack = chkServAutoTrack;
            this.tboxMaxCoreNum = tboxMaxCoreNum;
            this.cboxRandomSelectServerLatency = cboxRandomSelectServerLatency;
            this.chkPortableMode = chkPortableMode;
            this.chkSetUseV4 = chkSetUseV4;
            this.chkSetIsSupportSelfSignedCert = chkSetIsSupportSelfSignedCert;
            this.chkSetEnableStat = chkSetEnableStat;

            this.chkSetCheckVgcUpdateWhenAppStart = chkSetCheckVgcUpdateWhenAppStart;
            this.chkSetCheckV2RayCoreUpdateWhenAppStart = chkSetCheckV2rayCoreUpdateWhenAppStart;

            this.btnBrowseDebugLogFile = btnBrowseDebugLogFile;
            this.tboxDebugLogFilePath = tboxDebugLogFilePath;
            this.chkIsEnableDebugLogFile = chkIsEnableDebugLogFile;
            this.chkSetUpdateUseProxy = chkSetUpdateUseProxy;

            InitElement();
            BindEvents();
        }

        private void InitElement()
        {
            tboxDebugLogFilePath.Text = setting.DebugLogFilePath;
            chkIsEnableDebugLogFile.Checked = setting.isEnableDebugLogFile;

            cboxRandomSelectServerLatency.Text = setting.QuickSwitchServerLantency.ToString();

            chkSetUpdateUseProxy.Checked = setting.isUpdateUseProxy;
            chkSetCheckVgcUpdateWhenAppStart.Checked = setting.isCheckVgcUpdateWhenAppStart;
            chkSetCheckV2RayCoreUpdateWhenAppStart.Checked = setting.isCheckV2RayCoreUpdateWhenAppStart;

            chkSetEnableStat.Checked = setting.isEnableStatistics;
            chkSetUseV4.Checked = setting.isUseV4;
            chkSetIsSupportSelfSignedCert.Checked = setting.isSupportSelfSignedCert;
            chkPortableMode.Checked = setting.isPortable;
            cboxLanguage.SelectedIndex = (int)setting.culture;
            cboxPageSize.Text = setting.serverPanelPageSize.ToString();
            tboxMaxCoreNum.Text = setting.maxConcurrentV2RayCoreNum.ToString();

            var tracker = setting.GetServerTrackerSetting();
            chkServAutoTrack.Checked = tracker.isTrackerOn;
        }

        #region public method
        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            var pageSize = VgcApis.Misc.Utils.Str2Int(cboxPageSize.Text);
            if (pageSize != setting.serverPanelPageSize)
            {
                setting.serverPanelPageSize = pageSize;
                Services.Servers.Instance.RequireFormMainReload();
            }

            setting.isEnableDebugLogFile = chkIsEnableDebugLogFile.Checked;
            setting.DebugLogFilePath = tboxDebugLogFilePath.Text;

            setting.maxConcurrentV2RayCoreNum = VgcApis.Misc.Utils.Str2Int(tboxMaxCoreNum.Text);

            var index = cboxLanguage.SelectedIndex;
            if (IsIndexValide(index) && ((int)setting.culture != index))
            {
                setting.culture = (Models.Datas.Enums.Cultures)index;
            }

            var keepTracking = chkServAutoTrack.Checked;
            var trackerSetting = setting.GetServerTrackerSetting();
            if (trackerSetting.isTrackerOn != keepTracking)
            {
                trackerSetting.isTrackerOn = keepTracking;
                setting.SaveServerTrackerSetting(trackerSetting);
                setting.isServerTrackerOn = keepTracking;
                servers.OnAutoTrackingOptionChanged();
            }

            setting.QuickSwitchServerLantency = VgcApis.Misc.Utils.Str2Int(cboxRandomSelectServerLatency.Text);
            setting.isUpdateUseProxy = chkSetUpdateUseProxy.Checked;
            setting.isCheckVgcUpdateWhenAppStart = chkSetCheckVgcUpdateWhenAppStart.Checked;
            setting.isCheckV2RayCoreUpdateWhenAppStart = chkSetCheckV2RayCoreUpdateWhenAppStart.Checked;

            setting.isSupportSelfSignedCert = chkSetIsSupportSelfSignedCert.Checked;
            setting.isPortable = chkPortableMode.Checked;
            setting.isUseV4 = chkSetUseV4.Checked;

            // Must enable v4 mode first.
            setting.isEnableStatistics = setting.isUseV4 && chkSetEnableStat.Checked;

            setting.SaveUserSettingsNow();
            return true;
        }

        public override bool IsOptionsChanged()
        {
            if (setting.isUseV4 != chkSetUseV4.Checked
                || setting.isEnableDebugLogFile != chkIsEnableDebugLogFile.Checked
                || setting.DebugLogFilePath != tboxDebugLogFilePath.Text
                || setting.isSupportSelfSignedCert != chkSetIsSupportSelfSignedCert.Checked
                || setting.QuickSwitchServerLantency != VgcApis.Misc.Utils.Str2Int(cboxRandomSelectServerLatency.Text)
                || setting.isUpdateUseProxy != chkSetUpdateUseProxy.Checked
                || setting.isCheckVgcUpdateWhenAppStart != chkSetCheckVgcUpdateWhenAppStart.Checked
                || setting.isCheckV2RayCoreUpdateWhenAppStart != chkSetCheckV2RayCoreUpdateWhenAppStart.Checked
                || setting.isEnableStatistics != chkSetEnableStat.Checked
                || VgcApis.Misc.Utils.Str2Int(tboxMaxCoreNum.Text) != setting.maxConcurrentV2RayCoreNum
                || setting.isPortable != chkPortableMode.Checked
                || VgcApis.Misc.Utils.Str2Int(cboxPageSize.Text) != setting.serverPanelPageSize)
            {
                return true;
            }

            var index = cboxLanguage.SelectedIndex;
            if (IsIndexValide(index) && ((int)setting.culture != index))
            {
                return true;
            }

            var tracker = setting.GetServerTrackerSetting();
            if (tracker.isTrackerOn != chkServAutoTrack.Checked)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region private method
        void BindEvents()
        {
            btnBrowseDebugLogFile.Click += (s, a) =>
            {
                var path = VgcApis.Misc.UI.ShowSelectFileDialog(VgcApis.Models.Consts.Files.TxtExt);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    tboxDebugLogFilePath.Text = path;
                }
            };
        }


        bool IsIndexValide(int index)
        {
            if (index < 0 || index > 2)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
