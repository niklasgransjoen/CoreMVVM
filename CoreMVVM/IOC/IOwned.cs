namespace CoreMVVM.IOC
{
    /// <summary>
    /// A wrapper for allerting CoreMVVM that a created instance should not belong to the <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value.</typeparam>
    public interface IOwned<out T>
    {
        T Value { get; }
    }
}