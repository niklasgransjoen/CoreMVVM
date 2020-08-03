using System;

namespace CoreMVVM.Implementations
{
    /// <summary>
    /// A basic implementation of the view provider.
    /// </summary>
    public sealed class DefaultViewProvider : IViewProvider
    {
        public void FindView(ViewProviderContext context)
        {
            string viewTypeName = context.ViewModelType.FullName
                .Replace("ViewModel", "View")
                .Replace("WindowModel", "Window");

            Type viewType = context.ViewModelType.Assembly.GetType(viewTypeName);
            if (viewType != null)
            {
                context.SetViewType(viewType, cacheView: true);
            }
        }
    }
}