using System;
using System.Globalization;

namespace CoreMVVM
{
    public static class IStringParserExtensions
    {
        /// <summary>
        /// Parser the given value, then formats it with the given arguments using the current culture.
        /// </summary>
        public static string Format(this IStringParser stringParser, string value, params object[] args)
        {
            if (stringParser is null)
                throw new ArgumentNullException(nameof(stringParser));

            return stringParser.Format(CultureInfo.CurrentCulture, value, args);
        }

        /// <summary>
        /// Resolves a resource. The resource is then parsed, before it's returned. Returns null if no match is found.
        /// </summary>
        public static string? GetResource(this IStringParser stringParser, IResourceService resourceService, string key, params StringTagPair[] args)
        {
            if (stringParser is null) throw new ArgumentNullException(nameof(stringParser));
            if (resourceService is null) throw new ArgumentNullException(nameof(resourceService));

            string? resource = resourceService.GetString(key);
            if (resource is null)
                return null;

            return stringParser.Parse(resource, args);
        }

#if NETCORE

        /// <summary>
        /// Resolves a resource. The resource is then parsed, before it's returned. Returns null if no match is found.
        /// </summary>
        public static string? GetResource(this IStringParser stringParser, IResourceService resourceService, ReadOnlySpan<char> key, params StringTagPair[] args)
        {
            if (stringParser is null) throw new ArgumentNullException(nameof(stringParser));
            if (resourceService is null) throw new ArgumentNullException(nameof(resourceService));

            string? resource = resourceService.GetString(key);
            if (resource is null)
                return null;

            return stringParser.Parse(resource, args);
        }

#endif
    }
}