namespace CoreMVVM
{
    /// <summary>
    /// Service for initializing views.
    /// </summary>
    public interface IViewInitializer
    {
        /// <summary>
        /// Initializes a view.
        /// </summary>
        void InitView(object viewModel, object view);
    }
}