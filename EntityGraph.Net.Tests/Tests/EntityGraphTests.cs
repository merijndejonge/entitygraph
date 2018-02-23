using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Model;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    public abstract class EntityGraphTest
    {
        protected A a;
        protected B b;
        protected C c;
        protected D d;

        [TestInitialize]
        public virtual void TestSetup()
        {
            a = new A {Name = "A"};
            b = new B {Name = "B"};
            c = new C {Name = "C"};
            d = new D {Name = "D"};

            a.B = b;
            b.C = c;
            c.D = d;
            d.A = a;
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
        }
    }
}