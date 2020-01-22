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
        /// Gets the view type for the given view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        Type GetViewType<TViewModel>();

        /// <summary>
        /// Gets the view type for the given view model.
        /// </summary>
        Type GetViewType(Type viewModelType);

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

    /// <summary>
    /// Implements functionality for finding the view belonging to a view model.
    /// </summary>
    public interface IViewProvider
    {
        /// <summary>
        /// Attempts to locate a view.
        /// </summary>
        /// <typeparam name="TViewModel">The view model to locate the view of.</typeparam>
        /// <param name="context">Context for providing the result.</param>
        /// <returns>True if the view was found.</returns>
        bool FindView<TViewModel>(ViewProviderContext context);

        /// <summary>
        /// Attempts to locate a view.
        /// </summary>
        /// <param name="viewModel">The type of view model to locate the view of.</param>
        /// <param name="context">Context for providing the result.</param>
        /// <returns>True if the view was found.</returns>
        bool FindView(Type viewModel, ViewProviderContext context);
    }

    /// <summary>
    /// Context for resolving view from view model type.
    /// </summary>
    public sealed class ViewProviderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewProviderContext"/> class.
        /// </summary>
        public ViewProviderContext()
        {
        }

        /// <summary>
        /// Gets or sets the resolved view type.
        /// </summary>
        public Type ViewType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the resolved view type should be cached.
        /// </summary>
        /// <value>Default is false.</value>
        public bool CacheView { get; set; }
    }
}