using System;
using System.Collections.Generic;

namespace CoreMVVM
{
    /// <summary>
    /// For building a container.
    /// </summary>
    public class ContainerBuilder
    {
        private readonly Dictionary<Type, Registration> registeredTypes = new Dictionary<Type, Registration>();

        public ContainerBuilder()
        {
            Register<ILogger, ConsoleLogger>();
        }

        public void Register<TIn, TOut>() where TOut : TIn
        {
            registeredTypes[typeof(TIn)] = new Registration(typeof(TOut));
        }

        public void RegisterSingleton<T>() => RegisterSingleton<T, T>();

        public void RegisterSingleton<TIn, TOut>() where TOut : TIn
        {
            registeredTypes[typeof(TIn)] = new Registration(typeof(TOut))
            {
                IsSingleton = true,
            };
        }

        public IContainer Build()
        {
            registeredTypes[typeof(IContainer)] = new Registration(typeof(Container))
            {
                IsSingleton = true,
            };
            IContainer container = new Container(registeredTypes);
            registeredTypes[typeof(IContainer)].LastCreatedInstance = container;

            return container;
        }
    }

    /// <summary>
    /// Stores info about a registration.
    /// </summary>
    public class Registration
    {
        public Registration(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type of this registration.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets or sets a value indicating if this registration should only be created once.
        /// </summary>
        public bool IsSingleton { get; set; }

        /// <summary>
        /// Gets or sets the last created instance of this registration.
        /// </summary>
        public object LastCreatedInstance { get; set; }
    }
}