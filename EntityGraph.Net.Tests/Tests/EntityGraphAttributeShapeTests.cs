using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class EntityGraphAttributeShapeTests
    {
        public class A
        {
            [EntityGraph] public B B { get; set; }
            [EntityGraph("SomeEntityGraphName")] public C C { get; set; }
        }

        public class B
        {
        }

        public class C
        {
        }

        [TestMethod]
        public void AnymousEntityGraphAttributeShapeTest()
        {

            var a = new A();
            var b = new B();
            var c = new C();
            a.B = b;
            a.C = c;

            var gr = a.EntityGraph(new EntityGraphAttributeShape());
            Assert.IsTrue(gr.Count() == 3);
            Assert.IsTrue(gr.Contains(a));
            Assert.IsTrue(gr.Contains(b));
            Assert.IsTrue(gr.Contains(c));
        }

        [TestMethod]
        public void NamedEntityGraphAttributeShapeTest()
        {

            var a = new A();
            var b = new B();
            var c = new C();
            a.B = b;
            a.C = c;

            var gr = a.EntityGraph(new EntityGraphAttributeShape("SomeEntityGraphName"));
            Assert.IsTrue(gr.Count() == 2);
            Assert.IsTrue(gr.Contains(a));
            Assert.IsFalse(gr.Contains(b));
            Assert.IsTrue(gr.Contains(c));
        }
    }
}