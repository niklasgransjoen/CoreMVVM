using System;
using System.Linq;

namespace CoreMVVM.Extentions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Gets the default value of this type.
        /// </summary>
        /// <exception cref="ArgumentNullException">type is null.</exception>
        public static object GetDefault(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        /// <summary>
        /// Gets a value indicating if a type implements a certain interface.
        /// </summary>
        /// <param name="type">The type to check on.</param>
        /// <param name="interface">The omterface to check for.</param>
        /// <returns>True is the given type is or implements the interface.</returns>
        /// <exception cref="ArgumentNullException">type or interface is null.</exception>
        /// <exception cref="ArgumentException">interface is not a interface.</exception>
        public static bool ImplementsInterface(this Type type, Type @interface)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (@interface is null) throw new ArgumentNullException(nameof(@interface));

            if (!@interface.IsInterface)
                throw new ArgumentException("parameter must be interface", nameof(@interface));

            return type == @interface ||
                   type.GetInterfaces()
                       .Any(i => i == @interface);
        }

        /// <summary>
        /// Gets a value indicating if a type implements a certain generic interface.
        /// </summary>
        /// <param name="type">The type to check on.</param>
        /// <param name="genericInterface">The interface to check for.</param>
        /// <returns>True is the given type is or implements the generic interface.</returns>
        /// <exception cref="ArgumentNullException">type or genericInterface is null.</exception>
        /// <exception cref="ArgumentException">genericInterface is not a interface or generic type definition.</exception>
        public static bool ImplementsGenericInterface(this Type type, Type genericInterface)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (genericInterface is null) throw new ArgumentNullException(nameof(genericInterface));

            if (!genericInterface.IsInterface) throw new ArgumentException("parameter must be interface", nameof(genericInterface));
            if (!genericInterface.IsGenericTypeDefinition) throw new ArgumentException("parameter must be generic type definition", nameof(genericInterface));

            if (type.IsInterface &&
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == genericInterface)
            {
                return true;
            }

            return type.GetInterfaces()
                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
        }
    }
}