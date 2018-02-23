using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenSoftware.EntityGraphCore
{
    public partial class EntityGraph<TEntity>
    {
        /// <summary>
        /// Method that makes a copy of an entitygraph by copying entities that are included in the entity graph.
        /// </summary>
        /// <returns></returns>
        public EntityGraph<TEntity> Copy(Func<TEntity, IEnumerable<PropertyInfo>> getDataMembers)
        {
            var copiedEntity = GraphMap(x => CopyDataMembers(x, getDataMembers));
            return new EntityGraph<TEntity>(copiedEntity, GraphShape);
        }

        private TCopy CopyDataMembers<TCopy>(TCopy source, Func<TEntity, IEnumerable<PropertyInfo>> getDataMembers)
            where TCopy : TEntity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete 
            // type of the entity to copy.
            var copy = (TCopy) Activator.CreateInstance(source.GetType());
            var dataMembers = getDataMembers(source);
            // Copy DataMember properties
            ApplyState(copy, source, dataMembers);
            return copy;
        }

        /// <summary>
        /// Copies the values form the properties in dataMembers from sourceEntity to targetEntity.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <param name="targetObject"></param>
        /// <param name="dataMembers"></param>
        private static void ApplyState(object targetObject, object sourceEntity, IEnumerable<PropertyInfo> dataMembers)
        {
            if (targetObject == null)
            {
                return;
            }

            // Copy DataMember properties            
            foreach (var currentPropertyInfo in dataMembers)
            {
                var currentObject = currentPropertyInfo.GetValue(sourceEntity, null);
                currentPropertyInfo.SetValue(targetObject, currentObject, null);
            }
        }
    }
}