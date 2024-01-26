using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests
{
    [TestClass]
    public class ReflectionTests
    {
        public ReflectionTests() { }

        [TestMethod]
        public void WrappedCoreServCtrlInterfaceTest()
        {
            var iterfaces = new List<Type>
            {
                typeof(VgcApis.Interfaces.IWrappedCoreServCtrl),
                typeof(VgcApis.Interfaces.CoreCtrlComponents.IConfiger),
                typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl),
                typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates),
                typeof(VgcApis.Interfaces.CoreCtrlComponents.ILogger),
            };

            var cache = new HashSet<string>();

            foreach (var it in iterfaces)
            {
                var minfs = it.GetMethods();
                foreach (var minf in minfs)
                {
                    var pn = minf.GetParameters().Length;
                    var s = $"{minf.Name}({pn} params)";
                    if (cache.Contains(s))
                    {
                        Assert.Fail($"Duplicated func: {s}");
                    }
                    cache.Add(s);
                }
            }
        }
    }
}
