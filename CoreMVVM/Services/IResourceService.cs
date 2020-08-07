using CoreMVVM.IOC;
using System;
using System.Globalization;

namespace CoreMVVM
{
    /// <summary>
    /// Implements functionality for a service that looks up resource strings.
    /// </summary>
    /// <remarks>
    /// If this service is implemented as a non-singleton, a custom implementation of <see cref="IResourceServiceProvider"/> should also be registered.
    /// The fallback implementation assumes a singleton pattern of the <see cref="IResourceService"/>.
    /// </remarks>
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

        string? GetString(ReadOnlySpan<char> key);

#endif
    }

    /// <summary>
    /// Service for resolving and potentially freeing instances of <see cref="IResourceService"/>.
    /// </summary>
    [FallbackImplementation(typeof(FallbackImplementations.FallbackResourceServiceProvider))]
    public interface IResourceServiceProvider
    {
        IResourceService GetResourceService();

        void FreeResourceService(IResourceService resourceService);
    }
}