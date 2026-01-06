using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProxySetter.Resources.Langs;
using ProxySetter.Services;

namespace ProxySetter.Views.WinForms
{
    public partial class FormModifyPacList : Form
    {
        private readonly PsSettings setting;
        string[] oldCustomPacList;

        internal FormModifyPacList(PsSettings setting)
        {
            this.setting = setting;
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormModifyPacList_Load(object sender, EventArgs e)
        {
            oldCustomPacList = setting.GetCustomPacSetting();
            rtboxWhiteList.Text = oldCustomPacList[0];
            rtboxBlackList.Text = oldCustomPacList[1];
        }

        #region private methods
        List<string> Text2List(string text) => text.Replace("\r", "").Split('\n').ToList();

        bool IsOptionsChanged()
        {
            if (
                oldCustomPacList[0] != rtboxWhiteList.Text
                || oldCustomPacList[1] != rtboxBlackList.Text
            )
            {
                return true;
            }

            return false;
        }

        bool SaveOptions()
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

        #region UI event handlers
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSortWhiteList_Click(object sender, EventArgs e)
        {
            var sorted = VgcApis.Misc.Utils.SortPacList(Text2List(rtboxWhiteList.Text));
            rtboxWhiteList.Text = string.Join(Environment.NewLine, sorted);
        }

        private void btnSortBlackList_Click(object sender, EventArgs e)
        {
            var sorted = VgcApis.Misc.Utils.SortPacList(Text2List(rtboxBlackList.Text));
            rtboxBlackList.Text = string.Join(Environment.NewLine, sorted);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveOptions();
            Close();
        }

        #endregion
    }
}
