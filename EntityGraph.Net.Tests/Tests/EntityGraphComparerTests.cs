using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class EntityGraphComparerTests : EntityGraphTest
    {
        [TestMethod]
        public void EntityGraphComparerIdentityTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.EntityGraphEqual(gr));
        }
    }
}