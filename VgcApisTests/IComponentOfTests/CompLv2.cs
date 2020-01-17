using System.Diagnostics;

namespace VgcApisTests.IComponentOfTests
{
    public class CompLv2 :
        VgcApis.Models.BaseClasses.ComponentOf<CompLv1>
    {

        public CompLv2() { }

        public string Name() => "Comp lv2";

        protected override void AfterComponentsDisposed()
        {
            Debug.WriteLine("Comp lv2 disposed.");
        }
    }
}
