using CoreMVVM.Implementations;
using CoreMVVM.IOC;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CoreMVVM
{
    /// <summary>
    /// Provides methods for retrieving a view instance from a given view model.
    /// </summary>
    [FallbackImplementation(typeof(ViewLocator))]
    public interface IViewLocator
    {
        #region ResolveView

        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        /// <exception cref="ResolveException">the view model or view cannot be resolved by the IOC container.</exception>
        /// <exception cref="ViewNotFoundException">no view found of the given view model type.</exception>
        object ResolveView(Type viewModelType);

        /// <summary>
        /// Gets the view for the given view model.
        /// </summary>
        /// <exception cref="ResolveException">the view cannot be resolved by the IOC container.</exception>
        /// <exception cref="ViewNotFoundException">no view found of the given view model type.</exception>
        object ResolveView(object viewModel);

        /// <summary>
        /// Gets the view type for the given view model.
        /// </summary>
        /// <exception cref="ViewNotFoundException">no view found of the given view model type.</exception>
        Type ResolveViewType(Type viewModelType);

        #endregion ResolveView

        #region TryResolveView

        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        bool TryResolveView(Type viewModelType, [NotNullWhen(true)] out object? view);

        /// <summary>
        /// Gets the view for the given view model.
        /// </summary>
        bool TryResolveView(object viewModel, [NotNullWhen(true)] out object? view);

        /// <summary>
        /// Gets the view type for the given view model.
        /// </summary>
        bool TryResolveViewType(Type viewModelType, [NotNullWhen(true)] out Type? viewType);

        #endregion TryResolveView
    }
}