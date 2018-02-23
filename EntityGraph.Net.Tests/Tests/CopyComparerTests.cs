using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Model;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class CopyComparerTests : EntityGraphTest
    {
        /// <summary>
        /// Checks that an entitygraph can never be a copy of it self
        /// </summary>
        [TestMethod]
        public void IsCopyOfIdentityTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsFalse(gr.IsCopyOf(gr));
        }

        [TestMethod]
        public void IsCopyOfNamedIdentityTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphShape1);
            Assert.IsFalse(gr.IsCopyOf(gr));
        }

        [TestMethod]
        public void IsCopyOfSimpleCopyTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var copyOfA = a.Copy(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull)));
        }

        [TestMethod]
        public void IsCopyOfNamedCopyTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphShape1);
            var copyOfA = a.Copy(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph(EntityGraphs.CircularGraphShape1)));
        }

        [TestMethod]
        public void IsCopyOfEqualShallowCopyTest()
        {
            var gr1 = a.EntityGraph(EntityGraphs.CircularGraphShape1);
            a.BNotInGraph = new B();
            var copyOfA = a.Copy(EntityGraphs.CircularGraphFull);
            var gr2 = copyOfA.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr1.IsCopyOf(gr2));
        }

        [TestMethod]
        public void IsCopyOfCheckModifiedPropertyTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var copyOfA = a.Copy(EntityGraphs.CircularGraphFull);
            a.B.Name = "test";
            Assert.IsFalse(gr.IsCopyOf(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull)));

            copyOfA.B.Name = "test";
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull)));
        }

        [TestMethod]
        public void IsCopyOfCheckModifiedCollectionTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var copyOfA = a.Copy(EntityGraphs.CircularGraphFull);
            a.BSet.Add(new B());
            Assert.IsFalse(gr.IsCopyOf(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull)));

            // Observe that entities are checked for membe-wise equality, not for binair equality.
            copyOfA.BSet.Add(new B());
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull)));
        }
    }
}