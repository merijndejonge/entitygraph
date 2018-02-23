using System;
using System.Collections.Generic;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <inheritdoc />
    /// <summary>
    /// Class that is used to notify that the result of a validation rule has changed. 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class ValidationResultChangedEventArgs<TEntity, TResult> : EventArgs
    {
        /// <summary>
        /// gets the new result of the validation rule.
        /// </summary>
        public TResult ValidationResult { get; }

        /// <summary>
        /// Gets all the entities that are involved by the validation rule represented
        /// by this instance of ValidationResultChangedEventArgs.
        /// </summary>
        public IEnumerable<TEntity> Entities { get; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the ValidationResultChangedEventArgs class
        /// </summary>
        /// <param name="validationResult"></param>
        /// <param name="entities"></param>
        internal ValidationResultChangedEventArgs(TResult validationResult, IEnumerable<TEntity> entities)
        {
            ValidationResult = validationResult;
            Entities = entities;
        }
    }
}