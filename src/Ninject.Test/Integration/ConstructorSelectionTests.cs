namespace Ninject.Tests.Integration
{
    using System;
    using System.Runtime.InteropServices;

    using FluentAssertions;
    using Ninject.Parameters;
    using Ninject.Tests.Fakes;
    using Ninject.Tests.Integration.StandardKernelTests;

    using Xunit;

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class ConstructorSelectionTests
    {
        private readonly StandardKernel kernel;

        public ConstructorSelectionTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void DefaultCtorIsUsedWhenNoBindingAreAvailable()
        {
            kernel.Bind<Barracks>().ToSelf();

            var barracks = kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void CtorIsUsedWhenParameterIsSupplied()
        {
            kernel.Bind<Barracks>().ToSelf();
            var constructorArgument = new ConstructorArgument("warrior", new Samurai(new Sword()));
            var barracks = kernel.Get<Barracks>(constructorArgument);

            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().BeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void FirstAvailableWithBindingAvailableIsUsed()
        {
            kernel.Bind<Barracks>().ToSelf();
            kernel.Bind<IWeapon>().To<Sword>();

            var barracks = kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().NotBeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void CtorWithMostDependenciesIsUsedWhenBindingsAreAvailable()
        {
            kernel.Bind<Barracks>().ToSelf();
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<Samurai>();

            var barracks = kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().NotBeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void CreationWillFailIfAllDepenciesAreMissingAndInjectAttributeIsApplied()
        {
            kernel.Bind<NinjaBarracks>().ToSelf();

            Assert.Throws<ActivationException>(() => kernel.Get<NinjaBarracks>());

            kernel.Bind<IWeapon>().To<Sword>();
            Assert.Throws<ActivationException>(() => kernel.Get<NinjaBarracks>());
            kernel.Unbind<IWeapon>();

            kernel.Bind<IWarrior>().To<Samurai>();
            Assert.Throws<ActivationException>(() => kernel.Get<NinjaBarracks>());
            kernel.Unbind<IWarrior>();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void SelectedCtorIsUsedIfDeclared()
        {
            kernel.Bind<Barracks>().ToConstructor(_ => new Barracks());
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<Samurai>();

            var barracks = kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void SelectedCtorIsUsedIfDeclaredWithInjectedArgument()
        {
            kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(ctorArg.Inject<IWarrior>()));
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<Samurai>();

            var barracks = kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<Samurai>();
            barracks.Weapon.Should().BeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void WhenDefaultValuesArePassedToConstrctorSelectionTheyAreUsed()
        {
            kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(new Ninja(new Sword()), ctorArg.Inject<IWeapon>()));
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<Samurai>();

            var barracks = kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<Ninja>();
            barracks.Weapon.Should().NotBeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void DefaultValuesAreEvaluatedForEachRequest()
        {
            kernel.Bind<Barracks>().ToConstructor(_ => new Barracks(new Ninja(new Sword())));

            var barracks1 = kernel.Get<Barracks>();
            var barracks2 = kernel.Get<Barracks>();

            barracks1.Warrior.Should().NotBeSameAs(barracks2.Warrior);
        }

#if !WINDOWS_PHONE
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ConstantsCanBePassedToToConstructor()
        {
            var ninja = new Ninja(new Sword());
            kernel.Bind<Barracks>().ToConstructor(_ => new Barracks(ninja));

            var barracks1 = kernel.Get<Barracks>();
            var barracks2 = kernel.Get<Barracks>();

            barracks1.Warrior.Should().BeSameAs(barracks2.Warrior);
        }
#endif

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void WhenLazyValuesArePassedToConstrctorSelectionTheyAreEvaluatedAtResolve()
        {
            int activationCount = 0;
            kernel.Bind<Ninja>().ToSelf().Named("1").OnActivation(inst => activationCount++);
            kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(ctorArg.Context.Kernel.Get<Ninja>("1"), ctorArg.Inject<IWeapon>()));
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<Samurai>();

            activationCount.Should().Be(0);
            var barracks = kernel.Get<Barracks>();

            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<Ninja>();
            barracks.Weapon.Should().NotBeNull();
            activationCount.Should().Be(1);
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void WhenClassHasTwoConstructorsWithInjectAttributeThenAnActivationExceptionIsThrown()
        {
            kernel.Bind<ClassWithTwoInjectAttributes>().ToSelf();

            Action getClassWithTwoInjectAttributes = () => kernel.Get<ClassWithTwoInjectAttributes>();

            getClassWithTwoInjectAttributes.ShouldThrow<ActivationException>();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void WhenConstructorHasSelfBindableTypeItCountsAsServedParameter()
        {
            var instance = kernel.Get<ClassWithSelfBindableType>();

            instance.Sword.Should().NotBeNull();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void WhenConstructorHasAnOpenGenericTypeItCountsAsServedParameterIfBindingExists()
        {
            kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));
            var instance = kernel.Get<ClassWithGeneric>();

            instance.Generic.Should().NotBeNull();
        }

#if !SILVERLIGHT
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void WhenConstructorHasAValueWithDefaultValueItCountsAsServedParameter()
        {
            var instance = kernel.Get<ClassWithDefaultValue>();

            instance.X.Should().NotBe(0);
        }

        public class ClassWithDefaultValue
        {
            public ClassWithDefaultValue()
            {
            }

            public ClassWithDefaultValue([DefaultParameterValue(3)] int x)
            {
                this.X = x;
            }

            public int X { get; set; }
        }
#endif

        public class ClassWithGeneric
        {
            public ClassWithGeneric()
            {
            }

            public ClassWithGeneric(IGeneric<int> generic)
            {
                this.Generic = generic;
            }

            public IGeneric<int> Generic { get; set; }
        }

        public class ClassWithSelfBindableType
        {
            public ClassWithSelfBindableType()
            {
            }

            public ClassWithSelfBindableType(Sword sword)
            {
                this.Sword = sword;
            }

            public Sword Sword { get; set; }
        }

        public class ClassWithTwoInjectAttributes
        {
            [Inject]
            public ClassWithTwoInjectAttributes()
            {
            }

            [Inject]
            public ClassWithTwoInjectAttributes(int someValue)
            {
            }
        }
    }
}