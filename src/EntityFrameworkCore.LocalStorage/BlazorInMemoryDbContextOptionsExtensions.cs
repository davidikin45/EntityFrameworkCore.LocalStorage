using EntityFrameworkCore.LocalStorage.NewFolder;
using FileContextCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

// ReSharper disable once CheckNamespace
namespace EntityFrameworkCore.LocalStorage
{
    public static class BlazorInMemoryDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder<TContext> UseBlazorWebAssemblyInMemoryDatabase<TContext>([NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder, [NotNull] string databaseName, [CanBeNull] Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null) where TContext : DbContext
        {
            return optionsBuilder.UseInMemoryDatabase(databaseName, inMemoryOptionsAction).ForBlazorWebAssembly();
        }

        public static DbContextOptionsBuilder UseBlazorWebAssemblyInMemoryDatabase([NotNull] this DbContextOptionsBuilder optionsBuilder, [NotNull] string databaseName, [CanBeNull] Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {
            return optionsBuilder.UseInMemoryDatabase(databaseName, inMemoryOptionsAction).ForBlazorWebAssembly();
        }

        public static DbContextOptionsBuilder<TContext> UseBlazorWebAssemblyInMemoryDatabase<TContext>([NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder, [NotNull] string databaseName, [CanBeNull] InMemoryDatabaseRoot databaseRoot, [CanBeNull] Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null) where TContext : DbContext
        {
            return optionsBuilder.UseInMemoryDatabase(databaseName, databaseRoot, inMemoryOptionsAction).ForBlazorWebAssembly();
        }

        public static DbContextOptionsBuilder UseBlazorWebAssemblyInMemoryDatabase([NotNull] this DbContextOptionsBuilder optionsBuilder, [NotNull] string databaseName, [CanBeNull] InMemoryDatabaseRoot databaseRoot, [CanBeNull] Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {
            return optionsBuilder.UseInMemoryDatabase(databaseName, databaseRoot, inMemoryOptionsAction).ForBlazorWebAssembly();
        }

        public static DbContextOptionsBuilder<TContext> ForBlazorWebAssembly<TContext>([NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder) where TContext : DbContext
        {
            return optionsBuilder.ReplaceService<IModelSource, BlazorModelSource>();
        }

        public static DbContextOptionsBuilder ForBlazorWebAssembly([NotNull] this DbContextOptionsBuilder optionsBuilder)
        {
            return optionsBuilder.ReplaceService<IModelSource, BlazorModelSource>();
        }

        public static DbContextOptionsBuilder<TContext> EnableSeedData<TContext>([NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder) where TContext : DbContext
        {
            return optionsBuilder.ReplaceService<IFileContextStoreCache, LocalStorageStoreCache>();
        }

        public static DbContextOptionsBuilder EnableSeedData([NotNull] this DbContextOptionsBuilder optionsBuilder)
        {
            return optionsBuilder.ReplaceService<IFileContextStoreCache, LocalStorageStoreCache>();
        }
    }
}
 