using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Model;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class FactoryTests : EntityGraphTest
    {
        [TestMethod]
        public void FactoryTest()
        {
            var a1 = new A();
            var a2 = new A();
            var shape1 = new EntityGraphShape();
            var shape2 = new EntityGraphShape();

            var gr1 = EntityGraphFactory.Get(a1, shape1);
            var gr2 = EntityGraphFactory.Get(a1, shape2);
            var gr3 = EntityGraphFactory.Get(a2, shape1);
            var gr4 = EntityGraphFactory.Get(a2, shape2);

            var gr5 = EntityGraphFactory.Get(a1, shape1);

            Assert.IsTrue(gr1 == gr5);
            Assert.IsFalse(gr1 == gr2);
            Assert.IsFalse(gr1 == gr3);
            Assert.IsFalse(gr1 == gr4);
            Assert.IsFalse(gr2 == gr3);
            Assert.IsFalse(gr2 == gr4);
            Assert.IsFalse(gr3 == gr4);
        }
    }
}