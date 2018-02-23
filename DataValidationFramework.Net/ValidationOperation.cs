using System.ComponentModel.DataAnnotations;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Class that that represents an asynchronous validation operation.
    /// This is a WCF DomainServices.Client services specific instantiation of the 
    /// RiaServicesContrib.DataValidation.ValidationOperation class.
    /// </summary>
    public class ValidationOperation : ValidationOperation<ValidationResult>
    {
    }
}
