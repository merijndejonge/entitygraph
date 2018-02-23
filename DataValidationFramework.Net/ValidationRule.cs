using System.ComponentModel.DataAnnotations;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    public abstract class ValidationRule : ValidationRule<ValidationResult>
    {
        protected ValidationRule(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }
    }
}
