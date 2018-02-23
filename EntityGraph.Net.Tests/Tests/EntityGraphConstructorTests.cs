using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class EntityGraphConstructorTests : EntityGraphTest
    {
        [TestMethod]
        [Description(
            "Tests that passing a null value for the source entity constructor argument generates an exception")]
        public void NullConstructorArgumentThrowsExceptionTest1()
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new EntityGraph(null, new EntityGraphAttributeShape());
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        [Description("Tests that passing a null value for the graph shape constructor argument generates an exception")]
        public void NullConstructorArgumentThrowsExceptionTest2()
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new EntityGraph(a, default(EntityGraphShape));
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
        }

    }
}