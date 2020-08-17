using CoreMVVM.IOC;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Implementation of <see cref="IViewInitializer"/> that assigns the <see cref="ControlServiceProvider.ServiceProviderProperty"/> if the view is of type <see cref="DependencyObject"/>.
    /// </summary>
    public class ViewSetServiceProvider : IViewInitializer
    {
        private readonly ILifetimeScope _lifetimeScope;

        public ViewSetServiceProvider(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public void InitView(object viewModel, object view)
        {
            if (view is DependencyObject o)
            {
                o.SetServiceProvider(_lifetimeScope);
            }
        }
    }
}