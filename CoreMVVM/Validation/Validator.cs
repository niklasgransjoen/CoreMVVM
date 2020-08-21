using System;
using System.Reflection;

namespace CoreMVVM.Validation
{
    /// <summary>
    /// For performing validation on the properties of an instance.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Performs validation with the given context.
        /// </summary>
        public static ValidationResult[] Validate(ValidationContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var properties = context.GetProperties();
            ValidationResult[] result = new ValidationResult[properties.Count];

            for (int i = 0; i < properties.Count; i++)
            {
                ValidatableProperty property = properties[i];

                PropertyInfo pInfo = property.PropertyInfo;
                object value = pInfo.GetValue(context.ValidationModel);

                result[i] = property.Attribute.Validate(value, pInfo.Name);
            }

            return result;
        }
    }
}