﻿using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Provides possibility to hook load and save actions of <see cref="DbContext"/>.
    /// </summary>
    public sealed class DbContextHooker : IDisposable
    {
        private readonly DbContext _dbContext;
        private readonly List<IDbHook> _loadHooks;
        private readonly List<IDbHook> _saveHooks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextHooker"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public DbContextHooker(DbContext dbContext)
        {
            _loadHooks = new List<IDbHook>();
            _saveHooks = new List<IDbHook>();
            _dbContext = dbContext;

            var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
            objectContext.ObjectMaterialized += ObjectMaterialized;
            objectContext.SavingChanges += SavingChanges;
        }

        /// <summary>
        /// Registers a hook to run on object materialization stage.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        public void RegisterLoadHook(IDbHook dbHook)
        {
            _loadHooks.Add(dbHook);
        }

        /// <summary>
        /// Registers a hook to run before save data occurs.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        public void RegisterPreSaveHook(IDbHook dbHook)
        {
            _saveHooks.Add(dbHook);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
            objectContext.ObjectMaterialized -= ObjectMaterialized;
            objectContext.SavingChanges -= SavingChanges;
        }

        private void SavingChanges(object sender, EventArgs e)
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries().Select(entry => new DbEntityEntryAdapter(entry)))
            {
                foreach (var preSaveHook in _saveHooks)
                {
                    preSaveHook.HookEntry(entry);
                }
            }
        }

        private void ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entry = new DbEntityEntryAdapter(_dbContext.Entry(e.Entity));
            foreach (var loadHook in _loadHooks)
            {
                loadHook.HookEntry(entry);
            }
        }
    }
}
