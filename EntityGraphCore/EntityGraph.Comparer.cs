using System;
using System.Linq;

namespace OpenSoftware.EntityGraphCore
{
    public partial class EntityGraph<TEntity>
    {
        /// <summary>
        /// Determines whether two entity graphs are equal using the given comparer function.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EntityGraphEqual(EntityGraph<TEntity> graph, Func<TEntity, TEntity, bool> comparer = null)
        {
            if (this.Count() != graph.Count())
                return false;
            if (comparer == null)
            {
                comparer = (e1, e2) => e1 == e2;
            }

            var zipList = this.Zip(graph, (e1, e2) => new {e1, e2});
            return zipList.All(zipElem => comparer(zipElem.e1, zipElem.e2));
        }
    }
}