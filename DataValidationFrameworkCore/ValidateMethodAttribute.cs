using System;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <inheritdoc />
    /// <summary>
    /// Attribute that marks a method as a validation method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateMethodAttribute : Attribute
    {
    }
}