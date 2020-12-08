namespace CoreMVVM.Validation
{
    /// <summary>
    /// The result of a validation.
    /// </summary>
    public class ValidationResult
    {
        #region Constructors

        private ValidationResult(string propertyName, bool isSuccess, string? errorMessage)
        {
            PropertyName = propertyName;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult Success(string propertyName) => new(propertyName, true, null);

        public static ValidationResult Fail(string propertyName, string errorMessage) => new(propertyName, false, errorMessage);

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the validated property.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets a value indicating if the validation passed.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the error message of this validation result. Null if validation was successful.
        /// </summary>
        public string? ErrorMessage { get; }

        #endregion Properties
    }
}