using System;

namespace CoreMVVM.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ValidationAttribute : Attribute
    {
        public ValidationResult Validate(object value, string propertyName)
        {
            bool validationResult = IsValid(value);
            if (validationResult)
                return ValidationResult.Success(propertyName);
            else
                return ValidationResult.Fail(propertyName, $"Invalid value for property {propertyName}.");
        }

        protected abstract bool IsValid(object value);
    }
}