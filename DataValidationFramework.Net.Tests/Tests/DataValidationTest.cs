using DataValidationFramework.Net.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataValidationFramework.Net.Tests.Tests
{
    public abstract class DataValidationTest
    {
        protected A A;
        protected B B;
        protected C C;
        protected D D;

        [TestInitialize]
        public virtual void TestSetup()
        {
            A = new A {Name = nameof(A)};
            B = new B {Name = nameof(B)};
            C = new C {Name = nameof(C)};
            D = new D {Name = nameof(D)};

            A.B = B;
            B.C = C;
            C.D = D;
            D.A = A;
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
        }
    }
}