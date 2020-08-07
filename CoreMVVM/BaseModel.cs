using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoreMVVM
{
    /// <summary>
    /// A base class for ViewModels.
    /// </summary>
    public abstract class BaseModel : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Occurs when the value of a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Events

        #region Protected methods

        /// <summary>
        /// Compares a variable to a value. If different, the reference variable is assigned the value.
        /// Invokes <see cref="PropertyChanged"/> if the value was different.
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
            return true;
        }

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

        #endregion Protected methods
    }
}