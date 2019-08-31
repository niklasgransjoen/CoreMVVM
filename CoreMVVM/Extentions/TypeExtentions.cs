using System;

namespace CoreMVVM.Extentions
{
    public static class TypeExtentions
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
        /// Gets a value indicating if a generic type definition is assignable from a generic type.
        /// </summary>
        /// <param name="genericParentDef">The generic type definition.</param>
        /// <param name="child">The generic child to check.</param>
        /// <returns>True is the given generic type definition is assignable from the generic type.</returns>
        /// <exception cref="ArgumentNullException">genericParentDef or child is null.</exception>
        /// <exception cref="ArgumentException">genericParentDef is not a generic type definition.</exception>
        public static bool IsAssignableFromGeneric(this Type genericParentDef, Type child)
        {
            if (genericParentDef is null) throw new ArgumentNullException(nameof(genericParentDef));
            if (child is null) throw new ArgumentNullException(nameof(child));

            if (!genericParentDef.IsGenericTypeDefinition)
                throw new ArgumentException("parameter must be generic type definition", nameof(genericParentDef));

            if (!child.IsGenericType)
                return false;

            Type genericChildDef = child.GetGenericTypeDefinition();
            return genericParentDef.IsAssignableFrom(genericChildDef);
        }
    }
}