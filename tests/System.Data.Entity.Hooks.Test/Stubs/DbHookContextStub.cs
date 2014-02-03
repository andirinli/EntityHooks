﻿namespace System.Data.Entity.Hooks.Test.Stubs
{
    internal class DbHookContextStub : DbHookContext, IDbContext
    {
        private static string _connectionId;

        public DbHookContextStub()
            : base(Effort.DbConnectionFactory.CreatePersistent(_connectionId), true)
        {
        }

        public DbSet<FooEntityStub> Foos { get; set; }

        public static void ResetConnections()
        {
            _connectionId = Guid.NewGuid().ToString();
        }

        public void AddLoadHook(IDbHook dbHook)
        {
            RegisterLoadHook(dbHook);
        }

        public void AddPreSaveHook(IDbHook dbHook)
        {
            RegisterPreSaveHook(dbHook);
        }

        public void AddPostSaveHook(IDbHook dbHook)
        {
            RegisterPostSaveHook(dbHook);
        }
    }
}
