using System;
using System.Collections.Generic;

namespace CoreMVVM.Demo
{
    public sealed class ViewProvider : IViewProvider
    {
        private readonly Dictionary<Type, Type> _registeredViews = new Dictionary<Type, Type>();

        public Type FindView<TViewModel>()
        {
            return FindView(typeof(TViewModel));
        }

        public Type FindView(Type viewModel)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            if (_registeredViews.TryGetValue(viewModel, out Type viewType))
                return viewType;

            return null;
        }

        public void RegisterView<TViewModel, TView>()
        {
            _registeredViews[typeof(TViewModel)] = typeof(TView);
        }
    }
}