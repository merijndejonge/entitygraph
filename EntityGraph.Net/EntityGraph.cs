using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net
{
    public class EntityGraph : EntityGraph<object>
    {
        /// <inheritdoc />
        /// <summary>
        /// Extension method that returns an entity graph object, defined by the provided entity graph shape
        /// </summary>
        /// <param name="source"></param>
        /// <param name="shape"></param>
        public EntityGraph(object source, IEntityGraphShape shape) : base(source, shape)
        {
        }
    }
}
