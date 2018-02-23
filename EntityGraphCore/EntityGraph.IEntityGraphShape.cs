using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace OpenSoftware.EntityGraphCore
{
    /// <summary>
    /// Interface with methods to define the shape of an entity graph by means of its edges.
    /// </summary>
    public interface IEntityGraphShape
    {
        /// <summary>
        /// Returns an IEnumerable that iterates over the out edges of the given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IEnumerable<PropertyInfo> OutEdges(object entity);

        /// <summary>
        /// Indicates of the given property info represents an edge in this graph shape object.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        bool IsEdge(PropertyInfo edge);

        /// <summary>
        /// Returns the object that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        object GetNode(object entity, PropertyInfo edge);

        /// <summary>
        /// Returns the collection of objects that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        IEnumerable GetNodes(object entity, PropertyInfo edge);
    }
}