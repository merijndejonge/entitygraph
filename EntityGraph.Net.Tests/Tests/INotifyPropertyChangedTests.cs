using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Model;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class INotifyPropertyChangedTests : EntityGraphTest
    {
        [TestMethod]
        public void INotifyPropertyChangedTest()
        {
            var propertyChangedHandlerVisited = false;

            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.PropertyChanged += (sender, args) => { propertyChangedHandlerVisited = true; };
            d.Name = "Hello";
            Assert.IsTrue(propertyChangedHandlerVisited, "PropertyChanged handler not called");
        }

        [TestMethod]
        public void AddAssociationTest()
        {
            var newB = new B();
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsFalse(gr.Contains(newB));

            a.B = newB;
            Assert.IsTrue(gr.Contains(newB));
        }

        [TestMethod]
        public void RemoveAssociationTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.Contains(b));

            a.B = null;
            ;
            Assert.IsFalse(gr.Contains(b));
        }
    }
}
