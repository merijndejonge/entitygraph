using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    public class ValidationEngine : ValidationEngine<object, ValidationResult>
    {
        protected override bool HasValidationResult(object entity, string[] membersInError,
            ValidationResult validationResult)
        {
            if (validationResult == ValidationResult.Success)
                return false;
            var errorInfo = Entity(entity);
            if (errorInfo.HasErrors == false) return false;

            var validationError = errorInfo.Errors.SingleOrDefault(ve =>
                ve.ErrorMessage == validationResult.ErrorMessage && ve.MemberNames.SequenceEqual(membersInError));
            return validationError != null;
        }

        protected override void ClearValidationResult(object entity, string[] membersInError,
            ValidationResult validationResult)
        {
            if (validationResult == ValidationResult.Success) return;
            var errorInfo = Entity(entity);
            var validationError = errorInfo.Errors.SingleOrDefault(ve =>
                ve.ErrorMessage == validationResult.ErrorMessage && ve.MemberNames.SequenceEqual(membersInError));
            if (validationError != null)
            {
                errorInfo.Errors.Remove(validationError);
            }
        }

        protected override void SetValidationResult(object entity, string[] membersInError,
            ValidationResult validationResult)
        {
            if (validationResult == ValidationResult.Success) return;
            var errorInfo = Entity(entity);
            var vResult = new ValidationResult(validationResult.ErrorMessage, membersInError);
            errorInfo.Errors.Add(vResult);
        }

        protected override bool IsValidationSuccess(ValidationResult validationResult)
        {
            return validationResult == ValidationResult.Success;
        }
    }
}