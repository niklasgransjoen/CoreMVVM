using System;
using System.Collections.Generic;
using System.Windows;

namespace CoreMVVM.Demo
{
    public sealed class ViewProvider : IViewProvider
    {
        private readonly Dictionary<Type, Type> _registeredViews = new Dictionary<Type, Type>();

        public bool FindView(Type viewModel, ViewProviderContext context)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            if (_registeredViews.TryGetValue(viewModel, out Type viewType))
            {
                context.ViewType = viewType;
                context.CacheView = true;

                return true;
            }

            return false;
        }

        public void RegisterView<TViewModel, TView>() 
            where TViewModel : class 
            where TView : FrameworkElement
        {
            _registeredViews[typeof(TViewModel)] = typeof(TView);
        }
    }
}