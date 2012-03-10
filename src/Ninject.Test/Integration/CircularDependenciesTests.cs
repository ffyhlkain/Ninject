﻿namespace Ninject.Tests.Integration.CircularDependenciesTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Ninject.Activation;
    using Ninject.Parameters;
    using Xunit;

    public class CircularDependenciesContext : IDisposable
    {
        protected StandardKernel kernel;

        public CircularDependenciesContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public WhenDependenciesHaveTwoWayCircularReferenceBetweenConstructors()
        {
            kernel.Bind<TwoWayConstructorFoo>().ToSelf().InSingletonScope();
            kernel.Bind<TwoWayConstructorBar>().ToSelf().InSingletonScope();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void DoesNotThrowExceptionIfHookIsCreated()
        {
            var request = new Request(typeof(TwoWayConstructorFoo), null, Enumerable.Empty<IParameter>(), null, false, false);
            Assert.DoesNotThrow(() => kernel.Resolve(request));
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ThrowsActivationExceptionWhenHookIsResolved()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<TwoWayConstructorFoo>());
        }
    }

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class WhenDependenciesHaveTwoWayCircularReferenceBetweenProperties : CircularDependenciesContext
    {
        public WhenDependenciesHaveTwoWayCircularReferenceBetweenProperties()
        {
            kernel.Bind<TwoWayPropertyFoo>().ToSelf().InSingletonScope();
            kernel.Bind<TwoWayPropertyBar>().ToSelf().InSingletonScope();
        }


#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => kernel.Get<TwoWayPropertyFoo>());
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ScopeIsRespected()
        {
            var foo = kernel.Get<TwoWayPropertyFoo>();
            var bar = kernel.Get<TwoWayPropertyBar>();

            foo.Bar.Should().BeSameAs(bar);
            bar.Foo.Should().BeSameAs(foo);
        }
    }

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class WhenDependenciesHaveThreeWayCircularReferenceBetweenConstructors : CircularDependenciesContext
    {
        public WhenDependenciesHaveThreeWayCircularReferenceBetweenConstructors()
        {
            kernel.Bind<ThreeWayConstructorFoo>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayConstructorBar>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayConstructorBaz>().ToSelf().InSingletonScope();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void DoesNotThrowExceptionIfHookIsCreated()
        {
            var request = new Request(typeof(ThreeWayConstructorFoo), null, Enumerable.Empty<IParameter>(), null, false, false);
            Assert.DoesNotThrow(() => kernel.Resolve(request));
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ThrowsActivationExceptionWhenHookIsResolved()
        {
            Assert.Throws<ActivationException>(() => kernel.Get<ThreeWayConstructorFoo>());
        }
    }

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class WhenDependenciesHaveThreeWayCircularReferenceBetweenProperties : CircularDependenciesContext
    {
        public WhenDependenciesHaveThreeWayCircularReferenceBetweenProperties()
        {
            kernel.Bind<ThreeWayPropertyFoo>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayPropertyBar>().ToSelf().InSingletonScope();
            kernel.Bind<ThreeWayPropertyBaz>().ToSelf().InSingletonScope();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => kernel.Get<ThreeWayPropertyFoo>());
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ScopeIsRespected()
        {
            var foo = kernel.Get<ThreeWayPropertyFoo>();
            var bar = kernel.Get<ThreeWayPropertyBar>();
            var baz = kernel.Get<ThreeWayPropertyBaz>();

            foo.Bar.Should().BeSameAs(bar);
            bar.Baz.Should().BeSameAs(baz);
            baz.Foo.Should().BeSameAs(foo);
        }
    }

    public class TwoWayConstructorFoo
    {
        public TwoWayConstructorFoo(TwoWayConstructorBar bar) { }
    }

    public class TwoWayConstructorBar
    {
        public TwoWayConstructorBar(TwoWayConstructorFoo foo) { }
    }

    public class TwoWayPropertyFoo
    {
        [Inject] public TwoWayPropertyBar Bar { get; set; }
    }

    public class TwoWayPropertyBar
    {
        [Inject] public TwoWayPropertyFoo Foo { get; set; }
    }

    public class ThreeWayConstructorFoo
    {
        public ThreeWayConstructorFoo(ThreeWayConstructorBar bar) { }
    }

    public class ThreeWayConstructorBar
    {
        public ThreeWayConstructorBar(ThreeWayConstructorBaz baz) { }
    }

    public class ThreeWayConstructorBaz
    {
        public ThreeWayConstructorBaz(TwoWayConstructorFoo foo) { }
    }

    public class ThreeWayPropertyFoo
    {
        [Inject] public ThreeWayPropertyBar Bar { get; set; }
    }

    public class ThreeWayPropertyBar
    {
        [Inject] public ThreeWayPropertyBaz Baz { get; set; }
    }

    public class ThreeWayPropertyBaz
    {
        [Inject] public ThreeWayPropertyFoo Foo { get; set; }
    }

}