using CoreMVVM.IOC;
using CoreMVVM.Services;
using System.Reflection;

namespace CoreMVVM.Implementations
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
            var attributes = context.InterfaceType.GetCustomAttribute<FallbackImplementationAttribute>();
            if (attributes != null)
            {
                context.SetInterfaceImplementationType(attributes.Type);
            }
        }
    }
}