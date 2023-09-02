using System.Diagnostics;

namespace VgcApisTests.IComponentOfTests
{
    public class Container : VgcApis.BaseClasses.ComponentOf<Container>
    {
        public Container() { }

        public string Name() => "Container";

        protected override void CleanupAfterChildrenDisposed()
        {
            Debug.WriteLine("Container disposed.");
        }
    }
}
