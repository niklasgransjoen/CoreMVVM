using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace CoreMVVM.Windows.Markup
{
    /// <summary>
    /// Markup extension for using the <see cref="IResourceService"/> through XAML.
    /// </summary>
    public sealed class StringResource : MarkupExtension
    {
        private static readonly StringResourceBinder _binder = new StringResourceBinder();

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

            public StringResourceBinder()
            {
                if (DesignHelper.IsDesignMode)
                    return;

                _resourceService = ContainerProvider.ResolveRequiredService<IResourceService>();
                _resourceService.OnCurrentCultureChanged += OnCurrentCultureChanged;
            }

            public string this[string key]
            {
                get
                {
                    if (DesignHelper.IsDesignMode)
                        return $"${{res:{key}}}";

                    string resource = _resourceService.GetString(key);
                    return resource ?? $"${{res:{key}}}";
                }
            }

            private void OnCurrentCultureChanged(object sender, EventArgs e)
            {
                RaisePropertyChanged(Binding.IndexerName);
            }
        }
    }
}