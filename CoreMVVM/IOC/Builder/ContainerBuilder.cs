using CoreMVVM.IOC.Core;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// For building a container.
    /// </summary>
    public class ContainerBuilder
    {
        private readonly RegistrationCollection _registrations = new RegistrationCollection();

        public ContainerBuilder()
        {
            RegisterSingleton<ConsoleLogger>().As<ILogger>();
        }

        public RegistrationBuilder Register<T>()
        {
            return new RegistrationBuilder(_registrations, typeof(T));
        }

        public RegistrationBuilder RegisterSingleton<T>()
        {
            return new RegistrationBuilder(_registrations, typeof(T), isSingleton: true);
        }

        /// <summary>
        /// Constructs a container with all the registered components and services.
        /// </summary>
        public IContainer Build()
        {
            // Registers the container as a singleton, so it always resolves to this instance.
            RegisterSingleton<Container>().As<IContainer>().AsSelf();

            IContainer container = new Container(_registrations);

            // And set this instance as the last created one, so this is the one that's returned upon a call to IContainer.Resolve().
            _registrations[typeof(IContainer)].LastCreatedInstance = container;

            return container;
        }
    }
}