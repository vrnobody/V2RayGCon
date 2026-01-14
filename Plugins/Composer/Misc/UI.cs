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

        public static void RefreshPanel<TCtrl, TData>(
            FlowLayoutPanel panel,
            IEnumerable<TData> data,
            Func<TData, TCtrl> initFunc
        )
            where TData : class, VgcApis.Interfaces.IHasIndex
            where TCtrl : Control
        {
            var nodes = data.OrderBy(el => el.GetIndex()).ToList();
            var container = panel.Controls;
            container.Clear();
            foreach (var node in nodes)
            {
                container.Add(initFunc(node));
            }
        }

        public static bool SwapUserControls<T>(FlowLayoutPanel panel, T curItem, DragEventArgs args)
            where T : Control, VgcApis.Interfaces.IHasIndex
        {
            // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag
            Point p = panel.PointToClient(new Point(args.X, args.Y));
            var destItem = panel.GetChildAtPoint(p) as T;
            if (destItem == null || curItem == destItem)
            {
                return false;
            }

            // swap index
            var destIdx = destItem.GetIndex() + 0.5;
            var curIdx = (curItem.GetIndex() > destIdx) ? destIdx - 0.1 : destIdx + 0.1;
            destItem.SetIndex(destIdx);
            curItem.SetIndex(curIdx);

            // refresh panel
            var destPos = panel.Controls.GetChildIndex(destItem, false);
            panel.Controls.SetChildIndex(curItem, destPos);
            panel.Invalidate();
            return true;
        }
    }
}
