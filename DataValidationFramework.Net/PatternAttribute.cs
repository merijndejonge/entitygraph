using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    /// <inheritdoc />
    /// <summary>
    /// This class is similar to System.ComponentModel.DataAnnotations.RegularExpressionAttribute.
    /// This attribute checks if a property value matches the provided regular expression .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PatternAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the PatternAttribute class.
        /// </summary>
        /// <param name="pattern"></param>
        public PatternAttribute(string pattern)
        {
            Pattern = pattern;
        }

        /// <summary>
        /// Gets the regular expression pattern of this RegularExpressionAttribute instance.
        /// </summary>
        public string Pattern { get; }

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of the PatternValidator class.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        protected override ValidationRule<ValidationResult> Create(params ValidationRuleDependency[] signature)
        {
            var validator = new PatternValidator(signature)
            {
                Message = string.Format(ErrorMessage, PropertyInfo.Name),
                Pattern = Pattern
            };
            return validator;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// The actual implementation of the PatternValidator validation.
    /// </summary>
    public class PatternValidator : AttributeValidator
    {
        /// <summary>
        /// Gets or sets the error message of this validator.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the pattern of the validator.
        /// </summary>
        public string Pattern
        {
            get => _pattern;
            internal set
            {
                if (_pattern == value) return;
                _pattern = value;
                _regex = new Regex(value);
            }
        }

        private string _pattern;

        private Regex _regex;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the PatternValidator class using an array of ValidationRuleDependencies.
        /// </summary>
        /// <param name="signature"></param>
        public PatternValidator(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the PatternValidator class using a givenSignature.
        /// </summary>
        /// <param name="signature"></param>
        public PatternValidator(Signature signature)
            : base(signature.ToArray())
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// The validation method.
        /// </summary>
        /// <param name="value"></param>
        public override ValidationResult Validate(object value)
        {
            if (!(value is string input)) return ValidationResult.Success;
            var m = _regex.Match(input);
            return m.Length != input.Length ? new ValidationResult(Message) : ValidationResult.Success;
        }
    }
}