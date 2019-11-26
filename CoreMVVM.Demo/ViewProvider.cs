using CoreMVVM.Demo.Views;
using System;

namespace CoreMVVM.Demo
{
    public sealed class ViewProvider : IViewProvider
    {
        public Type FindView<TViewModel>()
        {
            return FindView(typeof(TViewModel));
        }

        public Type FindView(Type viewModel)
        {
            if (viewModel == typeof(DialogWindowModel))
                return typeof(DialogWindow);

            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            string viewTypeName = viewModel.Namespace.Replace("ViewModel", "View").Replace("WindowModel", "Window");
            return viewModel.Assembly.GetType(viewTypeName);
        }
    }
}