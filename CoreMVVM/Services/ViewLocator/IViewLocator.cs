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
        #region ResolveView

        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        /// <exception cref="ViewNotFoundException">no view found of the given view model type.</exception>
        object ResolveView<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Gets the view for the given view model.
        /// </summary>
        /// <exception cref="ViewNotFoundException">no view found of the given view model type.</exception>
        object ResolveView(object viewModel);

        /// <summary>
        /// Gets the view type for the given view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        /// <exception cref="ViewNotFoundException">no view found of the given view model type.</exception>
        Type ResolveViewType<TViewModel>();

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
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        bool TryResolveView<TViewModel>(out object view) where TViewModel : class;

        /// <summary>
        /// Gets the view for the given view model.
        /// </summary>
        bool TryResolveView(object viewModel, out object view);

        /// <summary>
        /// Gets the view type for the given view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        bool TryResolveViewType<TViewModel>(out Type viewType);

        /// <summary>
        /// Gets the view type for the given view model.
        /// </summary>
        bool TryResolveViewType(Type viewModelType, out Type viewType);

        #endregion TryResolveView

        /// <summary>
        /// Adds an action that gets performed on the resolved view before it's returned.
        /// </summary>
        /// <param name="action">The action to perform. The first argument is the view model, the second is the view.</param>
        void AddOnResolve(Action<object, object> action);

        /// <summary>
        /// Adds a view provider to the view locator.
        /// </summary>
        /// <typeparam name="TViewProvider">The type of the provider.</typeparam>
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
}