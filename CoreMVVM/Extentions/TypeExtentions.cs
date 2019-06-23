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
    }
}