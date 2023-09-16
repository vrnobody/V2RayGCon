using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.OptionComponent
{
    class TabImport : OptionComponentController
    {
        readonly FlowLayoutPanel flyPanel;
        readonly Button btnAdd;
        readonly Services.Settings setting;
        string oldOptions;

        public TabImport(FlowLayoutPanel flyPanel, Button btnAdd)
        {
            this.setting = Services.Settings.Instance;

            this.flyPanel = flyPanel;
            this.btnAdd = btnAdd;

            InitPanel();
            BindEvent();
        }

        #region public method
        public override bool SaveOptions()
        {
            string curOptions = GetCurOptions();

            if (curOptions != oldOptions)
            {
                setting.SaveGlobalImportItems(curOptions);
                oldOptions = curOptions;
                Services.Servers.Instance.RestartServersWithImportMark();
                return true;
            }
            return false;
        }

        public override bool IsOptionsChanged()
        {
            return GetCurOptions() != oldOptions;
        }

        public void Reload(string rawSetting)
        {
            setting.SaveGlobalImportItems(rawSetting);
            Misc.UI.ClearFlowLayoutPanel(this.flyPanel);
            InitPanel();
        }
        #endregion

        #region private method
        string GetCurOptions()
        {
            return JsonConvert.SerializeObject(CollectImportItems());
        }

        List<Models.Datas.ImportItem> CollectImportItems()
        {
            var itemList = new List<Models.Datas.ImportItem>();
            foreach (Views.UserControls.ImportUI item in this.flyPanel.Controls)
            {
                var v = item.GetValue();
                if (!string.IsNullOrEmpty(v.alias) || !string.IsNullOrEmpty(v.url))
                {
                    itemList.Add(v);
                }
            }
            return itemList;
        }

        void InitPanel()
        {
            var importUrlItemList = setting.GetGlobalImportItems();

            this.oldOptions = JsonConvert.SerializeObject(importUrlItemList);

            if (importUrlItemList.Count <= 0)
            {
                importUrlItemList.Add(new Models.Datas.ImportItem());
            }

            foreach (var item in importUrlItemList)
            {
                this.flyPanel.Controls.Add(
                    new Views.UserControls.ImportUI(item, UpdatePanelItemsIndex)
                );
            }

            UpdatePanelItemsIndex();
        }

        void BindEventBtnAddClick()
        {
            this.btnAdd.Click += (s, a) =>
            {
                var control = new Views.UserControls.ImportUI(
                    new Models.Datas.ImportItem(),
                    UpdatePanelItemsIndex
                );

                flyPanel.Controls.Add(control);
                flyPanel.ScrollControlIntoView(control);
                UpdatePanelItemsIndex();
            };
        }

        void BindEventFlyPanelDragDrop()
        {
            this.flyPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };

            this.flyPanel.DragDrop += (s, a) =>
            {
                // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag

                var curItem =
                    a.Data.GetData(typeof(Views.UserControls.ImportUI))
                    as Views.UserControls.ImportUI;

                var panel = s as FlowLayoutPanel;
                Point p = panel.PointToClient(new Point(a.X, a.Y));
                var destItem = panel.GetChildAtPoint(p);
                int destIndex = panel.Controls.GetChildIndex(destItem, false);
                panel.Controls.SetChildIndex(curItem, destIndex);
                panel.Invalidate();
            };
        }

        void BindEvent()
        {
            BindEventBtnAddClick();
            BindEventFlyPanelDragDrop();
        }

        void UpdatePanelItemsIndex()
        {
            var index = 1;
            foreach (Views.UserControls.ImportUI item in this.flyPanel.Controls)
            {
                item.SetIndex(index++);
            }
        }
        #endregion
    }
}
