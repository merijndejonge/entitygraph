using System.Collections.Generic;

namespace OpenSoftware.EntityGraphCore
{
    public partial class EntityGraph<TEntity> : IEnumerable<TEntity>
    {
        public IEnumerator<TEntity> GetEnumerator() {
            return EntityRelationGraph.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return EntityRelationGraph.GetEnumerator();
        }
    }
}
