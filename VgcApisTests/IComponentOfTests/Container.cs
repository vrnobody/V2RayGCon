using System.Diagnostics;

namespace VgcApisTests.IComponentOfTests
{
    public class Container :
        VgcApis.Models.BaseClasses.ComponentOf<Container>
    {
        public Container() { }

        public string Name() => "Container";

        protected override void AfterComponentsDisposed()
        {
            Debug.WriteLine("Container disposed.");
        }

    }
}
