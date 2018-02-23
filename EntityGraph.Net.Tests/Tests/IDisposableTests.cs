using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Model;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class IDisposableTests : EntityGraphTest
    {
        [TestMethod]
        public void IDisposableTest()
        {
            var propertyChangedCalled = false;
            var collectionChangedCalled = false;
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.PropertyChanged += (sender, args) => { propertyChangedCalled = true; };
            gr.CollectionChanged += (sender, args) => { collectionChangedCalled = true; };
            gr.Dispose();
            a.Name = "Some Name";
            a.BSet.Add(new B());
            Assert.IsFalse(propertyChangedCalled, "PropertyChanged event handler is called after Dispose()");
            Assert.IsFalse(collectionChangedCalled, "CollectionChanged event handler is called after Dispose()");
        }
    }
}
