using NUnit.Framework;

namespace CoreMVVM.Tests.IOC.Builder
{
    [TestFixture]
    public class RegistrationBuilderTests
    {
        /*private ContainerBuilder _builder;

        [SetUp]
        public void BeforeEach()
        {
            _builder = new ContainerBuilder(registerDefaults: false);
        }

        [TearDown]
        public void AfterEach()
        {
            _builder = null;
        }

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Types(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.Register<Class>();
            regBuilder.As(type);

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(typeof(Class), regBuilder.Registrations[type].Type);
        }

        [TestCaseSource(nameof(GetTypes))]
        public void Builder_Registers_Types_AsSelf(Type type)
        {
            IRegistrationBuilder regBuilder = _builder.Register(type).AsSelf();

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(type, regBuilder.Registrations[type].Type);
        }

        [TestCaseSource(nameof(GetFactoriesData))]
        public void Builder_Registers_Types_WithFactory(Type type, Func<IContainer, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.Register(factory);
            regBuilder.As(type);

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(factory, regBuilder.Registrations[type].Factory);
        }

        [TestCaseSource(nameof(GetFactoriesData))]
        public void Builder_Registers_Types_AsSelf_WithFactory(Type type, Func<IContainer, object> factory)
        {
            IRegistrationBuilder regBuilder = _builder.Register(factory).AsSelf();

            Assert.AreEqual(type, regBuilder.Registrations.Keys.First());
            Assert.AreEqual(factory, regBuilder.Registrations[type].Factory);
        }

        private static IEnumerable<Type> GetTypes()
        {
            yield return typeof(IInterface);
            yield return typeof(Class);
            yield return typeof(Struct);
        }

        private static IEnumerable<object[]> GetFactoriesData()
        {
            foreach ((Type type, Func<IContainer, object> factory) in GetFactories())
                yield return new object[] { type, factory };
        }

        private static IEnumerable<(Type type, Func<IContainer, object> factory)> GetFactories()
        {
            yield return (typeof(IInterface), c => new Implementation());
            yield return (typeof(Class), c => new Class());
            yield return (typeof(Struct), c => new Struct());
        }*/
    }
}