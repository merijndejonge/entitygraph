using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class FullEntityGraphShapeTests : EntityGraphTest
    {
        public class A
        {
            [Association("ab", null, null)] public B B { get; set; }

            public int Foo { get; set; }
        }

        public class B
        {
            public int Bar { get; set; }
            [Association("cs", null, null)] public List<C> Cs { get; set; }
            [Association("a", null, null)] public A A { get; set; }
        }

        public class C
        {
            [Association("a", null, null)] public A A { get; set; }
        }

        [TestMethod]
        public void FullEntityGraphShapeTestSingleAssociations()
        {
            var aa = new A {B = new B()};

            var shape = new FullEntityGraphShape();

            var outedges = shape.OutEdges(aa).ToArray();
            Assert.IsTrue(outedges.Length == 1);
            Assert.IsTrue(outedges.Single() == typeof(A).GetProperty(nameof(A.B)));

            var graph = new EntityGraph<object>(aa, shape);
            Assert.IsTrue(graph.Count() == 2);
        }

        [TestMethod]
        public void FullEntityGraphShapeTestMultiAssociations()
        {
            var bb = new B();
            var shape = new FullEntityGraphShape();
            var outedges = shape.OutEdges(bb);
            Assert.IsTrue(outedges.Count() == 2);
        }

        [TestMethod]
        public void FullEntityGraphShapeTestCollectionAssociations()
        {
            var aa = new A {B = new B()};
            aa.B.Cs = new List<C> {new C()};

            var shape = new FullEntityGraphShape();

            var outedges = shape.OutEdges(aa.B).ToArray();
            Assert.IsTrue(outedges.Length == 2);

            var collection = outedges.Single(x => x.Name == nameof(B.Cs));

            Assert.IsTrue(collection == typeof(B).GetProperty(nameof(B.Cs)));

            var graph = new EntityGraph<object>(aa, shape);
            Assert.IsTrue(graph.Count() == 3);
        }

        [TestMethod]
        public void FullEntityGraphShapeTestCycle()
        {
            var aa = new A {B = new B()};
            aa.B.Cs = new List<C> {new C {A = aa}};

            var shape = new FullEntityGraphShape();

            var graph = new EntityGraph<object>(aa, shape);
            Assert.IsTrue(graph.Count() == 3);
        }
    }
}
