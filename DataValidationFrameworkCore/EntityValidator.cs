using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Class that implements signature-based validation for single entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class EntityValidator<TEntity, TResult> : IDisposable
        where TEntity : class
        where TResult : class
    {
        private IValidationEngine<TEntity, TResult> _validator;
        public IEntityErrorInfo<TResult> Entity => Validator?.Entity(EntityToValidate);

        protected IValidationEngine<TEntity, TResult> Validator
        {
            get => _validator;
            set
            {
                if (_validator == value) return;
                if (_validator != null)
                {
                    ClearValidatorEventHandlers();
                }

                _validator = value;
                if (_validator != null)
                {
                    SetValidatorEventHandlers();
                }
            }
        }

        /// <summary>
        /// Gets or sets the entity this validator operates on.
        /// </summary>
        private TEntity EntityToValidate { get; }

        /// <summary>
        /// Initializes a new instance of the EntityValidator class.
        /// </summary>
        /// <param name="entityToValidate"></param>
        public EntityValidator(TEntity entityToValidate)
        {
            EntityToValidate = entityToValidate;
        }

        private void SetValidatorEventHandlers()
        {
            if (EntityToValidate is INotifyPropertyChanged changed)
            {
                changed.PropertyChanged += EntityValidator_PropertyChanged;
            }

            foreach (var propInfo in EntityToValidate.GetType().GetProperties())
            {
                if (!typeof(INotifyCollectionChanged).IsAssignableFrom(propInfo.PropertyType)) continue;
                if (propInfo.GetValue(EntityToValidate, null) is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged += EntityValidator_CollectionChanged;
                }
            }
        }

        private void ClearValidatorEventHandlers()
        {
            if (EntityToValidate is INotifyPropertyChanged changed)
            {
                changed.PropertyChanged -= EntityValidator_PropertyChanged;
            }

            foreach (var propInfo in EntityToValidate.GetType().GetProperties())
            {
                if (!typeof(INotifyCollectionChanged).IsAssignableFrom(propInfo.PropertyType)) continue;
                if (propInfo.GetValue(EntityToValidate, null) is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged -= EntityValidator_CollectionChanged;
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Disposes allocated resources.
        /// </summary>
        public void Dispose()
        {
            if (Validator == null) return;
            Validator.Dispose();
            Validator = null;
        }

        /// <summary>
        /// Callback method that is called when a CollectionChanged event is received from the entity.
        /// We synthesize what the corresponding entity property is and then call the validate method 
        /// of the validation engine for the entity and the property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntityValidator_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var senderType = sender.GetType();
            foreach (var propInfo in EntityToValidate.GetType().GetProperties())
            {
                if (!propInfo.PropertyType.IsAssignableFrom(senderType)) continue;
                if (propInfo.GetValue(EntityToValidate, null) != sender) continue;
                Validator.Validate(EntityToValidate, propInfo.Name);
                return;
            }
        }

        /// <summary>
        /// Callback method that is called when a PropertyChanged event is received from the entity.
        /// We call the Validate method of the validation engine for the object and property name of the 
        /// changed property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntityValidator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Validator.Validate((TEntity) sender, e.PropertyName);
        }
    }
}
