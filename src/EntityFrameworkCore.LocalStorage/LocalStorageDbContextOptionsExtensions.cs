using EntityFrameworkCore.LocalStorage.NewFolder;
using FileContextCore;
using FileContextCore.Infrastructure;
using FileContextCore.Storage;
using FileContextCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace EntityFrameworkCore.LocalStorage
{
    public static class LocalStorageDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder<TContext> UseLocalStorageDatabaseConnectionString<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            IJSRuntime JSRuntime,
            string connectionString,
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseLocalStorageDatabaseConnectionString(
                (DbContextOptionsBuilder)optionsBuilder, JSRuntime, connectionString, databaseRoot, inMemoryOptionsAction);

        public static DbContextOptionsBuilder<TContext> UseLocalStorageDatabase<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            IJSRuntime JSRuntime,
            string serializer = "json",
            string databaseName = "",
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseLocalStorageDatabase(
                (DbContextOptionsBuilder)optionsBuilder, JSRuntime, serializer, databaseName, databaseRoot, inMemoryOptionsAction);

        public static DbContextOptionsBuilder UseLocalStorageDatabaseConnectionString(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            IJSRuntime JSRuntime,
            string connectionString,
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {
            string[] connectionStringParts = connectionString.Split(';');
            Dictionary<string, string> connectionStringSplitted = connectionStringParts
                .Select(segment => segment.Split('='))
                .ToDictionary(parts => parts[0].Trim().ToLowerInvariant(), parts => parts[1].Trim());

            return UseLocalStorageDatabase(optionsBuilder,
                JSRuntime,
                connectionStringSplitted.GetValueOrDefault("serializer"),
                connectionStringSplitted.GetValueOrDefault("databasename")
                , databaseRoot, inMemoryOptionsAction);
        }

            public static DbContextOptionsBuilder UseLocalStorageDatabase(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            IJSRuntime JSRuntime,
            string serializer = "json",
            string databaseName = "",
            [CanBeNull] FileContextDatabaseRoot databaseRoot = null,
            [CanBeNull] Action<FileContextDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {
            var dbServices = new ServiceCollection();
  
            dbServices.AddEntityFrameworkLocalStorageDatabase(JSRuntime);

            optionsBuilder.UseInternalServiceProvider(dbServices.BuildServiceProvider()).UseFileContextDatabase(serializer, "localstorage", databaseName, null, databaseRoot, inMemoryOptionsAction);

            return optionsBuilder;
        }
    }
}
