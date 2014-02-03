﻿using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Test.Stubs;

namespace System.Data.Entity.Hooks.Test
{
    [TestFixture]
    internal abstract class DbHookRegistrarFixture
    {
        private IDbHook _hook1;
        private IDbHook _hook2;
        private IDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _hook1 = Substitute.For<IDbHook>();
            _hook2 = Substitute.For<IDbHook>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();   
            }
        }

        [Test]
        public void ShouldRunPreSaveHooks_OnSave()
        {
            _dbContext = SetupDbContext();
            RegisterPreSaveHook(_hook1);
            RegisterPreSaveHook(_hook2);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldRunLoadHooks_OnLoad()
        {
            var foo = new FooEntityStub();
            _dbContext = SetupDbContext();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();
            _dbContext.Dispose();

            _dbContext = SetupDbContext();

            RegisterLoadHook(_hook1);
            RegisterLoadHook(_hook2);

            _dbContext.Foos.Load();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldNotRunLoadHooks_OnSave()
        {
            _dbContext = SetupDbContext();
            RegisterLoadHook(_hook1);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();

            _hook1.DidNotReceive().HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldNotRunPreSaveHooks_OnLoad()
        {
            var foo = new FooEntityStub();
            _dbContext = SetupDbContext();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();
            _dbContext.Dispose();

            _dbContext = SetupDbContext();
            RegisterPreSaveHook(_hook1);

            _dbContext.Foos.Load();

            _hook1.DidNotReceive().HookEntry(Arg.Any<IDbEntityEntry>());
        }

        protected abstract void RegisterLoadHook(IDbHook hook);

        protected abstract void RegisterPreSaveHook(IDbHook hook);

        protected abstract IDbContext SetupDbContext();
    }
}
