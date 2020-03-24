using System;
using System.Globalization;

namespace CoreMVVM
{
    /// <summary>
    /// A tool for parsing strings, performing resource lookup.
    /// </summary>
    public static class StringParser
    {
        private static readonly Lazy<IStringParser> _stringParser = ContainerProvider.Resolve<Lazy<IStringParser>>();

        public static IStringParser Core => _stringParser.Value;

        /// <summary>
        /// Utility method. Parser the given value, then formats it with the given arguments.
        /// </summary>
        public static string Format(string value, params object[] args)
        {
            using var resourceService = GetResourceService();
            return Core.Format(resourceService.Value, CultureInfo.CurrentCulture, value, args);
        }

        /// <summary>
        /// Utility method. Parser the given value, then formats it with the given arguments.
        /// </summary>
        public static string Format(IFormatProvider formatProvider, string value, params object[] args)
        {
            using var resourceService = GetResourceService();
            return Core.Format(resourceService.Value, formatProvider, value, args);
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public static string Parse(string value, params StringTagPair[] args)
        {
            using var resourceService = GetResourceService();
            return Core.Parse(resourceService.Value, value, args);
        }

        /// <summary>
        /// Evaluates the value of a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to evaluate.</param>
        /// <returns>The value that the property was evaluated to.</returns>
        public static string GetValue(string propertyName, params StringTagPair[] args)
        {
            using var resourceService = GetResourceService();
            return Core.GetValue(resourceService.Value, propertyName, args);
        }

        /// <summary>
        /// Utility method. Resolves the given resource, then parses it.
        /// </summary>
        /// <returns>The resolved, parsed resource. If resource does not exist, returns null.</returns>
        public static string GetResource(string key, params StringTagPair[] args)
        {
            using var resourceService = GetResourceService();
            return Core.GetResource(resourceService.Value, key, args);
        }

#if NETCORE

        public static string GetValue(ReadOnlySpan<char> propertyName, params StringTagPair[] args)
        {
            using var resourceService = GetResourceService();
            return Core.GetValue(resourceService.Value, propertyName, args);
        }

        /// <summary>
        /// Utility method. Resolves the given resource, then parses it.
        /// </summary>
        /// <returns>The resolved, parsed resource. If resource does not exist, returns null.</returns>
        public static string GetResource(ReadOnlySpan<char> key, params StringTagPair[] args)
        {
            using var resourceService = GetResourceService();
            return Core.GetResource(resourceService.Value, key, args);
        }

#endif

        #region Utilities

        private static ResourceService GetResourceService() => new ResourceService(null);

        private readonly struct ResourceService : IDisposable
        {
            private static readonly Lazy<IResourceServiceProvider> _resourceServiceProvider = ContainerProvider.Resolve<Lazy<IResourceServiceProvider>>();

            public ResourceService(object _)
            {
                Value = _resourceServiceProvider.Value.GetResourceService();
            }

            public IResourceService Value { get; }

            public void Dispose()
            {
                _resourceServiceProvider.Value.FreeResourceService(Value);
            }
        }

        #endregion Utilities
    }
}