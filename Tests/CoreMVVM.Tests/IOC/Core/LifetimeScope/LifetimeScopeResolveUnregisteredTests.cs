using CoreMVVM.Tests;
using System;
using Xunit;

namespace CoreMVVM.IOC.Core.Tests
{
    public class LifetimeScopeResolveUnregisteredTests : LifetimeScopeTestBase
    {
        [Fact]
        public void LifetimeScope_CreatesInstance_NoParams()
        {
            Class subject = LifetimeScope.ResolveRequiredService<Class>();
            Assert.NotNull(subject);
        }

        [Fact]
        public void LifetimeScope_CreatesInstance_WithParams()
        {
            ClassWithConstructor subject = LifetimeScope.ResolveRequiredService<ClassWithConstructor>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.a);
        }

        [Fact]
        public void LifetimeScope_CreatesInstance_ParameterlessConstructor()
        {
            ClassWithEmptyConstructor subject = LifetimeScope.ResolveRequiredService<ClassWithEmptyConstructor>();

            Assert.NotNull(subject);
            Assert.True(subject.constructorWasInvoked);
        }

        [Fact]
        public void LifetimeScope_Calls_ConstructorWithTheMostParams()
        {
            ClassWithManyConstructors subject = LifetimeScope.ResolveRequiredService<ClassWithManyConstructors>();

            Assert.NotNull(subject);
            Assert.NotNull(subject.Ec1);
            Assert.NotNull(subject.Ec2);
        }

        [Fact]
        public void LifetimeScope_Throws_ResolveUnregisteredInterface()
        {
            Assert.Throws<ResolveUnregisteredServiceException>(() => LifetimeScope.ResolveRequiredService<IInterface>());
        }

        [Fact]
        public void LifetimeScope_Throws_ResolveIllegalTypes()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                LifetimeScope.ResolveRequiredService(typeof(int));
            });

            Assert.Throws<ResolveException>(() =>
            {
                LifetimeScope.ResolveRequiredService<string>();
            });
        }

        internal class ClassWithConstructor
        {
            public Class? a;

            public ClassWithConstructor()
            {
            }

            public ClassWithConstructor(Class a)
            {
                this.a = a;
            }
        }

        internal class ClassWithEmptyConstructor
        {
            public bool constructorWasInvoked;

            public ClassWithEmptyConstructor()
            {
                constructorWasInvoked = true;
            }
        }

        internal class ClassWithManyConstructors
        {
            public ClassWithManyConstructors()
            {
            }

            public ClassWithManyConstructors(Class ec1)
            {
                Ec1 = ec1;
            }

            public ClassWithManyConstructors(Class ec1, Class ec2)
            {
                Ec1 = ec1;
                Ec2 = ec2;
            }

            public Class? Ec1 { get; }
            public Class? Ec2 { get; }
        }
    }
}