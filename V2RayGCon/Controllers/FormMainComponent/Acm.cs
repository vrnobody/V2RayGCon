using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VgcApis.UserControls;
using VgcApis.UserControls.AcmComboBoxComps;

namespace V2RayGCon.Controllers.FormMainComponent
{
    internal class Acm : IDisposable
    {
        private readonly AcmComboBox cbox;
        private AutocompleteMenu menu;

        public Acm(AcmComboBox box)
        {
            this.cbox = box;
            this.menu = CreateAcm(box);
        }

        #region public methods
        #endregion

        #region private methods
        void CleanupAcm()
        {
            var control = menu.TargetControlWrapper.TargetControl;
            menu.SetAutocompleteMenu(control, null);
            menu.Dispose();
        }

        AutocompleteMenu CreateAcm(AcmComboBox box)
        {
            var m = new AutocompleteMenu()
            {
                MaximumSize = new Size(300, 200),
                ToolTipDuration = 20000,
                SearchPattern = "#",
                MinFragmentLength = 1,
                ImageList = new ImageList(),
            };

            m.ImageList.Images.Add(Properties.Resources.DomainType_16x);
            m.ImageList.Images.Add(Properties.Resources.TextBlock_16x);

            var numSnippets = VgcApis
                .Libs.Infr.KeywordFilter.GetNumericTips()
                .Select(t => new Snippet(t, 0));
            var strSnippets = VgcApis
                .Libs.Infr.KeywordFilter.GetStringTips()
                .Select(t => new Snippet(t, 1));

            m.TargetControlWrapper = new ExToolStripComboBoxWrapper(box);
            m.SetAutocompleteItems(numSnippets.Concat(strSnippets));
            return m;
        }
        #endregion

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    CleanupAcm();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Acm()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
