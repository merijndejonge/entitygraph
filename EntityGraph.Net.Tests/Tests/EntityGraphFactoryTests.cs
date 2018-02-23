using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    [TestClass]
    public class EntityGraphFactoryTests : EntityGraphTest
    {
        /// <summary>
        /// Test that EntityGraphFactory does not prevent garbage collection of its generated entity graphs
        /// </summary>
        [TestMethod]
        public void WeakReferenceTest()
        {
            var type = typeof(EntityGraphFactory);
            const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.FlattenHierarchy |
                                              BindingFlags.GetField |
                                              BindingFlags.NonPublic;

            Assert.IsNotNull(type.BaseType);
            var entityGraphsField = type.BaseType.GetField("EntityGraphs", bindingFlags);
            Assert.IsNotNull(entityGraphsField);
            var entityGraphs = (Dictionary<int, WeakReference>) entityGraphsField.GetValue(null);

            var initialCount = entityGraphs.Count;

            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(entityGraphs.Count == initialCount + 1);

            var graphElement = entityGraphs.SingleOrDefault(x => x.Value.Target == gr);
            Assert.IsNotNull(graphElement);
            
            Assert.IsTrue(entityGraphs[graphElement.Key].IsAlive);

            gr.Dispose();
            // ReSharper disable once RedundantAssignment
            gr = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsTrue(entityGraphs.Count == initialCount + 1);
            Assert.IsFalse(entityGraphs[graphElement.Key].IsAlive);
        }
    }
}