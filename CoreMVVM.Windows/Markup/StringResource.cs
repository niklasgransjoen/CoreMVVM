using CoreMVVM.IOC;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CoreMVVM.Windows.Markup
{
    /// <summary>
    /// Markup extension for using the <see cref="IResourceService"/> through XAML.
    /// </summary>
    public sealed class StringResource : MarkupExtension
    {
        private static StringResourceBinder? _binder;

        public StringResource()
        {
        }

        public StringResource(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        /// <summary>
        /// Gets or sets the resource key to lookup.
        /// </summary>
        [ConstructorArgument("key")]
        public string Key { get; set; } = string.Empty;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (DesignHelper.IsDesignMode)
                return $"${{res:{Key}}}";

            if (_binder is null)
            {
                var valueTargetProvider = serviceProvider.ResolveRequiredService<IProvideValueTarget>();
                if (!(valueTargetProvider.TargetObject is DependencyObject dependencyObject))
                    return $"${{res:{Key}}}";

                var lifetimeScope = ControlServiceProvider.RequireServiceProvider(dependencyObject);
                _binder = lifetimeScope.ResolveRequiredService<StringResourceBinder>();
            }

            Binding binding = new Binding($"[{Key}]")
            {
                Mode = BindingMode.OneWay,
                Source = _binder,
            };

            return binding.ProvideValue(serviceProvider);
        }

        private sealed class StringResourceBinder : BaseModel
        {
            private readonly IResourceService _resourceService;

            public StringResourceBinder(IResourceService resourceService)
            {
                _resourceService = resourceService;
                _resourceService.OnCurrentCultureChanged += OnCurrentCultureChanged;
            }

            public string this[string key]
            {
                get
                {
                    string? resource = _resourceService.GetString(key);
                    return resource ?? $"${{res:{key}}}";
                }
            }

            private void OnCurrentCultureChanged(object? sender, EventArgs e)
            {
                RaisePropertyChanged(Binding.IndexerName);
            }
        }
    }
}