namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Interface for attribute validators. They have a validate method with a fixed signature.
    /// </summary>
    public interface IAttributeValidator<out TResult>
    {
        /// <summary>
        /// Validation method for attribute validators.
        /// </summary>
        /// <param name="value"></param>
        TResult Validate(object value);
    }
}