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
}