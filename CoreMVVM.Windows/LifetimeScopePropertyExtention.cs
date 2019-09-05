using CoreMVVM.IOC;
using System;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Property extention of attaching <see cref="ILifetimeScope"/> to dependency objects.
    /// </summary>
    public static class LifetimeScopePropertyExtention
    {
        public static readonly DependencyProperty ServiceProviderProperty = DependencyProperty.RegisterAttached(
            "LifetimeScope", typeof(ILifetimeScope), typeof(LifetimeScopePropertyExtention));

        public static void SetLifetimeScope(DependencyObject element, ILifetimeScope value)
        {
            element.SetValue(ServiceProviderProperty, value);
        }

        public static ILifetimeScope GetLifetimeScope(DependencyObject element)
        {
            return (ILifetimeScope)element.GetValue(ServiceProviderProperty);
        }

        /// <summary>
        /// Gets the LifetimeScope from the given dependency object or one of its parents.
        /// </summary>
        /// <exception cref="Exception">lifetimescope was not found in the visual tree of the given component.</exception>
        public static ILifetimeScope FindLifetimeScope(DependencyObject o)
        {
            bool result = TryFindLifetimeScope(o, out ILifetimeScope lifetimeScope);
            if (result)
                return lifetimeScope;
            
            throw new Exception($"Could not locate {nameof(IContainer)} in visual tree.");
        }

        /// <summary>
        /// Attempts to get the container from the given dependency object or one of its parents.
        /// </summary>
        public static bool TryFindLifetimeScope(DependencyObject o, out ILifetimeScope lifetimeScope)
        {
            DependencyObject current = o;
            do
            {
                lifetimeScope = GetLifetimeScope(current);
                if (lifetimeScope != null)
                    return true;

                current = LogicalTreeHelper.GetParent(current);
            }
            while (current != null);

            lifetimeScope = null;
            return false;
        }
    }
}