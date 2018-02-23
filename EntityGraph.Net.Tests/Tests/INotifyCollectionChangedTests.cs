using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Model;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class INotifyCollectionChangedTests : EntityGraphTest
    {
        [TestMethod]
        public void ICollectionChangedTest()
        {
            var collectionChangedHandlerVisited = false;
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.CollectionChanged += (sender, args) => { collectionChangedHandlerVisited = true; };
            a.BSet.Add(new B());
            Assert.IsTrue(collectionChangedHandlerVisited, "CollectionChanged handler not called");
        }

        [TestMethod]
        public void AddToEntityCollectionTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var bb = new B();
            a.BSet.Add(bb);
            Assert.IsTrue(gr.Contains(bb), "Entity graph does not contain entity b");
        }

        [TestMethod]
        public void RemoveFromEntityCollectionTest()
        {
            var bb = new B();
            a.BSet.Add(bb);
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            a.BSet.Remove(bb);
            Assert.IsFalse(gr.Contains(bb), "Entity graph still contains entity b");
        }
    }
}
