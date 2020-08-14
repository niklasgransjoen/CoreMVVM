using System;
using System.Globalization;

namespace CoreMVVM.FallbackImplementations
{
    /// <summary>
    /// Empty implementation of <see cref="IResourceService"/>.
    /// </summary>
    public sealed class FallbackResourceService : IResourceService
    {
        private CultureInfo _cultureInfo = CultureInfo.CurrentUICulture;

        public event EventHandler? OnCurrentCultureChanged;

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

        public string? GetString(string key) => null;
    }
}