using System;

namespace OpenSoftware.EntityGraphCore
{
    public partial class EntityGraph<TEntity>
    {
        /// <summary>
        /// Determines whether two entity graphs are copies of each other by member-wise comparing all entities in both graphs.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="memberwiseCompare"></param>
        /// <returns></returns>
        public bool IsCopyOf(EntityGraph<TEntity> graph, Func<TEntity, TEntity, bool> memberwiseCompare)
        {
            return EntityGraphEqual(graph, (e1, e2) => e1 != e2 && memberwiseCompare(e1, e2));
        }
    }
}