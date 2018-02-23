using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenSoftware.EntityGraphCore
{
    public class EntityGraphAttributeShape : IEntityGraphShape
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the EntityGraphAttributeShape class. 
        /// </summary>
        public EntityGraphAttributeShape() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntityGraphAttributeShape class. 
        /// </summary>
        /// <param name="name"></param>
        public EntityGraphAttributeShape(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of this entity graph shape.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        /// <summary>
        /// Returns an IEnumerable of PropertyInfo objects for properties which have the "RiaServicesContrib.EntityGraph" attribute. 
        /// If Name is not null, the name of the entity graph attribute shape should match the name of the attribute.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<PropertyInfo> OutEdges(object entity)
        {
            bool Match(EntityGraphAttribute entityGraph) =>
                entityGraph != null && (Name == null || Name == entityGraph.Name);

            const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in entity.GetType().GetProperties(bindingAttr)
                where p.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(Match)
                select p;
            return qry;
        }

        /// <inheritdoc />
        /// <summary>
        /// Indicates of the given property info represents an edge in this graph shape object.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool IsEdge(PropertyInfo edge)
        {
            bool Match(EntityGraphAttribute entityGraph) =>
                entityGraph != null && (Name == null || Name == entityGraph.Name);

            return edge.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(Match);
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the object that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public virtual object GetNode(object entity, PropertyInfo edge)
        {
            return edge.GetValue(entity, null);
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the collection og objects that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public virtual IEnumerable GetNodes(object entity, PropertyInfo edge)
        {
            return (IEnumerable) edge.GetValue(entity, null);
        }
    }
}