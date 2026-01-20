using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Composer.Models;

namespace Composer.Misc
{
    public static class UI
    {
        public static bool HasDropablePackageNameControl(DragEventArgs e)
        {
            var name = typeof(Views.UserControls.PkgNameUC).FullName;
            foreach (string ty in e.Data.GetFormats())
            {
                if (ty == name)
                {
                    return true;
                }
            }
            return false;
        }

        static readonly List<string> dropableTypes = new List<string>()
        {
            typeof(Views.UserControls.ServerSelectorUC).FullName,
            VgcApis.Models.Consts.UI.VgcServUiName,
        };

        public static bool HasDropableSelectorControl(DragEventArgs e)
        {
            foreach (string ty in e.Data.GetFormats())
            {
                if (dropableTypes.Contains(ty))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
