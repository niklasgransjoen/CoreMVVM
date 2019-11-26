using CoreMVVM.IOC;
using CoreMVVM.Services;

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
            var attributes = context.InterfaceType.GetCustomAttributes(typeof(FallbackImplementationAttribute), inherit: false);
            if (attributes.Length != 0)
            {
                var fallbackAttribute = (FallbackImplementationAttribute)attributes[0];
                context.SetInterfaceImplementationType(fallbackAttribute.Type);
            }
        }
    }
}