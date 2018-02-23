using System.Collections.Specialized;

namespace OpenSoftware.EntityGraphCore
{
    // ReSharper disable once UnusedTypeParameter
    public partial class EntityGraph<TEntity> : INotifyCollectionChanged
    {
        [Initialize]
        internal void SetupINotifyCollectionChanged()
        {
            EntityRelationGraphResetting += (sender, args) => RemoveNotifyCollectionChangedHandlers();
            EntityRelationGraphResetted += (sender, args) =>
            {
                SetupNotifyCollectionChangedHandlers();
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            };
            SetupNotifyCollectionChangedHandlers();
        }

        [Dispose]
        internal void DisposeINotifyCollectionChanged()
        {
            RemoveNotifyCollectionChangedHandlers();
        }

        private void SetupNotifyCollectionChangedHandlers()
        {
            foreach (var node in EntityRelationGraph.Nodes)
            {
                foreach (var list in node.ListEdges)
                {
                    if (!typeof(INotifyCollectionChanged).IsAssignableFrom(list.Key.PropertyType)) continue;
                    var collection = (INotifyCollectionChanged) list.Key.GetValue(node.Node, null);
                    collection.CollectionChanged += Collection_CollectionChanged;
                }
            }
        }

        private void RemoveNotifyCollectionChangedHandlers()
        {
            foreach (var node in EntityRelationGraph.Nodes)
            {
                foreach (var list in node.ListEdges)
                {
                    if (!typeof(INotifyCollectionChanged).IsAssignableFrom(list.Key.PropertyType)) continue;
                    var collection = (INotifyCollectionChanged) list.Key.GetValue(node.Node, null);
                    collection.CollectionChanged -= Collection_CollectionChanged;
                }
            }
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EntityRelationGraphReset();
            CollectionChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Handler to receive collection changed events
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
