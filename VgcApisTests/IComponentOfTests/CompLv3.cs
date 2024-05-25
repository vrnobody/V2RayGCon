using System.Diagnostics;

namespace VgcApisTests.IComponentOfTests
{
    public class CompLv3 : VgcApis.BaseClasses.ComponentOf<CompLv1>
    {
        public CompLv3(string name)
        {
            this.Name = name;
        }

        public CompLv3() { }

        public string Name { get; set; } = "def property";

        protected override void CleanupAfterChildrenDisposed()
        {
            Debug.WriteLine("Comp lv3 disposed.");
        }
    }
}
