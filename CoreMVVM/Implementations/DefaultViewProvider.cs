using System;

namespace CoreMVVM.Implementations
{
    /// <summary>
    /// A basic implementation of the view provider.
    /// </summary>
    public sealed class DefaultViewProvider : IViewProvider
    {
        public bool FindView(Type viewModel, ViewProviderContext context)
        {
            string viewTypeName = viewModel.FullName
                .Replace("ViewModel", "View")
                .Replace("WindowModel", "Window");

            Type viewType = viewModel.Assembly.GetType(viewTypeName);
            if (viewType is null)
                return false;

            context.ViewType = viewType;
            context.CacheView = true;

            return true;
        }
    }
}