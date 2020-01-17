using System.Diagnostics;

namespace VgcApisTests.IComponentOfTests
{
    public class CompLv1 :
        VgcApis.BaseClasses.ComponentOf<Container>
    {
        public CompLv1() { }

        public string Name() => "Component lv1";

        protected override void CleanupAfterChildrenDisposed()
        {
            Debug.WriteLine("Comp lv1 disposed.");
        }
    }
}
