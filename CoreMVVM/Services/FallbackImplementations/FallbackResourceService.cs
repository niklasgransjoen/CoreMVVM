using CoreMVVM.IOC;
using System;
using System.Globalization;

namespace CoreMVVM.FallbackImplementations
{
    [Scope(ComponentScope.Singleton)]
    public sealed class FallbackResourceService : IResourceService
    {
        private CultureInfo _cultureInfo = CultureInfo.CurrentUICulture;

        public event EventHandler OnCurrentCultureChanged;

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
            return null;
        }

#if NETCORE

        public string GetString(ReadOnlySpan<char> key)
        {
            return null;
        }

#endif
    }

    [Scope(ComponentScope.Singleton)]
    public sealed class FallbackResourceServiceProvider : IResourceServiceProvider
    {
        private readonly IResourceService _resourceService;

        public FallbackResourceServiceProvider(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public IResourceService GetResourceService()
        {
            return _resourceService;
        }

        public void FreeResourceService(IResourceService resourceService)
        {
        }
    }
}