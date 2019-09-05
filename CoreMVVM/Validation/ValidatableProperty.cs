using System.Reflection;

namespace CoreMVVM.Validation
{
    /// <summary>
    /// A binding between a property and a validation property.
    /// </summary>
    public class ValidatableProperty
    {
        public ValidatableProperty(PropertyInfo propertyInfo, ValidationAttribute attribute)
        {
            PropertyInfo = propertyInfo;
            Attribute = attribute;
        }

        public PropertyInfo PropertyInfo { get; }
        public ValidationAttribute Attribute { get; }
    }
}