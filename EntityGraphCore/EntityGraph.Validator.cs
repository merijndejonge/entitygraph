using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.EntityGraphCore
{
    public partial class EntityGraph<TEntity>
    {
        private IValidationEngine<TEntity, ValidationResult> _validator;

        public IValidationEngine<TEntity, ValidationResult> Validator
        {
            get => _validator;
            set
            {
                if (_validator == value) return;
                if (_validator != null)
                {
                    ClearValidatorEventHandlers();
                    _validator.ValidationRuleSetChanged -= _validator_ValidationRuleSetChanged;
                }

                _validator = value;
                if (_validator == null) return;
                SetValidatorEventHandlers();
                _validator.ValidationRuleSetChanged += _validator_ValidationRuleSetChanged;
                _validator.Validate(this);
            }
        }

        private void _validator_ValidationRuleSetChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _validator.Validate(this);
        }

        private void SetValidatorEventHandlers()
        {
            CollectionChanged += Validator_CollectionChanged;
            PropertyChanged += Validator_PropertyChanged;
        }

        private void ClearValidatorEventHandlers()
        {
            CollectionChanged -= Validator_CollectionChanged;
            PropertyChanged -= Validator_PropertyChanged;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        [Dispose]
        internal void DisposeGraphValidation()
        {
            if (Validator == null) return;
            Validator.Dispose();
            Validator = null;
        }

        /// <summary>
        /// Callback method that is called when a CollectionChanged event is received from the entity graph.
        /// We obtain the node and edge in the entity graph for this collection and then call the Validate
        /// method of the validation engine for them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Validator_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                Validator.Validate(this);
            }
            else
            {
                var senderType = sender.GetType();
                var collectionOwnerInfo = (
                    from node in EntityRelationGraph.Nodes
                    from edge in node.ListEdges
                    where
                        edge.Key.PropertyType.IsAssignableFrom(senderType) &&
                        edge.Key.GetValue(node.Node, null) == sender
                    select
                        new {Owner = node.Node, Edge = edge.Key}
                ).Single();
                Validator.Validate(collectionOwnerInfo.Owner, collectionOwnerInfo.Edge.Name, this);
            }
        }

        /// <summary>
        /// Callback method that is called when a PropertyChanged event is received from the entity graph.
        /// We call the Validate method of the validation engine for the object and property name of the 
        /// changed property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Validator_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            Validator.Validate(sender, args.PropertyName, this);
        }
    }
}