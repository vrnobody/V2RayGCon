using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;
using V2RayGCon.Services;

namespace V2RayGCon.Controllers.OptionComponent
{
    public class TabCustomInboundSettings : OptionComponentController
    {
        private readonly Settings settings;
        private readonly Servers servers;
        private readonly FlowLayoutPanel flyPanel;
        private readonly Button btnAdd;
        List<CustomConfigTemplate> datas;

        public TabCustomInboundSettings(FlowLayoutPanel flyPanel, Button btnAdd)
        {
            this.settings = Settings.Instance;
            this.servers = Servers.Instance;

            this.flyPanel = flyPanel;
            this.btnAdd = btnAdd;

            BindButtonAdd();
            BindFlyPanelDragDropEvent();

            datas =
                VgcApis.Misc.Utils.Clone(settings.GetCustomConfigTemplates())?.ToList()
                ?? new List<CustomConfigTemplate>();

            InitPanel();
        }

        #region public methods
        public override void Cleanup() { }

        public override bool IsOptionsChanged()
        {
            var src = settings.GetCustomConfigTemplates();
            GatherTemplatesSetting();
            return !VgcApis.Misc.Utils.SerializableEqual(src, datas);
        }

        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }
            var requireUpdateSummary = settings.ReplaceCustomConfigTemplates(datas);
            if (requireUpdateSummary)
            {
                VgcApis.Misc.Utils.DoItLater(servers.UpdateAllServersSummary, 3000);
            }
            return true;
        }
        #endregion

        #region private methods
        void GatherTemplatesSetting()
        {
            datas.Clear();
            var ucs = flyPanel.Controls.OfType<Views.UserControls.ConfigTemplateUI>();
            var index = 1;
            foreach (var uc in ucs)
            {
                uc.SetIndex(index++);
                var tpl = uc.GetTemplateSettings();
                datas.Add(tpl);
            }
        }

        void BindFlyPanelDragDropEvent()
        {
            flyPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };

            flyPanel.DragDrop += (s, a) =>
            {
                var data = a.Data;
                if (data.GetDataPresent(typeof(Views.UserControls.ConfigTemplateUI)))
                {
                    SwapFlyControls(s, a);
                }
            };
        }

        private void SwapFlyControls(object sender, DragEventArgs args)
        {
            // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag
            var panel = sender as FlowLayoutPanel;
            Point pos = panel.PointToClient(new Point(args.X, args.Y));
            if (
                !(
                    args.Data.GetData(typeof(Views.UserControls.ConfigTemplateUI))
                    is Views.UserControls.ConfigTemplateUI curItem
                )
                || !(panel.GetChildAtPoint(pos) is Views.UserControls.ConfigTemplateUI destItem)
                || curItem == destItem
            )
            {
                return;
            }

            // !must! reset item index!!
            var destPos = panel.Controls.GetChildIndex(destItem, false);
            panel.Controls.SetChildIndex(curItem, destPos);
            panel.Invalidate();
        }

        void BindButtonAdd()
        {
            btnAdd.Click += (s, a) =>
            {
                var form = new Views.WinForms.FormCustomConfigTemplates();
                form.FormClosed += (_, __) =>
                {
                    if (form.DialogResult == DialogResult.OK)
                    {
                        var inbS = form.inbS;
                        VgcApis.Misc.UI.Invoke(() =>
                        {
                            var uc = new Views.UserControls.ConfigTemplateUI(inbS);
                            flyPanel.Controls.Add(uc);
                            uc.SetIndex(flyPanel.Controls.Count);
                        });
                    }
                };
                form.Show();
            };
        }

        void InitPanel()
        {
            flyPanel.SuspendLayout();
            for (var i = 0; i < datas.Count; i++)
            {
                var uc = new Views.UserControls.ConfigTemplateUI(datas[i]);
                flyPanel.Controls.Add(uc);
            }
            flyPanel.ResumeLayout();
        }
        #endregion

        #region protected methods

        #endregion
    }
}
