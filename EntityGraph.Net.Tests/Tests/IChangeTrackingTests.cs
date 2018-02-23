using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Tests.ChangeTrackingTests;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    namespace ChangeTrackingTests
    {
        public class A : IChangeTracking
        {
            private string _name;

            public void AcceptChanges()
            {
                IsChanged = false;
            }

            public B B { get; set; }

            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    IsChanged = true;
                }
            }

            public bool IsChanged { get; private set; }
        }

        public class B : IChangeTracking
        {
            private string _name;

            public void AcceptChanges()
            {
                IsChanged = false;
            }

            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    IsChanged = true;
                }
            }
            public bool IsChanged { get; private set; }
        }
    }

    [TestClass]
    public class IChangeTrackingTests : EntityGraphTest
    {
        [TestMethod]
        public void IsChangedAndAcceptChangesTest()
        {
            var aa = new A {B = new B()};
            var graph = new EntityGraph(aa, new EntityGraphShape().Edge<A,B>(x => x.B));

            Assert.IsFalse(aa.IsChanged);
            Assert.IsFalse(aa.B.IsChanged);
            Assert.IsFalse(graph.IsChanged);

            aa.B.Name = "Changed";
            Assert.IsFalse(aa.IsChanged);
            Assert.IsTrue(aa.B.IsChanged);
            Assert.IsTrue(graph.IsChanged);

            graph.AcceptChanges();
            Assert.IsFalse(aa.IsChanged);
            Assert.IsFalse(aa.B.IsChanged);
            Assert.IsFalse(graph.IsChanged);
        }
    }
}
