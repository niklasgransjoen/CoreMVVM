using System;

namespace CoreMVVM
{
    /// <summary>
    /// Extensions for <see cref="IViewProvider"/>.
    /// </summary>
    public static class IViewProviderExtensions
    {
        /// <summary>
        /// Attempts to locate a view.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type to resolve view type for.</typeparam>
        /// <param name="context">Context for providing the result.</param>
        /// <returns>True if the view was found.</returns>
        public static bool FindView<TViewModel>(this IViewProvider viewProvider, ViewProviderContext context)
            where TViewModel : class
        {
            if (viewProvider is null)
                throw new ArgumentNullException(nameof(viewProvider));

            return viewProvider.FindView(typeof(TViewModel), context);
        }
    }
}