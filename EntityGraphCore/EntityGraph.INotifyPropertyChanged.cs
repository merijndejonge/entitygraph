using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace OpenSoftware.EntityGraphCore
{
    // ReSharper disable once UnusedTypeParameter
    public partial class EntityGraph<TEntity> : INotifyPropertyChanged
    {
        /// <summary>
        /// handler to receive property changed events.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            PropertyChanged?.Invoke(this, eventArgs);
        }

        [Initialize]
        internal void SetupINotifyPropertyChanged()
        {
            EntityRelationGraphResetting += (sender, args) => RemoveNotifyPropertyChangedHandlers();
            EntityRelationGraphResetted += (sender, args) => SetupNotifyPropertyChangedHandlers();
            SetupNotifyPropertyChangedHandlers();
        }

        [Dispose]
        internal void DisposeINotifyPropertyChanged()
        {
            RemoveNotifyPropertyChangedHandlers();
        }

        private void SetupNotifyPropertyChangedHandlers()
        {
            foreach (var node in EntityRelationGraph.OfType<INotifyPropertyChanged>())
            {
                node.PropertyChanged += Node_PropertyChanged;
            }
        }

        private void RemoveNotifyPropertyChangedHandlers()
        {
            foreach (var node in EntityRelationGraph.OfType<INotifyPropertyChanged>())
            {
                node.PropertyChanged -= Node_PropertyChanged;
            }
        }

        void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propInfo = sender.GetType().GetProperty(e.PropertyName);
            if (GraphShape.IsEdge(propInfo))
            {
                EntityRelationGraphReset();
            }

            PropertyChanged?.Invoke(sender, e);
        }
    }
}