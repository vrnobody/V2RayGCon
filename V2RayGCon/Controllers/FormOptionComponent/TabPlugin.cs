using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.OptionComponent
{
    class TabPlugin : OptionComponentController
    {
        readonly FlowLayoutPanel flyPanel;
        readonly Services.Settings setting;
        readonly Services.PluginsServer pluginServ;

        string oldOptions;
        List<Models.Datas.PluginInfoItem> curPluginInfos;

        public TabPlugin(
            Button btnRefreshPluginsPanel,
            CheckBox chkIsLoad3rdPartyPlugins,
            FlowLayoutPanel flyPanel
        )
        {
            setting = Services.Settings.Instance;
            pluginServ = Services.PluginsServer.Instance;

            this.flyPanel = flyPanel;

            curPluginInfos = setting.GetPluginInfoItems();
            MarkdownCurOption();

            InitControls(btnRefreshPluginsPanel, chkIsLoad3rdPartyPlugins);
        }

        #region public method
        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            curPluginInfos = CollectPluginInfoItems();
            MarkdownCurOption();
            setting.SavePluginInfoItems(curPluginInfos);
            pluginServ.RestartAllPlugins();
            Services.Notifier.Instance.RefreshNotifyIconLater();
            return true;
        }

        public override bool IsOptionsChanged()
        {
            return GetCurOptions() != oldOptions;
        }
        #endregion

        #region private method
        string GetCurOptions()
        {
            return JsonConvert.SerializeObject(CollectPluginInfoItems());
        }

        List<Models.Datas.PluginInfoItem> CollectPluginInfoItems()
        {
            var itemList = new List<Models.Datas.PluginInfoItem>();
            foreach (Views.UserControls.PluginInfoUI item in this.flyPanel.Controls)
            {
                itemList.Add(item.GetValue());
            }
            return itemList;
        }

        void MarkdownCurOption()
        {
            this.oldOptions = JsonConvert.SerializeObject(curPluginInfos);
        }

        void RemoveAllControls()
        {
            var controls = flyPanel.Controls;
            flyPanel.Controls.Clear();
            flyPanel.PerformLayout();
            foreach (Views.UserControls.PluginInfoUI control in controls)
            {
                control.Dispose();
            }
        }

        void InitControls(Button btnRefreshPluginsPanel, CheckBox chkIsLoad3rdPartyPlugins)
        {
            chkIsLoad3rdPartyPlugins.Checked = setting.isLoad3rdPartyPlugins;

            chkIsLoad3rdPartyPlugins.CheckedChanged += (s, a) =>
            {
                setting.isLoad3rdPartyPlugins = chkIsLoad3rdPartyPlugins.Checked;
            };

            btnRefreshPluginsPanel.Click += (s, a) =>
            {
                curPluginInfos = pluginServ.GatherAllPluginInfos();
                RefreshPluginsPanel();
            };

            RefreshPluginsPanel();
        }

        private void RefreshPluginsPanel()
        {
            RemoveAllControls();
            foreach (var item in curPluginInfos)
            {
                this.flyPanel.Controls.Add(new Views.UserControls.PluginInfoUI(item));
            }
        }
        #endregion
    }
}
