using CoreMVVM.IOC;
using System;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// For attatching and using the attached service provider of a dependency object.
    /// </summary>
    public static class ControlServiceProvider
    {
        /// <summary>
        /// Identifies the ServiceProvider attached dependency property.
        /// </summary>
        public static readonly DependencyProperty ServiceProviderProperty =
            DependencyProperty.RegisterAttached("ServiceProvider", typeof(ILifetimeScope), typeof(ControlServiceProvider),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the service provider attached to a dependency object.
        /// </summary>
        /// <param name="o">The dependency object the service provider is attached to.</param>
        /// <returns>The attached service provider, or null if there is none.</returns>
        public static ILifetimeScope? GetServiceProvider(this DependencyObject o)
        {
            if (o is null)
                throw new ArgumentNullException(nameof(o));

            return (ILifetimeScope)o.GetValue(ServiceProviderProperty);
        }

        /// <summary>
        /// Attaches a service provider to a dependency object.
        /// </summary>
        /// <param name="o">The dependency object to attach the service provider to.</param>
        /// <param name="value">The service provider.</param>
        public static void SetServiceProvider(this DependencyObject o, ILifetimeScope? value)
        {
            if (o is null)
                throw new ArgumentNullException(nameof(o));

            o.SetValue(ServiceProviderProperty, value);
        }

        /// <summary>
        /// Gets the service provider attached to a dependency object.
        /// </summary>
        /// <param name="o">The dependency object the service provider is attached to.</param>
        /// <returns>The attached service provider.</returns>
        /// <exception cref="InvalidOperationException">The dependency object does not have an attached service provider.</exception>
        public static ILifetimeScope RequireServiceProvider(this DependencyObject o)
        {
            return GetServiceProvider(o) ?? throw new InvalidOperationException("Required Control Service Provider not available.");
        }
    }
}