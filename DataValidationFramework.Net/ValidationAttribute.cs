using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Abstract base class that implements attribute-based validation.
    /// This is a WCF DomainServices.Client services-specific implementation of the 
    /// RiaServicesContrib.DataValidation.IValidationAttribute interface.
    /// Notes:
    /// This class derives from Attribute. Since we don't have multiple inheritance,
    /// the validation rule is associatied with this attribute using containement.
    /// Since attribute subclasses cannot be generic. 
    /// </summary>
    public abstract class ValidationAttribute : Attribute, IValidationAttribute<ValidationResult>
    {
        /// <summary>
        /// Gets or sets the error message string that will be used to report validation errors. 
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the property info object to which this attribute is attached.
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Creates a new Validation rule and binds it to the given property.
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public ValidationRule<ValidationResult> BindTo(PropertyInfo propInfo)
        {
            PropertyInfo = propInfo;
            var signature = CreateSignature(propInfo);
            return Create(signature);
        }

        /// <summary>
        /// Method that creates a new instance of a validation rule for given signature.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        protected abstract ValidationRule<ValidationResult> Create(params ValidationRuleDependency[] signature);

        /// <summary>
        /// Creates a signature with a single rule dependency expression of type 
        ///   Expression&lt;Func&lt;object,object>>:
        ///     A => ((t)A).name as object
        ///   
        /// where 
        ///  name equles propInfo.name
        ///  t equals propInfo.DeclaringType
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        private static ValidationRuleDependency CreateSignature(PropertyInfo propInfo)
        {
            var paramExpr = Expression.Parameter(typeof(object), "A");

            var castExpr = Expression.Convert(paramExpr, propInfo.DeclaringType);
            var memberExpr = Expression.Property(castExpr, propInfo);
            var typeAsExpr = Expression.TypeAs(memberExpr, typeof(object));
            var lambdaExpr = Expression.Lambda<Func<object, object>>(typeAsExpr, paramExpr);
            return new ValidationRuleDependency {Expression = lambdaExpr};
        }
    }
}
