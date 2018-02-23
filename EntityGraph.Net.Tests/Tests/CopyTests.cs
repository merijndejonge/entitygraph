using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Model;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class CopyTests : EntityGraphTest
    {
        [TestInitialize]
        public override void TestSetup()
        {
            base.TestSetup();
            IdFactory.AutoGenerateKeys = true;
        }
        public override void TestCleanup()
        {
            base.TestCleanup();
            IdFactory.AutoGenerateKeys = false;
        }
        [TestMethod]
        public void CopyTest()
        {
            a.BSet.Add(new B { Name = "B1" });
            var copyOfA = a.Copy(EntityGraphs.CircularGraphFull);
            Assert.AreEqual((object) a.EntityGraph(EntityGraphs.CircularGraphFull).Count(), copyOfA.EntityGraph(EntityGraphs.CircularGraphFull).Count(), "Copy of a does not have same number of elements");
            var zip = a.EntityGraph(EntityGraphs.CircularGraphFull).Zip(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull), (ea, eb) => new { ea, eb });
            foreach(var el in zip)
            {
                Assert.IsTrue(el.ea.GetType() == el.eb.GetType(), "Copy is not equal");
                switch (el.ea)
                {
                    case A a1:
                        Assert.IsTrue(a1.Name == ((A)el.eb).Name);
                        break;
                    case B b1:
                        Assert.IsTrue(b1.Name == ((B)el.eb).Name);
                        break;
                    case C c1:
                        Assert.IsTrue(c1.Name == ((C)el.eb).Name);
                        break;
                    case D d1:
                        Assert.IsTrue(d1.Name == ((D)el.eb).Name);
                        break;
                }
            }
        }

        [TestMethod]
        public void CopyNamedGraphTest()
        {
            a.BSet.Add(new B { Name = "B1" });
            var copyOfA = a.Copy(EntityGraphs.CircularGraphShape1);
            var gr1 = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var gr2 = copyOfA.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr1.Count() == gr2.Count() + 1, "Copy of a does the correct number of elements");
            Assert.IsTrue(!copyOfA.BSet.Any());
            var zip = a.EntityGraph(EntityGraphs.CircularGraphFull).Zip(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull), (ea, eb) => new { ea, eb });
            foreach(var el in zip)
            {
                Assert.AreEqual(el.ea.GetType(), el.eb.GetType(), "Copy is not equal");
                switch (el.ea)
                {
                    case A _:
                        Assert.IsTrue(((A)el.ea).Name == ((A)el.eb).Name);
                        break;
                    case B _:
                        Assert.IsTrue(((B)el.ea).Name == ((B)el.eb).Name);
                        break;
                    case C _:
                        Assert.IsTrue(((C)el.ea).Name == ((C)el.eb).Name);
                        break;
                    case D _:
                        Assert.IsTrue(((D)el.ea).Name == ((D)el.eb).Name);
                        break;
                }
            }
        }

        /// <summary>
        /// Test that copying a one-2-many relation copies all members of the entity graph
        /// </summary>
        [TestMethod]
        public void OneToManyCopyTest()
        {
            var f = new F();
            f.ESet.Add(new E());
            f.ESet.Add(new E());

            var copyOfF = f.Copy(EntityGraphs.SimpleGraphShapeFull);
            var g1 = f.EntityGraph(EntityGraphs.SimpleGraphShapeFull);
            var g2 = copyOfF.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));
            Assert.IsTrue(copyOfF.ESet.Count == 2);
        }
        /// <summary>
        /// Test that copying a "named" one-2-many relation copies all members of the entity graph
        /// </summary>
        [TestMethod]
        public void OneToManyNamedCopyTest1()
        {
            var f = new F();
            f.ESet.Add(new E());
            f.ESet.Add(new E());

            var copyOfF = f.Copy(EntityGraphs.SimpleGraphShape1);
            var g1 = f.EntityGraph(EntityGraphs.SimpleGraphShape1);
            var g2 = copyOfF.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));
            Assert.IsTrue(copyOfF.ESet.Count == 2);
        }
        /// <summary>
        /// Tests that copying using a non-existing graph name, will yield a graph without the one-2-many associations
        /// </summary>
        [TestMethod]
        public void OneToManyNamedCopyTest2()
        {
            var f = new F();

            var copyOfF = f.Copy(EntityGraphs.SimpleGraphShape3);
            var g1 = f.EntityGraph(EntityGraphs.SimpleGraphShape3);
            var g2 = copyOfF.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));
            Assert.IsTrue(!copyOfF.ESet.Any());
        }
        /// <summary>
        /// Tests that copying a many-2-one relation will copy the complete entity graph
        /// </summary>
        [TestMethod]
        public void ManyToOneCopyTest1()
        {
            var f = new F();
            var e1 = new E { F = f };

            var copyOfE1 = e1.Copy(EntityGraphs.SimpleGraphShapeFull);
            var g1 = e1.EntityGraph(EntityGraphs.SimpleGraphShapeFull);
            var g2 = copyOfE1.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));
            Assert.IsTrue(copyOfE1.F != f);
        }
        /// <summary>
        /// Test to check for a circular entity graph it doesn't matter which element is copied. The 
        /// result will always be an entity graph with the same entities.
        /// </summary>
        [TestMethod]
        public void CircularGraphCopyTest()
        {
            var g1 = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var g2 = b.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(g1.All(n => g2.Contains(n)));
            Assert.IsTrue(g2.All(n => g1.Contains(n)));
        }

        [TestMethod]
        public void CopyWithSelfReferenceTest()
        {
            var testClass = new SelfReferenceTestClass();
            testClass.SelfReference = testClass;
            Assert.IsTrue(testClass == testClass.SelfReference);

            var shape = new EntityGraphShape().Edge<SelfReferenceTestClass, SelfReferenceTestClass>(x => x.SelfReference);
            var copy = testClass.Copy(shape);
            Assert.IsTrue(copy == copy.SelfReference);
            Assert.IsTrue(copy.EntityGraph(shape).IsCopyOf(testClass.EntityGraph(shape)));
        }
    }
    public class SelfReferenceTestClass 
    {
        public string Name { get; set; }
        public SelfReferenceTestClass SelfReference { get; set; }
    }
}
