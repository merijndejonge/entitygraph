using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace OpenSoftware.EntityGraphCore
{
    public class EntityRelation<TNType>
    {
        public EntityRelation()
        {
            SingleEdges = new Dictionary<PropertyInfo, TNType>();
            ListEdges = new Dictionary<PropertyInfo, List<TNType>>();
        }

        public TNType Node { get; set; }
        public Dictionary<PropertyInfo, TNType> SingleEdges { get; set; }
        public Dictionary<PropertyInfo, List<TNType>> ListEdges { get; set; }
    }

    public class EntityRelationGraph<TNType> : IEnumerable<TNType>
    {
        public EntityRelationGraph()
        {
            Nodes = new List<EntityRelation<TNType>>();
        }

        public List<EntityRelation<TNType>> Nodes { get; }

        public IEnumerator<TNType> GetEnumerator()
        {
            foreach (var node in Nodes)
            {
                yield return node.Node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}