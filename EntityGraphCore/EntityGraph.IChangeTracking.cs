using System.ComponentModel;
using System.Linq;

namespace OpenSoftware.EntityGraphCore
{
    // ReSharper disable once UnusedTypeParameter
    public partial class EntityGraph<TEntity> : IChangeTracking
    {
        /// <inheritdoc />
        /// <summary>
        /// Resets the entity graph’s state to unchanged by accepting the modifications of all its entities.
        /// </summary>
        public void AcceptChanges()
        {
            foreach (var entity in this.OfType<IChangeTracking>())
            {
                entity.AcceptChanges();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the changed status of the entity graph.
        /// </summary>
        public bool IsChanged
        {
            get { return this.OfType<IChangeTracking>().Aggregate(false, (isChanged, e) => isChanged | e.IsChanged); }
        }
    }
}