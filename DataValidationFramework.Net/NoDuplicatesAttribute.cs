using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    /// <inheritdoc />
    /// <summary>
    /// This validation attribute verifies that a collection does not contain duplicate elements
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NoDuplicatesAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of the NoDuplicatesValidator class.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        protected override ValidationRule<ValidationResult> Create(params ValidationRuleDependency[] signature)
        {
            var validator = new NoDuplicatesValidator(signature)
            {
                Message = string.Format(ErrorMessage, PropertyInfo.Name),
            };
            return validator;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// The actual implementation of the NoDuplicates validation.
    /// </summary>
    public class NoDuplicatesValidator : AttributeValidator
    {
        /// <summary>
        /// Gets or sets the error message of this validator.
        /// </summary>
        public string Message { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the NoDuplicatesValidator class using an array of ValidationRuleDependencies.
        /// </summary>
        /// <param name="signature"></param>
        public NoDuplicatesValidator(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the NoDuplicatesValidator class using a givenSignature.
        /// </summary>
        /// <param name="signature"></param>
        public NoDuplicatesValidator(Signature signature)
            : base(signature.ToArray())
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Validation method for the class.
        /// </summary>
        /// <param name="value"></param>
        public override ValidationResult Validate(object value)
        {
            var collection = (IEnumerable) value;
            if (collection == null)
            {
                return ValidationResult.Success;
            }

            var list = new List<object>();
            foreach (var element in collection)
            {
                if (list.Contains(element))
                {
                    return new ValidationResult(Message);
                }

                list.Add(element);
            }

            return ValidationResult.Success;
        }
    }
}

