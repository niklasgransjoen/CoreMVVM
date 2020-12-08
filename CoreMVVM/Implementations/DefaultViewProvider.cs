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
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var vmTypeName = context.ViewModelType.FullName;
            if (vmTypeName is null)
                return;

            string viewTypeName = vmTypeName
                .Replace("ViewModel", "View")
                .Replace("WindowModel", "Window");

            var viewType = context.ViewModelType.Assembly.GetType(viewTypeName);
            if (viewType is not null)
            {
                context.SetViewType(viewType, cacheView: true);
            }
        }
    }
}