using CoreMVVM.IOC;
using System;
using System.Windows;

namespace CoreMVVM.Windows
{
    public static class ControlServiceProvider
    {
        public static readonly DependencyProperty ServiceProviderProperty =
            DependencyProperty.RegisterAttached("ServiceProvider", typeof(ILifetimeScope), typeof(ControlServiceProvider),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static ILifetimeScope GetServiceProvider(DependencyObject o)
        {
            if (o is null)
                throw new ArgumentNullException(nameof(o));

            return (ILifetimeScope)o.GetValue(ServiceProviderProperty);
        }

        public static ILifetimeScope RequireServiceProvider(DependencyObject o)
        {
            var serviceProvider = GetServiceProvider(o);
            if (serviceProvider is null)
                throw new InvalidOperationException("Required Control Service Provider not available.");

            return serviceProvider;
        }

        public static void SetServiceProvider(DependencyObject o, ILifetimeScope value)
        {
            if (o is null)
                throw new ArgumentNullException(nameof(o));

            o.SetValue(ServiceProviderProperty, value);
        }
    }
}