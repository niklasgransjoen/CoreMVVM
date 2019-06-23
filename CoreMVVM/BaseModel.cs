using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoreMVVM
{
    /// <summary>
    /// A base class for models and viewmodels.
    /// </summary>
    public abstract class BaseModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Protected methods

        /// <summary>
        /// Sets the given property, and invokes <see cref="RaisePropertyChanged(string)"/> on it if it's changed.
        /// <para>Returns true if the property was changed.</para>
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="property">A reference to the property to set.</param>
        /// <param name="value">The new value of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual bool SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
                return false;

            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the given property, and invokes <see cref="RaisePropertyChanged(string)"/> on it if it's changed.
        /// <para>Returns true if the property was changed.</para>
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="property">A reference to the property to set.</param>
        /// <param name="value">The new value of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="additionalProperties">The names of additional properties to call <see cref="RaisePropertyChanged(string)"/> on.</param>
        /// <remarks>
        /// This method calls <see cref="SetProperty{T}(ref T, T, string)"/>, so any overrides to it will take effect even if properties call this one instead.
        /// </remarks>
        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null, params string[] additionalProperties)
        {
            bool result = SetProperty(ref property, value, propertyName);
            if (result)
            {
                foreach (string name in additionalProperties)
                    RaisePropertyChanged(name);
            }

            return result;
        }

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event on a property.
        /// </summary>
        /// <param name="name">The name of the property to raise the event on.</param>
        protected void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event on the calling member (should be a property).
        /// </summary>
        protected void RasieThisPropertyChanged([CallerMemberName] string propertyName = null) => RaisePropertyChanged(propertyName);

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event on all properties.
        /// </summary>
        protected void RaiseAllPropertiesChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

        #endregion Protected methods
    }
}