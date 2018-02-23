using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenSoftware.EntityGraphCore
{
    // ReSharper disable once UnusedTypeParameter
    public partial class EntityGraph<TEntity>
    {
        public EntityGraphChangeSet GetChanges()
        {
            var addedEntities = new List<IEntityStateTracking>();
            var modifiedEntities = new List<IEntityStateTracking>();
            var removedEntities = new List<IEntityStateTracking>();

            var changeSet = new EntityGraphChangeSet
            {
                AddedEntities = new ReadOnlyCollection<IEntityStateTracking>(addedEntities),
                ModifiedEntities = new ReadOnlyCollection<IEntityStateTracking>(modifiedEntities),
                RemovedEntities = new ReadOnlyCollection<IEntityStateTracking>(removedEntities)
            };

            foreach (var node in EntityRelationGraph.OfType<IEntityStateTracking>())
            {
                switch (node.EntityState)
                {
                    case EntityState.Deleted:
                        removedEntities.Add(node);
                        break;
                    case EntityState.Modified:
                        modifiedEntities.Add(node);
                        break;
                    case EntityState.New:
                        addedEntities.Add(node);
                        break;
                }
            }

            return changeSet;
        }
    }
}