using CoreMVVM.IOC.Core;
using System;
using System.Collections.Generic;

namespace CoreMVVM.IOC.Builder
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
}