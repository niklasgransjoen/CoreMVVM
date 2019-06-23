using CoreMVVM.IOC;
using System;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Property extention of attaching container to dependency objects.
    /// </summary>
    public static class ContainerPropertyExtention
    {
        public static readonly DependencyProperty ServiceProviderProperty = DependencyProperty.RegisterAttached(
            "ServiceProvider", typeof(IContainer), typeof(ContainerPropertyExtention));

        public static void SetServiceProvider(DependencyObject element, IContainer value)
        {
            element.SetValue(ServiceProviderProperty, value);
        }

        public static IContainer GetServiceProvider(DependencyObject element)
        {
            return (IContainer)element.GetValue(ServiceProviderProperty);
        }

        /// <summary>
        /// Gets the container from the given dependency object or one of its parents.
        /// </summary>
        public static IContainer GetContainer(DependencyObject o)
        {
            DependencyObject current = o;
            do
            {
                IContainer container = GetServiceProvider(current);
                if (container != null)
                    return container;

                current = LogicalTreeHelper.GetParent(current);
            }
            while (current != null);

            throw new Exception($"Could not locate {nameof(IContainer)} in visual tree.");
        }
    }
}