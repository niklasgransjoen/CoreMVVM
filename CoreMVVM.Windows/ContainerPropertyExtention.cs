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

        public static void SetContainer(DependencyObject element, IContainer value)
        {
            element.SetValue(ServiceProviderProperty, value);
        }

        public static IContainer GetContainer(DependencyObject element)
        {
            return (IContainer)element.GetValue(ServiceProviderProperty);
        }

        /// <summary>
        /// Gets the container from the given dependency object or one of its parents.
        /// </summary>
        /// <exception cref="Exception">container was not found in the visual tree of the given component.</exception>
        public static IContainer FindContainer(DependencyObject o)
        {
            bool result = TryFindContainer(o, out IContainer container);
            if (result)
                return container;
            
            throw new Exception($"Could not locate {nameof(IContainer)} in visual tree.");
        }

        /// <summary>
        /// Attempts to get the container from the given dependency object or one of its parents.
        /// </summary>
        public static bool TryFindContainer(DependencyObject o, out IContainer container)
        {
            DependencyObject current = o;
            do
            {
                container = GetContainer(current);
                if (container != null)
                    return true;

                current = LogicalTreeHelper.GetParent(current);
            }
            while (current != null);

            container = null;
            return false;
        }
    }
}