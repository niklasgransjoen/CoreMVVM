namespace CoreMVVM.IOC
{
    /// <summary>
    /// Specifies the scope of a component.
    /// </summary>
    public enum ComponentScope
    {
        /// <summary>
        /// The component has no scope. New instance per call to resolve.
        /// </summary>
        Transient = 0,

        /// <summary>
        /// The component is only constructed once throughout the lifetime of the lifetime scope.
        /// </summary>
        LifetimeScope = 1,

        /// <summary>
        /// The component is only constructed once throughout the lifetime of the container.
        /// </summary>
        Singleton = 2,
    }
}