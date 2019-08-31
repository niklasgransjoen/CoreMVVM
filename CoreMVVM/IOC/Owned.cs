namespace CoreMVVM.IOC
{
    /// <summary>
    /// A wrapper for allerting CoreMVVM that a created instance should not belong to the <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value.</typeparam>
    public class Owned<T> : IOwned<T>
    {
        public Owned(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}