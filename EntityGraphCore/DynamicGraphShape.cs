using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenSoftware.EntityGraphCore
{
    /// <inheritdoc />
    /// <summary>
    /// This class implements a dynamic graph shape that uses a property filter for selecting edges.
    /// It can typically be used to define a full graph for an object graph where all edges can be
    /// identified in a similar way (e.g., because they all have the AssociationAttribute). Using 
    /// this EntityGraphShape prevents you from having to define all edges explicitely.
    /// </summary>
    public class DynamicGraphShape : IEntityGraphShape
    {
        private readonly Func<PropertyInfo, bool> _isEdgeFilter;

        /// <summary>
        /// Initializes a new instance of the DynamicGraphShape class with given property filter.
        /// </summary>
        /// <param name="isEdgeFilter"></param>
        public DynamicGraphShape(Func<PropertyInfo, bool> isEdgeFilter)
        {
            _isEdgeFilter = isEdgeFilter;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns an IEnumerable that iterates over the out edges of the given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<PropertyInfo> OutEdges(object entity)
        {
            return entity.GetType().GetProperties().Where(_isEdgeFilter);
        }

        /// <inheritdoc />
        /// <summary>
        /// Indicates of the given property info represents an edge in this graph shape object.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool IsEdge(PropertyInfo edge)
        {
            return _isEdgeFilter(edge);
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the object that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public object GetNode(object entity, PropertyInfo edge)
        {
            return edge.GetValue(entity, null);
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the collection of objects that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public IEnumerable GetNodes(object entity, PropertyInfo edge)
        {
            var nodes = (IEnumerable) edge.GetValue(entity, null);
            return nodes ?? new List<object>();
        }
    }
}