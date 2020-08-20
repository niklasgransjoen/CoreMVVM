using CoreMVVM.IOC;
using System;
using System.Globalization;

namespace CoreMVVM
{
    /// <summary>
    /// Implements functionality for a service that looks up resource strings.
    /// </summary>
    [FallbackImplementation(typeof(FallbackImplementations.FallbackResourceService))]
    public interface IResourceService
    {
        /// <summary>
        /// Occurs when the current culture changes.
        /// </summary>
        event EventHandler OnCurrentCultureChanged;

        /// <summary>
        /// Gets or sets the current culture of this resource service.
        /// </summary>
        CultureInfo CurrentCulture { get; set; }

        /// <summary>
        /// Performs a resource lookup. Returns null if key is not found.
        /// </summary>
        string? GetString(string key);

#if NETCORE

        /// <summary>
        /// Performs a resource lookup. Returns null if key is not found.
        /// </summary>
        public string? GetString(ReadOnlySpan<char> key) => GetString(key.ToString());

#endif
    }
}