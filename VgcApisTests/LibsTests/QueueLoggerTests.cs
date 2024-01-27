using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class QueueLoggerTests
    {
        [TestMethod]
        public void SingleThreadQLoggerTest()
        {
            var cap = 5;
            var q = new VgcApis.Libs.Sys.QueueLogger(cap);

            var logs = q.GetLogAsString(false);
            Assert.AreEqual("", logs);

            var sb = new StringBuilder();
            for (int i = 0; i < cap; i++)
            {
                q.Log(i.ToString());
                sb.AppendLine(i.ToString());
            }
            logs = q.GetLogAsString(true);
            var exp = sb.ToString();
            Assert.AreEqual(exp, logs);
            logs = q.GetLogAsString(false);
            exp = VgcApis.Misc.Utils.TrimTrailingNewLine(exp);
            Assert.AreEqual(exp, logs);

            for (int i = cap; i < cap + 3; i++)
            {
                q.Log(i.ToString());
            }
            sb = new StringBuilder();
            for (int i = 3; i < cap + 3; i++)
            {
                sb.AppendLine(i.ToString());
            }
            exp = sb.ToString();
            logs = q.GetLogAsString(true);
            Assert.AreEqual(exp, logs);

            q.Clear();
            logs = q.GetLogAsString(false);
            Assert.AreEqual("", logs);

            sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                q.Log(i.ToString());
                sb.AppendLine(i.ToString());
            }
            logs = q.GetLogAsString(true);
            exp = sb.ToString();
            Assert.AreEqual(exp, logs);

            for (int i = 103; i < 200; i++)
            {
                q.Log(i.ToString());
            }
            sb = new StringBuilder();
            for (int i = 200 - cap; i < 200; i++)
            {
                sb.AppendLine(i.ToString());
            }
            exp = sb.ToString();
            logs = q.GetLogAsString(true);
            Assert.AreEqual(exp, logs);

            q.Dispose();
            Assert.AreEqual("", q.GetLogAsString(false));

            q.Log("hello");
            Assert.AreEqual("", q.GetLogAsString(false));
        }
    }
}
