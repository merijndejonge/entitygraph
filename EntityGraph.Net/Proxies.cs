using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net
{
    public static class EntityGraphProxies
    {
        /// <summary>
        /// Extension method that returns an entity graph object as defined by the provided entity graph shape object.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static EntityGraph EntityGraph<TEntity>(this TEntity entity, IEntityGraphShape shape)
            where TEntity : class
        {
            return EntityGraphFactory.Get(entity, shape);
        }

        /// <summary>
        /// Extension method that copies the given entity and all associated entities in the entity graph defined by the given 
        /// entity graph shape.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntity Copy<TEntity>(this TEntity entity, IEntityGraphShape shape) where TEntity : class
        {
            return (TEntity) entity.EntityGraph(shape).Copy(Helpers.GetDataMembers).Source;
        }

        /// <summary>
        /// Determines whether two entity graphs are copies of each other by member-wise comparing all entities in both graphs.
        /// </summary>
        /// <param name="me"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsCopyOf(this EntityGraph me, EntityGraph other)
        {
            return me.IsCopyOf(other, Helpers.MemberwiseCompare);
        }
    }
}
