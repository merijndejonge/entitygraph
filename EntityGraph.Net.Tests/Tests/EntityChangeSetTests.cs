using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.EntityGraph.Net.Tests.Tests.ChangeSet;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    namespace ChangeSet
    {
        public class A : IEntityStateTracking
        {
            public A(EntityState entityState)
            {
                EntityState = entityState;
            }

            public EntityState EntityState { get; }
        }
    }

    [TestClass]
    public class EntityChangeSetTests : EntityGraphTest
    {
        [TestMethod]
        public void ChangeSetIsEmptyTest()
        {
            var aa = new A(EntityState.New);
            var cs = new EntityGraphChangeSet();
            Assert.IsTrue(cs.IsEmpty);

            cs.RemovedEntities = new ReadOnlyCollection<IEntityStateTracking>(new List<IEntityStateTracking>());
            Assert.IsTrue(cs.IsEmpty);

            cs.AddedEntities = new ReadOnlyCollection<IEntityStateTracking>(new List<IEntityStateTracking> {aa});
            Assert.IsFalse(cs.IsEmpty);
        }

        [TestMethod]
        public void ChangeTrackNewTest()
        {
            var aa = new A(EntityState.New);
            var graph = new EntityGraph(aa, new EntityGraphShape());

            var changes = graph.GetChanges();

            Assert.IsTrue(changes.AddedEntities.Contains(aa));
        }

        [TestMethod]
        public void ChangeTrackModifiedTest()
        {
            var aa = new A(EntityState.Modified);
            var graph = new EntityGraph(aa, new EntityGraphShape());

            var changes = graph.GetChanges();

            Assert.IsTrue(changes.ModifiedEntities.Contains(aa));
        }

        [TestMethod]
        public void ChangeTrackDeletedTest()
        {
            var aa = new A(EntityState.Deleted);
            var graph = new EntityGraph(aa, new EntityGraphShape());

            var changes = graph.GetChanges();

            Assert.IsTrue(changes.RemovedEntities.Contains(aa));
        }
    }
}