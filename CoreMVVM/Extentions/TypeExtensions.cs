using System;
using System.Linq;

namespace CoreMVVM
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the default value of this type.
        /// </summary>
        /// <exception cref="ArgumentNullException">type is null.</exception>
        public static object GetDefault(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        /// <summary>
        /// Determines if two types are compatible, even if both are generic type definitions.
        /// </summary>
        /// <remarks>
        /// If at least one of the types is not a generic type definition, the call is simply forwarded <see cref="Type.IsAssignableFrom(Type)"/>.
        /// If both are generic type definitions, the returned result tells you if t2 can be assigned a variable of type t1 *if* both times are created
        /// with the same type arguments.
        /// </remarks>
        public static bool IsAssignableFromGeneric(this Type t1, Type t2)
        {
            if (t1 is null) throw new ArgumentNullException(nameof(t1));
            if (t2 is null) throw new ArgumentNullException(nameof(t2));

            if (t1 == t2)
                return true;

            if (!t1.IsGenericTypeDefinition || !t2.IsGenericTypeDefinition)
                return t1.IsAssignableFrom(t2);

            if (t1.IsValueType)
                return false;

            if (t1.IsClass)
            {
                if (t2.BaseType is null)
                    return false;

                if (!t2.BaseType.IsGenericType)
                    return t1.IsAssignableFrom(t2.BaseType);
                else if (t2.BaseType.GetGenericArguments().All(arg => arg.IsGenericParameter))
                    return t1.IsAssignableFromGeneric(t2.BaseType.GetGenericTypeDefinition());
                else
                    return false;
            }

            // t1 is an interface.
            foreach (var interfaceType in t2.GetInterfaces().Where(i => i.IsGenericType))
            {
                if (t1 != interfaceType.GetGenericTypeDefinition())
                    continue;

                // We're looking at the two same interfaces
                // Now it remains to check if their generic arguments match.
                return interfaceType.GetGenericArguments().All(arg => arg.IsGenericParameter);
            }

            return t1.IsAssignableFrom(t2);
        }

        /// <summary>
        /// Gets an exact generic class or interface from that's an ancestor of the given type.
        /// </summary>
        /// <param name="type">The type to get an exact generic ascestor from.</param>
        /// <param name="genericTypeDefinition">The generic type defintion of the exact type to locate.</param>
        /// <returns>The located type, or null if no such type could be found.</returns>
        /// <exception cref="ArgumentException">genericTypeDefinition is not a generic type definition.</exception>
        public static Type GetGenericBaseType(this Type type, Type genericTypeDefinition)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (genericTypeDefinition is null) throw new ArgumentNullException(nameof(genericTypeDefinition));

            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException($"Type '{genericTypeDefinition}' is not a generic type definition.");

            if (genericTypeDefinition.IsValueType)
                return null;

            if (genericTypeDefinition.IsInterface)
            {
                return type.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == genericTypeDefinition);
            }

            // genericTypeDefinition is a class.
            if (type.BaseType != null)
            {
                if (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == genericTypeDefinition)
                    return type.BaseType;

                return type.BaseType.GetGenericBaseType(genericTypeDefinition);
            }

            return null;
        }
    }
}