﻿using System;
using System.Drawing;

namespace VgcApis.UserControls.AcmComboBoxComps
{
    [Serializable]
    public class Colors
    {
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color SelectedForeColor { get; set; }
        public Color SelectedBackColor { get; set; }
        public Color SelectedBackColor2 { get; set; }
        public Color HighlightingColor { get; set; }

        public Colors()
        {
            ForeColor = Color.Black;
            BackColor = Color.White;
            SelectedForeColor = Color.Black;
            SelectedBackColor = Color.Orange;
            SelectedBackColor2 = Color.White;
            HighlightingColor = Color.Orange;
        }
    }
}
