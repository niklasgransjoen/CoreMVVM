using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// For dynamically creating datatemplates for views, and registrating them with the current application.
    /// </summary>
    public static class DataTemplateRegistrator
    {
#if NET45
        private static readonly string[] _empty = new string[0];
#endif

        private static readonly HashSet<Type> _resolvedViewModels = new();

        /// <summary>
        /// Creates a datatemplate for the given view/viewmodel, if one has not already been created.
        /// </summary>
        public static void AssureDataTemplateExists(Type viewModelType, Type viewType)
        {
            if (viewModelType is null) throw new ArgumentNullException(nameof(viewModelType));
            if (viewType is null) throw new ArgumentNullException(nameof(viewType));

            if (_resolvedViewModels.Contains(viewModelType))
                return;

            DataTemplate template = CreateTemplate(viewModelType, viewType);

            // Add datatemplate to global resource pool.
            Application.Current.Resources.Add(template.DataTemplateKey, template);
            _resolvedViewModels.Add(viewModelType);
        }

        /// <summary>
        /// Creates the datatemplate by writing and parsing xaml.
        /// </summary>
        private static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            string xaml = string.Format(CultureInfo.InvariantCulture, xamlTemplate, viewModelType.Name, viewType.Name);

            var context = new ParserContext
            {
#if !NET45
                XamlTypeMapper = new(Array.Empty<string>())
#else
                XamlTypeMapper = new(_empty)

#endif
            };
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("vm", "vm");
            context.XmlnsDictionary.Add("v", "v");

            return (DataTemplate)XamlReader.Parse(xaml, context);
        }
    }
}