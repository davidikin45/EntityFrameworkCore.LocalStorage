using Blazored.LocalStorage;
using EntityFrameworkCore.LocalStorage.StoreManager;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace EntityFrameworkCore.LocalStorage.Storage.Internal
{
    public class SerializableTableFactory : IInMemoryTableFactory
    {
        private readonly bool _sensitiveLoggingEnabled;
        private readonly LocalStorageOptions _localStorageOptions;
        private readonly ISyncLocalStorageService _localStorage;

        private readonly ConcurrentDictionary<IKey, Func<IInMemoryTable>> _factories = new ConcurrentDictionary<IKey, Func<IInMemoryTable>>();

        public SerializableTableFactory(ILoggingOptions loggingOptions, LocalStorageOptions localStorageOptions, ISyncLocalStorageService localStorage)
        {
            _sensitiveLoggingEnabled = loggingOptions.IsSensitiveDataLoggingEnabled;
            _localStorageOptions = localStorageOptions;
            _localStorage = localStorage;
        }

        public virtual IInMemoryTable Create(IEntityType entityType)
              => _factories.GetOrAdd(entityType.FindPrimaryKey(), CreateTable)();

        private Func<IInMemoryTable> CreateTable(IKey key)
            => (Func<IInMemoryTable>)typeof(SerializableTableFactory).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateFactory))
                .MakeGenericMethod(GetKeyType(key))
                .Invoke(null, new object[] {key.DeclaringEntityType, _sensitiveLoggingEnabled , _localStorageOptions, _localStorage });

        private Type GetKeyType(IKey key) => key.Properties.Count > 1 ? typeof(object[]) : key.Properties.First().ClrType;

        private static Func<IInMemoryTable> CreateFactory<TKey>(
            IEntityType entityType, bool sensitiveLoggingEnabled, LocalStorageOptions options, ISyncLocalStorageService localStorage)
            => () => new SerializableTable<TKey>(entityType, sensitiveLoggingEnabled, new DefaultStoreManager<TKey>(options, entityType, localStorage));
    }
}

