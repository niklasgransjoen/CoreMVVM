namespace CoreMVVM
{
    /// <summary>
    /// Provides methods for retrieving a view instance from a given view model.
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Gets the view for the view model of a given type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to get the view for.</typeparam>
        object GetView<TViewModel>();

        /// <summary>
        /// Gets the view for the given view model.
        /// </summary>
        object GetView(object viewModel);
    }
}