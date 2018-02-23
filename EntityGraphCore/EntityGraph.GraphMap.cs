using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenSoftware.EntityGraphCore
{
    public partial class EntityGraph<TEntity>
    {
        /// <summary>
        /// Method that implements a generic traversal over an entity graph (defined by 
        /// associations marked with an entity graph attibute and applies 'action' to each visited node.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TEntity GraphMap(Func<TEntity, TEntity> action)
        {
            var nodeMap = new Dictionary<TEntity, TEntity>();

            nodeMap = EntityRelationGraph.Nodes.Aggregate(nodeMap, (nm, graphNode) =>
                {
                    nm.Add(graphNode.Node, action(graphNode.Node));
                    return nm;
                }
            );
            BuildEntityGraph(nodeMap, EntityRelationGraph);
            return nodeMap[Source];
        }

        /// <summary>
        /// (Re-)builds the associations between the nodes of the graph.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="graph"></param>
        private static void BuildEntityGraph(IReadOnlyDictionary<TEntity, TEntity> nodes,
            EntityRelationGraph<TEntity> graph)
        {
            foreach (var n in graph.Nodes)
            {
                var newEntity = nodes[n.Node];
                foreach (var association in n.SingleEdges.Keys)
                {
                    var oldAssociationEntity = n.SingleEdges[association];
                    var newAssociationEntity = nodes[oldAssociationEntity];
                    association.SetValue(newEntity, newAssociationEntity, null);
                }

                foreach (var association in n.ListEdges.Keys)
                {
                    var assocList = (IEnumerable) association.GetValue(newEntity, null);
                    if (assocList == null) continue;
                    var assocListType = assocList.GetType();
                    var addMethod = assocListType.GetMethod("Add");

                    foreach (var oldAssociationEntity in n.ListEdges[association])
                    {
                        var newAssociationEntity = nodes[oldAssociationEntity];
                        addMethod.Invoke(assocList, new object[] {newAssociationEntity});
                    }
                }
            }
        }
    }
}