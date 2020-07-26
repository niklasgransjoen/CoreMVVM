using System;

namespace CoreMVVM.IOC.Core
{
    internal static class IRegistrationExtensions
    {
        /// <summary>
        /// Returns the concrete type of this registration, even if its an unbound generic type.
        /// </summary>
        public static Type GetConcreteType(this IRegistration registration, Type serviceType)
        {
            if (registration is null) throw new ArgumentNullException(nameof(registration));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));

            if (registration.Type.IsGenericTypeDefinition)
                return registration.Type.MakeGenericType(serviceType.GenericTypeArguments);

            return registration.Type;
        }
    }
}