using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static V2RayGCon.Misc.UI;

namespace V2RayGCon.Test
{
    [TestClass]
    public class UITests
    {
        public UITests() { }

        [TestMethod]
        public void EnsureServerUiIsIDropableControlTest()
        {
            var ty = typeof(Views.UserControls.ServerUI).FullName;
            Assert.AreEqual(VgcApis.Models.Consts.UI.VgcServUiName, ty);

            var servUI = new Views.UserControls.ServerUI();
            var dropable = servUI as VgcApis.Interfaces.IDropableControl;
            Assert.IsNotNull(dropable);

            var impossible = servUI as VgcApis.Interfaces.ICoreServCtrl;
            Assert.IsNull(impossible);
        }

        [TestMethod]
        public void UpdateControlOnDemandTest()
        {
            TextBox box = new TextBox { Text = "abc" };
            var result = UpdateControlOnDemand(box, "def");
            Assert.AreEqual("def", box.Text);
            Assert.AreEqual(true, result);

            CheckBox cbox = new CheckBox { Checked = true };
            result = UpdateControlOnDemand(cbox, false);
            Assert.AreEqual(false, cbox.Checked);
            Assert.AreEqual(true, result);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                UpdateControlOnDemand(box, 123);
            });
        }
    }
}
