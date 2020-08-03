using System;

namespace CoreMVVM
{
    /// <summary>
    /// Implements functionality for finding the view belonging to a view model.
    /// </summary>
    public interface IViewProvider
    {
        /// <summary>
        /// Attempts to locate a view.
        /// </summary>
        /// <param name="context">Context for the operation.</param>
        void FindView(ViewProviderContext context);
    }

    /// <summary>
    /// Context for resolving view from view model type.
    /// </summary>
    public sealed class ViewProviderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewProviderContext"/> class.
        /// </summary>
        public ViewProviderContext(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }

        /// <summary>
        /// Gets the type of the view model to resolve a view for.
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        /// Gets the resolved view type.
        /// </summary>
        public Type? ViewType { get; private set; }

        /// <summary>
        /// Gets a value indicating if the resolved view type should be cached.
        /// </summary>
        public bool CacheView { get; private set; }

        public void SetViewType(Type viewType, bool cacheView)
        {
            if (viewType is null)
                throw new ArgumentNullException(nameof(viewType));

            if (!viewType.IsClass || viewType.IsAbstract)
                throw new ArgumentException($"Type '{viewType}' must be a non-abstract, non-static class.", nameof(viewType));

            ViewType = viewType;
            CacheView = cacheView;
        }
    }
}