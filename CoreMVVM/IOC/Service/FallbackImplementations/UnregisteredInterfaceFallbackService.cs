using CoreMVVM.IOC;
using System.Reflection;

namespace CoreMVVM.IOC.FallbackImplementations
{
    /// <summary>
    /// Implementation of <see cref="IResolveUnregisteredInterfaceService"/>,
    /// uses <see cref="FallbackImplementationAttribute"/> to attempt resolve.
    /// </summary>
    public sealed class UnregisteredInterfaceFallbackService : IResolveUnregisteredInterfaceService
    {
        /// <summary>
        /// Handles the resolving of an unregistered interface.
        /// </summary>
        public void Handle(ResolveUnregisteredInterfaceContext context)
        {
            var attribute = context.InterfaceType.GetCustomAttribute<FallbackImplementationAttribute>();
            if (attribute != null)
            {
                context.SetInterfaceImplementationType(attribute.Type);
                context.CacheScope = ComponentScope.Singleton;
            }
        }
    }
}