using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CoreMVVM.Validation
{
    /// <summary>
    /// A base class for ViewModels that require validation. Use <see cref="BaseModel"/> instead if no validation is required.
    /// </summary>
    public abstract class BaseValidationModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Fields

        private readonly Dictionary<string, IEnumerable<string>> _errors = new Dictionary<string, IEnumerable<string>>();

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when the value of a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs when the collection of validation errors changes.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        #endregion Events

        #region Public properties

        /// <summary>
        /// Gets a value indicating if this validation model has any validation errors.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        #endregion Public properties

        #region Public methods

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="string.Empty"/>,
        /// to retrieve entity-level errors.</param>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.SelectMany(e => e.Value);

            if (_errors.TryGetValue(propertyName, out IEnumerable<string> errors))
                return errors;

            return Enumerable.Empty<string>();
        }

        #endregion Public methods

        #region Protected methods

        /// <summary>
        /// Compares a variable to a value. If different, the reference variable is assigned the value.
        /// Invokes <see cref="PropertyChanged"/> and validates the value if it was different.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="property">A reference to the field of the property.</param>
        /// <param name="value">The new value of the property.</param>
        /// <param name="propertyName">The name of the property. Leave as null when calling from the property's setter.</param>
        /// <returns>True if the property changed.</returns>
        /// <remarks>Uses the default EqualityComparer of the type of the property.</remarks>
        protected virtual bool SetProperty<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(property, value))
                return false;

            property = value;
            RaisePropertyChanged(propertyName);
            ValidateProperty(propertyName);
            return true;
        }

        /// <summary>
        /// Validates a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        protected void ValidateProperty(string? propertyName)
        {
            ValidationContext context = new ValidationContext(this, propertyName);
            ValidationResult[] results = Validator.Validate(context);

            var groupedResult = results.GroupBy(r => r.PropertyName);
            foreach (var result in groupedResult)
            {
                var validationErrors = result.Where(r => !r.IsSuccess).ToArray();
                if (validationErrors.Length == 0)
                {
                    // There were no errors
                    if (_errors.Remove(result.Key))
                        RaiseErrorsChanged(result.Key);

                    continue;
                }

                // There were errors
                var errorMessages = validationErrors.Select(r => r.ErrorMessage!).ToList().AsReadOnly();
                if (!_errors.ContainsKey(result.Key))
                {
                    // state went from no error to errors
                    _errors[result.Key] = errorMessages;
                    RaiseErrorsChanged(result.Key);
                }
                else if (_errors.TryGetValue(result.Key, out var previousErrors))
                {
                    // Skip if error messages hasn't changed.
                    if (previousErrors.SequenceEqual(errorMessages))
                        continue;

                    _errors[result.Key] = errorMessages;
                    RaiseErrorsChanged(result.Key);
                }
            }
        }

        /// <summary>
        /// Validates all properties.
        /// </summary>
        protected void ValidateAllProperties() => ValidateProperty(null);

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event on a property.
        /// </summary>
        /// <param name="name">The name of the property to invoke the event on.</param>
        protected void RaisePropertyChanged(string? name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event on the calling member (should be a property).
        /// </summary>
        protected void RaiseThisPropertyChanged([CallerMemberName] string? propertyName = null) => RaisePropertyChanged(propertyName);

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event on all properties.
        /// </summary>
        protected void RaiseAllPropertiesChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

        /// <summary>
        /// Invokes the <see cref="ErrorsChanged"/> event on a property.
        /// </summary>
        /// <param name="name">The name of the property to invoke the event on.</param>
        protected void RaiseErrorsChanged(string name) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(name));

        #endregion Protected methods
    }
}