using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OpenSoftware.EntityGraph.Net.Tests")]

namespace OpenSoftware.EntityGraphCore
{
    public class EntityGraphChangeSet : IEnumerable<IEntityStateTracking>
    {
        public ReadOnlyCollection<IEntityStateTracking> AddedEntities { get; internal set; }
        public ReadOnlyCollection<IEntityStateTracking> ModifiedEntities { get; internal set; }
        public ReadOnlyCollection<IEntityStateTracking> RemovedEntities { get; internal set; }

        public bool IsEmpty
        {
            get
            {
                if (ModifiedEntities != null && ModifiedEntities.Count > 0)
                {
                    return false;
                }

                if (AddedEntities != null && AddedEntities.Count > 0)
                {
                    return false;
                }

                if (RemovedEntities != null && RemovedEntities.Count > 0)
                {
                    return false;
                }

                return true;
            }
        }

        public IEnumerator<IEntityStateTracking> GetEnumerator()
        {
            foreach (var entity in AddedEntities)
            {
                yield return entity;
            }

            foreach (var entity in ModifiedEntities)
            {
                yield return entity;
            }

            foreach (var entity in RemovedEntities)
            {
                yield return entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("Added = {0}, Modified = {1}, Removed = {2}",
                AddedEntities.Count,
                ModifiedEntities.Count,
                RemovedEntities.Count);
        }
    }
}