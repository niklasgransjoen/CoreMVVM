using System;
using System.Globalization;

namespace CoreMVVM.Demo
{
    /// <summary>
    /// Simple implementation of <see cref="IResourceService"/>.
    /// </summary>
    /// <remarks>
    /// Can easily be made more complex with more functionality.
    /// A normal implementation is to have a "Add/Register ResourceManager" method,
    /// and then iterate through a collection of them when finding a string.
    /// </remarks>
    public sealed class ResourceManager : IResourceService
    {
        public event EventHandler OnCurrentCultureChanged;

        private CultureInfo _cultureInfo = CultureInfo.CurrentUICulture;

        public CultureInfo CurrentCulture
        {
            get => _cultureInfo;
            set
            {
                if (_cultureInfo != value)
                {
                    _cultureInfo = value;
                    OnCurrentCultureChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string GetString(string key)
        {
            return Resources.Resources.ResourceManager.GetString(key, _cultureInfo);
        }
    }
}