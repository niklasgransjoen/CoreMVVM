using System;

namespace CoreMVVM.Validation
{
    /// <summary>
    /// Base class for validation attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ValidationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttribute"/> class.
        /// </summary>
        public ValidationAttribute()
        {
        }

        #region Properties

        /// <summary>
        /// Gets or sets the error message to show on a failed validation.
        /// </summary>
        /// <remarks>When this value is null, a default error message is used instead.</remarks>
        /// <value>Default is null.</value>
        protected virtual string ErrorMessage { get; set; }

        #endregion Properties

        /// <summary>
        /// Validates a value.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="propertyName">The name of the property holdign this value. Not null.</param>
        /// <returns>An instance holding information about the result of the validation.</returns>
        /// <exception cref="ArgumentNullException">propertyName is null.</exception>
        public ValidationResult Validate(object value, string propertyName)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));

            bool validationResult = IsValid(value);
            if (validationResult)
                return ValidationResult.Success(propertyName);
            else
            {
                string message = ErrorMessage;
                if (message is null)
                    message = $"Invalid value for property {propertyName}.";

                return ValidationResult.Fail(propertyName, message);
            }
        }

        /// <summary>
        /// Validates a value.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True is value passed validation.</returns>
        protected abstract bool IsValid(object value);
    }
}