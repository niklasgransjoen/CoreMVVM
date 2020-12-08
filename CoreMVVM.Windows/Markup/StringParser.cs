using CoreMVVM.IOC;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CoreMVVM.Windows.Markup
{
    /// <summary>
    /// Markup extension for using the <see cref="CoreMVVM.IStringParser"/> through XAML.
    /// </summary>
    public sealed class StringParser : MarkupExtension
    {
        private static StringParserBinder? _binder;

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

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (DesignHelper.IsDesignMode)
                return $"!{Value}!";

            if (_binder is null)
            {
                var valueTargetProvider = serviceProvider.ResolveRequiredService<IProvideValueTarget>();
                if (valueTargetProvider.TargetObject is not DependencyObject dependencyObject)
                    return $"!{Value}!";

                var lifetimeScope = dependencyObject.RequireServiceProvider();
                _binder = lifetimeScope.ResolveRequiredService<StringParserBinder>();
            }

            Binding binding = new Binding($"[{Value}]")
            {
                Mode = BindingMode.OneWay,
                Source = _binder,
            };

            return binding.ProvideValue(serviceProvider);
        }

        private sealed class StringParserBinder : BaseModel
        {
            private readonly IStringParser _stringParser;
            private readonly IResourceService _resourceService;

            public StringParserBinder(IStringParser stringParser, IResourceService resourceService)
            {
                _stringParser = stringParser;
                _resourceService = resourceService;
                _resourceService.OnCurrentCultureChanged += OnCurrentCultureChanged;
            }

            public string this[string value] => _stringParser.Parse(value);

            private void OnCurrentCultureChanged(object? sender, EventArgs e)
            {
                RaisePropertyChanged(Binding.IndexerName);
            }
        }
    }
}