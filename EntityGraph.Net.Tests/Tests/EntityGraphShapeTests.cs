using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Model;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class EntityGraphShapeTests : EntityGraphTest
    {
        [TestMethod]
        public void MyTestMethod()
        {
        }

        [TestMethod]
        public void SimpleGraphShapeTests()
        {
            var newB = new B {Name = "NewB"};
            a.BSet.Add(newB);
            var shape = new EntityGraphShape()
                .Edge<A, B>(x => x.B)
                .Edge<A, B>(x => x.BSet);

            var gr = a.EntityGraph(shape);

            Assert.IsTrue(gr.Contains(a));
            Assert.IsTrue(gr.Contains(b));
            Assert.IsTrue(gr.Contains(newB));
        }

        [TestMethod]
        public void InvalidPathExpressionTest1()
        {
            var isValidExpression = true;
            try
            {
                new EntityGraphShape().Edge<A, D>(x => x.B.C.D);
            }
            catch (Exception)
            {
                isValidExpression = false;
            }

            Assert.IsFalse(isValidExpression);
        }

        [TestMethod]
        public void InvalidPathExpressionTest2()
        {
            var isValidExpression = true;
            try
            {
                new EntityGraphShape().Edge<A, B>(x => x.BSet.First());
            }
            catch (Exception)
            {
                isValidExpression = false;
            }

            Assert.IsFalse(isValidExpression);
        }

        [TestMethod]
        public void InvalidPathExpressionTest3()
        {
            var isValidExpression = true;
            try
            {
                new EntityGraphShape().Edge<A, A>(x => x.B.ASet);
            }
            catch (Exception)
            {
                isValidExpression = false;
            }

            Assert.IsFalse(isValidExpression);
        }

        [TestMethod]
        public void EntitiyGraphShapeUnionTest()
        {
            var shape1 = new EntityGraphShape()
                .Edge<A, D>(x => x.DSet)
                .Edge<A, B>(x => x.B);
            var shape2 = new EntityGraphShape()
                .Edge<A, D>(x => x.DSet)
                .Edge<A, B>(x => x.B)
                .Edge<C, D>(x => x.D);
            var shape3 = shape1.Union(shape2);

            Assert.IsTrue(shape1.All(edge => shape3.Contains(edge)));
            Assert.IsTrue(shape2.All(edge => shape2.Contains(edge)));

            Assert.IsFalse(shape3.Any(edge => shape1.Contains(edge) == false && shape2.Contains(edge) == false));
            Assert.IsTrue(shape3.Count() == 3);
        }
    }
}
