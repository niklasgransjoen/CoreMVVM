using System;

namespace CoreMVVM
{
    /// <summary>
    /// Context for resolving view from view model type.
    /// </summary>
    public sealed class ViewProviderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewProviderContext"/> class.
        /// </summary>
        public ViewProviderContext()
        {
        }

        /// <summary>
        /// Gets or sets the resolved view type.
        /// </summary>
        public Type ViewType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the resolved view type should be cached.
        /// </summary>
        /// <value>Default is false.</value>
        public bool CacheView { get; set; }
    }
}