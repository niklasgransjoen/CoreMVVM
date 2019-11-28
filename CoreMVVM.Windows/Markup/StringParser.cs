using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace CoreMVVM.Windows.Markup
{
    /// <summary>
    /// Markup extension for using the <see cref="CoreMVVM.StringParser"/> through XAML.
    /// </summary>
    public sealed class StringParser : MarkupExtension
    {
        private static readonly StringParserBinder _binder = new StringParserBinder();

        public StringParser()
        {
        }

        public StringParser(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the string to parse.
        /// </summary>
        [ConstructorArgument("value")]
        public string Value { get; set; } = string.Empty;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding binding = new Binding($"[{Value}]")
            {
                Mode = BindingMode.OneWay,
                Source = _binder,
            };

            return binding.ProvideValue(serviceProvider);
        }

        private sealed class StringParserBinder : BaseModel
        {
            public StringParserBinder()
            {
                if (DesignHelper.IsDesignMode)
                    return;

                var resourceService = ContainerProvider.Resolve<IResourceService>();
                resourceService.OnCurrentCultureChanged += OnCurrentCultureChanged;
            }

            public string this[string key]
            {
                get
                {
                    if (DesignHelper.IsDesignMode)
                        return $"!{key}!";

                    return CoreMVVM.StringParser.Parse(key);
                }
            }

            private void OnCurrentCultureChanged(object sender, EventArgs e)
            {
                RaisePropertyChanged(Binding.IndexerName);
            }
        }
    }
}