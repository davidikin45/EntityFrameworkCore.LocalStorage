using EntityFrameworkCore.LocalStorage.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            IJSRuntime JSRuntime,
            string connectionString,
            InMemoryDatabaseRoot databaseRoot = null,
            Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseLocalStorageDatabaseConnectionString(
                (DbContextOptionsBuilder)optionsBuilder, JSRuntime, connectionString, databaseRoot, inMemoryOptionsAction);

        public static DbContextOptionsBuilder<TContext> UseLocalStorageDatabase<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            IJSRuntime JSRuntime,
            string serializer = "json",
            string databaseName = "",
            string password = "",
            InMemoryDatabaseRoot databaseRoot = null,
            Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseLocalStorageDatabase(
                (DbContextOptionsBuilder)optionsBuilder, JSRuntime, serializer, databaseName, password, databaseRoot, inMemoryOptionsAction);

        public static DbContextOptionsBuilder UseLocalStorageDatabaseConnectionString(
            this DbContextOptionsBuilder optionsBuilder,
            IJSRuntime JSRuntime,
            string connectionString,
            InMemoryDatabaseRoot databaseRoot = null,
            Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {
            string[] connectionStringParts = connectionString.Split(';');
            Dictionary<string, string> connectionStringSplitted = connectionStringParts
                .Select(segment => segment.Split('='))
                .ToDictionary(parts => parts[0].Trim().ToLowerInvariant(), parts => parts[1].Trim());

            return UseLocalStorageDatabase(optionsBuilder,
                JSRuntime,
                connectionStringSplitted.GetValueOrDefault("serializer"),
                connectionStringSplitted.GetValueOrDefault("databasename"),
                connectionStringSplitted.GetValueOrDefault("password")
                , databaseRoot, inMemoryOptionsAction);
        }

        public static DbContextOptionsBuilder UseLocalStorageDatabase(
        this DbContextOptionsBuilder optionsBuilder,
        IJSRuntime JSRuntime,
        string serializer = "json",
        string databaseName = "",
        string password = "",
        InMemoryDatabaseRoot databaseRoot = null,
        Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {

            var filemanager = string.IsNullOrEmpty(password) ? "default" : $"encrypted:{password}";
            var options = new LocalStorageOptions() { Serializer = serializer, DatabaseName = databaseName, Password = password, FileManager = filemanager, Location = null };

            //var dbServices = new ServiceCollection();
            //dbServices.AddEntityFrameworkInMemoryDatabase();
            //dbServices.AddSingleton(options);

            //dbServices.AddEntityFrameworkLocalStorageDatabase(JSRuntime);

            //optionsBuilder.UseInternalServiceProvider(dbServices.BuildServiceProvider()).UseFileContextDatabase(serializer, filemanager, databaseName, null, databaseRoot, inMemoryOptionsAction);

            //optionsBuilder.UseInternalServiceProvider(dbServices.BuildServiceProvider()).UseInMemoryDatabase(databaseName, databaseRoot, inMemoryOptionsAction);

            //optionsBuilder.UseInternalServiceProvider(dbServices.BuildServiceProvider()).UseInMemoryDatabase(databaseName, databaseRoot, inMemoryOptionsAction);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new LocalStorageOptionsExtension(JSRuntime, options));
            optionsBuilder.UseInMemoryDatabase(databaseName, databaseRoot, inMemoryOptionsAction);

            return optionsBuilder;
        }
    }
}
