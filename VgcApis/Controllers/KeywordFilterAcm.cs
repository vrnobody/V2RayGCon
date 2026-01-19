using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VgcApis.UserControls;
using VgcApis.UserControls.AcmComboBoxComps;

namespace VgcApis.Controllers
{
    public class KeywordFilterAcm : IDisposable
    {
        private AutocompleteMenu menu;

        public KeywordFilterAcm(AcmToolStripComboBox box)
        {
            this.menu = CreateToolStripComboBoxAcm(box);
        }

        public KeywordFilterAcm(AcmComboBox box)
        {
            this.menu = CreateComboBoxAcm(box);
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

        AutocompleteMenu CreateComboBoxAcm(AcmComboBox box)
        {
            var m = CreateAcmMenu();
            var tooltips = VgcApis.Libs.Infr.KeywordFilter.GetFilterTips();
            var snippets = CreateSnippets(tooltips);

            m.TargetControlWrapper = new ExComboBoxWrapper(box);
            m.SetAutocompleteItems(snippets);
            return m;
        }

        AutocompleteMenu CreateToolStripComboBoxAcm(AcmToolStripComboBox box)
        {
            var m = CreateAcmMenu();
            var tooltips = VgcApis.Libs.Infr.KeywordFilter.GetAllTips();
            var snippets = CreateSnippets(tooltips);

            m.TargetControlWrapper = new ExToolStripComboBoxWrapper(box);
            m.SetAutocompleteItems(snippets);
            return m;
        }

        private AutocompleteMenu CreateAcmMenu()
        {
            var m = new AutocompleteMenu()
            {
                MaximumSize = new Size(300, 200),
                ToolTipDuration = 20000,
                MinFragmentLength = 0,
                ImageList = new ImageList(),
            };
            var icons = new Bitmap[]
            {
                Properties.Resources.DomainType_16x, // num
                Properties.Resources.Button_16x, // str
                Properties.Resources.SortAscending_16x, // #orderby
                Properties.Resources.CheckboxUncheckCancel_16x, // #take
                Properties.Resources.GoToLastRow_16x, // #goto
                Properties.Resources.CPPCommentCode_16x, // #//
            };
            m.ImageList.Images.AddRange(icons);
            return m;
        }

        private static List<KeywordFilterSnippet> CreateSnippets(
            List<System.Collections.ObjectModel.ReadOnlyCollection<string>> tooltips
        )
        {
            List<KeywordFilterSnippet> snippets = new List<KeywordFilterSnippet>();
            for (int i = 0; i < tooltips.Count; i++)
            {
                var snp = tooltips[i].Select(tip => new KeywordFilterSnippet(tip, i));
                snippets.AddRange(snp);
            }

            return snippets;
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
