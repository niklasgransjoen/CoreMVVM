using System;

namespace CoreMVVM.Implementations
{
    /// <summary>
    /// A basic implementation of the view provider.
    /// </summary>
    public sealed class DefaultViewProvider : IViewProvider
    {
        public Type FindView<TViewModel>() where TViewModel : class
        {
            return FindView(typeof(TViewModel));
        }

        public Type FindView(Type viewModel)
        {
            string viewTypeName = viewModel.FullName.Replace("ViewModel", "View").Replace("WindowModel", "Window");
            return viewModel.Assembly.GetType(viewTypeName);
        }
    }
}