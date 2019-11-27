using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// Used to specify the scope of a class without registrating it with the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ScopeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAttribute"/> class.
        /// </summary>
        /// <param name="scope">The scope of the attributed class.</param>
        public ScopeAttribute(ComponentScope scope)
        {
            Scope = scope;
        }

        /// <summary>
        /// Gets the scope the attributed class is to be registered with.
        /// </summary>
        public ComponentScope Scope { get; }
    }
}