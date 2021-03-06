﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace CoreMVVM
{
    /// <summary>
    /// Extensions for <see cref="IViewLocator"/>.
    /// </summary>
    public static class IViewLocatorExtensions
    {
        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        /// <exception cref="ViewNotFoundException">no view found of the given view model type.</exception>
        public static object ResolveView<TViewModel>(this IViewLocator viewLocator)
            where TViewModel : class
        {
            if (viewLocator is null)
                throw new ArgumentNullException(nameof(viewLocator));

            return viewLocator.ResolveView(typeof(TViewModel));
        }

        /// <summary>
        /// Gets the view type for the given view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        /// <exception cref="ViewNotFoundException">no view found of the given view model type.</exception>
        public static Type ResolveViewType<TViewModel>(this IViewLocator viewLocator)
            where TViewModel : class
        {
            if (viewLocator is null)
                throw new ArgumentNullException(nameof(viewLocator));

            return viewLocator.ResolveViewType(typeof(TViewModel));
        }

        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        public static bool TryResolveView<TViewModel>(this IViewLocator viewLocator, [NotNullWhen(true)] out object? view)
            where TViewModel : class
        {
            if (viewLocator is null)
                throw new ArgumentNullException(nameof(viewLocator));

            return viewLocator.TryResolveView(typeof(TViewModel), out view);
        }

        /// <summary>
        /// Gets the view type for the given view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        public static bool TryResolveViewType<TViewModel>(this IViewLocator viewLocator, [NotNullWhen(true)] out Type? viewType)
            where TViewModel : class
        {
            if (viewLocator is null)
                throw new ArgumentNullException(nameof(viewLocator));

            return viewLocator.TryResolveViewType(typeof(TViewModel), out viewType);
        }
    }
}