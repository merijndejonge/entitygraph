using System;
using System.Collections.Generic;

namespace OpenSoftware.EntityGraphCore
{
    /// <summary>
    /// Class that implements a cache for instantiated entity graphs using weak references.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityGraphFactory<TEntity>
        where TEntity : class
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Dictionary<int, WeakReference> EntityGraphs = new Dictionary<int, WeakReference>();

        protected abstract EntityGraph<TEntity> Create(TEntity entity, IEntityGraphShape shape);

        /// <summary>
        /// Creates a new entity graph for the given entity and shape and stores it in the
        /// cache, or returns the graph from the cache if it is already present there.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        protected EntityGraph<TEntity> GetOrCreate(TEntity entity, IEntityGraphShape shape)
        {
            {
                var key = entity.GetHashCode() ^ shape.GetHashCode();
                WeakReference reference;
                if (EntityGraphs.ContainsKey(key) == false)
                {
                    reference = new WeakReference(null);
                    EntityGraphs.Add(key, reference);
                }
                else
                {
                    reference = EntityGraphs[key];
                }

                if (reference.Target == null)
                {
                    reference.Target = Create(entity, shape);
                }

                return EntityGraphs[key].Target as EntityGraph<TEntity>;
            }
        }
    }
}