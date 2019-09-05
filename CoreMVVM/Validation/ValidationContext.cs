using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreMVVM.Validation
{
    /// <summary>
    /// Provides context to the validator.
    /// </summary>
    public class ValidationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="validationModel">A reference to the instance to validate.</param>
        /// <exception cref="ArgumentNullException">validationModel is null.</exception>
        public ValidationContext(object validationModel)
        {
            ValidationModel = validationModel ?? throw new ArgumentNullException(nameof(validationModel));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="validationModel">A reference to the instance to validate.</param>
        /// <param name="propertyName">The name of the property to validate. Can be null.</param>
        /// <exception cref="ArgumentNullException">validationModel is null.</exception>
        public ValidationContext(object validationModel, string propertyName)
        {
            ValidationModel = validationModel ?? throw new ArgumentNullException(nameof(validationModel));
            PropertyName = propertyName;
        }

        #region Properties

        /// <summary>
        /// Gets the model to perform validation upon.
        /// </summary>
        public object ValidationModel { get; }

        /// <summary>
        /// Gets the name of the property to validate. Null if all properties should be validated.
        /// </summary>
        public string PropertyName { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a sequence of all validatable properties of the instance.
        /// </summary>
        public IList<ValidatableProperty> GetProperties()
        {
            IEnumerable<PropertyInfo> properties = ValidationModel.GetType().GetProperties();
            if (!string.IsNullOrEmpty(PropertyName))
                properties = properties.Where(p => p.Name == PropertyName);

            List<ValidatableProperty> result = new List<ValidatableProperty>();
            foreach (PropertyInfo property in properties)
            {
                IEnumerable<Attribute> attributes = property.GetCustomAttributes();
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is ValidationAttribute validationAttribute)
                    {
                        result.Add(new ValidatableProperty(property, validationAttribute));
                    }
                }
            }

            return result;
        }

        #endregion Methods
    }
}