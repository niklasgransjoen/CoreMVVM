namespace CoreMVVM.IOC.Core
{
    /// <summary>
    /// Implementation of <see cref="IContainer"/>.
    /// </summary>
    internal sealed class Container : LifetimeScope, IContainer
    {
        /// <summary>
        /// Creates a new container.
        /// </summary>
        public Container(ToolBox toolBox) : base(toolBox)
        {
        }
    }
}