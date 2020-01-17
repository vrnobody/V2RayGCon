using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabPacCustomList : ComponentCtrl
    {
        Services.PsSettings setting;

        string[] oldCustomPacList;
        RichTextBox rtboxWhiteList, rtboxBlackList;
        Button btnSetSortWhitelist, btnSetSortBlacklist;

        public TabPacCustomList(
            Services.PsSettings setting,

            RichTextBox rtboxWhiteList,
            RichTextBox rtboxBlackList,

                Button btnSetSortWhitelist,
            Button btnSetSortBlacklist)
        {
            this.setting = setting;

            // oldCustomPacList != customPacList
            oldCustomPacList = setting.GetCustomPacSetting();

            this.rtboxBlackList = rtboxBlackList;
            this.rtboxWhiteList = rtboxWhiteList;
            this.btnSetSortWhitelist = btnSetSortWhitelist;
            this.btnSetSortBlacklist = btnSetSortBlacklist;

            InitControls();
            BindEvents();
        }

        #region private methods
        List<string> Text2List(string text) =>
            text.Replace("\r", "").Split('\n').ToList();

        void BindEvents()
        {
            btnSetSortWhitelist.Click += (s, a) =>
            {
                var sorted = VgcApis.Libs.Utils.SortPacList(Text2List(rtboxWhiteList.Text));
                rtboxWhiteList.Text = string.Join(Environment.NewLine, sorted);
            };

            btnSetSortBlacklist.Click += (s, a) =>
            {
                var sorted = VgcApis.Libs.Utils.SortPacList(Text2List(rtboxBlackList.Text));
                rtboxBlackList.Text = string.Join(Environment.NewLine, sorted);
            };
        }

        private void InitControls()
        {
            rtboxWhiteList.Text = oldCustomPacList[0];
            rtboxBlackList.Text = oldCustomPacList[1];
        }
        #endregion

        #region public method
        public void Reload()
        {
            oldCustomPacList = setting.GetCustomPacSetting();
            VgcApis.Libs.UI.RunInUiThread(rtboxWhiteList, () =>
            {
                InitControls();
            });
        }

        public override void Cleanup()
        {
            // do nothing
        }

        public override bool IsOptionsChanged()
        {
            if (oldCustomPacList[0] != rtboxWhiteList.Text
                || oldCustomPacList[1] != rtboxBlackList.Text)
            {
                return true;
            }

            return false;
        }

        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            oldCustomPacList[0] = rtboxWhiteList.Text;
            oldCustomPacList[1] = rtboxBlackList.Text;

            setting.SaveCustomPacSetting(oldCustomPacList);
            return true;
        }
        #endregion
    }
}
