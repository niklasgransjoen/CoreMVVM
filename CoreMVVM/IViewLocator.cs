using CoreMVVM.Implementations;
using CoreMVVM.IOC;
using System;

namespace CoreMVVM
{
    /// <summary>
    /// Provides methods for retrieving a view instance from a given view model.
    /// </summary>
    [FallbackImplementation(typeof(ViewLocator))]
    public interface IViewLocator
    {
        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        object GetView<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Gets the view for the given view model.
        /// </summary>
        object GetView(object viewModel);

        /// <summary>
        /// Adds a view provider to the view locator.
        /// </summary>
        /// <typeparam name="IViewProivder">The type of the provider.</typeparam>
        /// <remarks>
        /// View providers are used to locate views belonging to a given view model.
        /// </remarks>
        void AddViewProvider<TViewProvider>() where TViewProvider : class, IViewProvider;

        /// <summary>
        /// Adds a view provider to the view locator.
        /// </summary>
        /// <param name="viewProvider">The provider.</param>
        /// <remarks>
        /// View providers are used to locate views belonging to a given view model.
        /// </remarks>
        void AddViewProvider(IViewProvider viewProvider);
    }

    /// <summary>
    /// Implements functionality for finding the view belonging to a view model.
    /// </summary>
    public interface IViewProvider
    {
        Type FindView<TViewModel>() where TViewModel : class;

        Type FindView(Type viewModel);
    }
}