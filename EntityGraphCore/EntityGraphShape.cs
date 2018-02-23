using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenSoftware.EntityGraphCore
{
    public class EntityGraphEdge
    {
        public Type FromType { get; set; }
        private PropertyInfo _edgeInfo;

        public PropertyInfo EdgeInfo
        {
            get => _edgeInfo;
            set
            {
                if (_edgeInfo != value)
                {
                    _edgeInfo = value;
                    if (typeof(IEnumerable).IsAssignableFrom(_edgeInfo.PropertyType) &&
                        _edgeInfo.PropertyType.IsGenericType)
                    {
                        ToType = _edgeInfo.PropertyType.GetGenericArguments()[0];
                    }
                    else
                    {
                        ToType = _edgeInfo.PropertyType;
                    }
                }
            }
        }

        public Type ToType { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}->{1}", FromType.Name, ToType.Name);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj is EntityGraphEdge == false)
            {
                return false;
            }

            var edge = (EntityGraphEdge) obj;
            return FromType == edge.FromType && EdgeInfo == edge.EdgeInfo;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (FromType != null)
            {
                hashCode ^= FromType.GetHashCode();
            }

            if (EdgeInfo != null)
            {
                hashCode ^= EdgeInfo.GetHashCode();
            }

            return hashCode;
        }
    }

    public class EntityGraphShape : IEntityGraphShape, IEnumerable<EntityGraphEdge>
    {
        public delegate TTo EdgeType<in TFrom, out TTo>(TFrom from);

        public delegate IEnumerable<TTo> EdgeEnumType<in TFrom, out TTo>(TFrom from);

        internal List<EntityGraphEdge> Edges = new List<EntityGraphEdge>();

        public EntityGraphShape Edge<TLhs, TRhs>(Expression<EdgeType<TLhs, TRhs>> edge)
        {
            var entityType = edge.Parameters.Single().Type;
            if (edge.Body is MemberExpression == false)
            {
                var msg = string.Format("Edge expression '{0}' is invalid: it should have the form 'A => A.B'",
                    edge.ToString());
                throw new Exception(msg);
            }

            var mexpr = (MemberExpression) edge.Body;
            if (mexpr.Expression is ParameterExpression == false)
            {
                var msg = string.Format("Edge expression '{0}' is invalid: it should have the form 'A => A.B'",
                    edge.ToString());
                throw new Exception(msg);
            }

            var propInfo = mexpr.Member as PropertyInfo;
            if (entityType != null && propInfo != null)
            {
                Edges.Add(new EntityGraphEdge {FromType = entityType, EdgeInfo = propInfo});
            }

            return this;
        }

        // We can't use TEntity as the return type of EdgeEnumType, because IEnumerable<T> is not 
        // covariant in Silverlight!!. Therefore a second type parameter TRHS is needed.
        public EntityGraphShape Edge<TLhs, TRhs>(Expression<EdgeEnumType<TLhs, TRhs>> edge)
        {
            var entityType = edge.Parameters.Single().Type;
            Expression body;
            if (edge.Body is UnaryExpression expression)
            {
                if (expression.NodeType == ExpressionType.Convert)
                {
                    body = expression.Operand;
                }
                else
                {
                    var msg = string.Format(
                        "Edge expression '{0}' is invalid: the lamda expression has an unsupported format.",
                        edge.ToString());
                    throw new Exception(msg);
                }
            }
            else
            {
                body = edge.Body;
            }

            if (body is MemberExpression == false)
            {
                var msg = string.Format("Edge expression '{0}' is invalid: it should have the form 'A => A.B'",
                    edge.ToString());
                throw new Exception(msg);
            }

            var mexpr = (MemberExpression) body;
            if (mexpr.Expression is ParameterExpression == false)
            {
                var msg = string.Format("Edge expression '{0}' is invalid: it should have the form 'A => A.B'",
                    edge.ToString());
                throw new Exception(msg);
            }

            var propInfo = mexpr.Member as PropertyInfo;
            if (entityType != null && propInfo != null)
            {
                Edges.Add(new EntityGraphEdge {FromType = entityType, EdgeInfo = propInfo});
            }

            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns an IEnumerable that iterates over the out edges of the given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<PropertyInfo> OutEdges(object entity)
        {
            if (entity == null)
            {
                return new List<PropertyInfo>();
            }

            var entityType = entity.GetType();
            return this.Where(edge => edge.FromType.IsAssignableFrom(entityType)).Select(edge => edge.EdgeInfo)
                .Distinct();
        }

        public IEnumerator<EntityGraphEdge> GetEnumerator()
        {
            return Edges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        /// <summary>
        /// Indicates of the given property info represents an edge in this graph shape object.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool IsEdge(PropertyInfo edge)
        {
            return Edges.Any(e =>
                e.EdgeInfo.Name == edge.Name && e.EdgeInfo.PropertyType.IsAssignableFrom(edge.PropertyType));
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
        /// Returns the collection of objects that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public virtual IEnumerable GetNodes(object entity, PropertyInfo edge)
        {
            var nodes = (IEnumerable) edge.GetValue(entity, null);
            return nodes ?? new List<object>();
        }
    }
}