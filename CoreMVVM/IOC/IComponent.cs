namespace CoreMVVM.IOC
{
    /// <summary>
    /// Implements logic available to the <see cref="ILifetimeScope"/> control.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Initialization invoked upon the component being resolved by the <see cref="ILifetimeScope"/>.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope that resolved the component.</param>
        void InitializeComponent(ILifetimeScope lifetimeScope);
    }
}