using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.InMemory.Internal;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.EntityFrameworkCore.InMemory.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EntityFrameworkCore.LocalStorage.Storage.Internal
{
    public class SerializableStore : IInMemoryStore
    {
        private readonly IInMemoryTableFactory _tableFactory;
        private readonly bool _useNameMatching;

        private readonly object _lock = new object();

        private Dictionary<object, ISerializableTable> _tables;

        public SerializableStore(
            IInMemoryTableFactory tableFactory,
            bool useNameMatching)
        {
            _tableFactory = tableFactory;
            _useNameMatching = useNameMatching;
        }

        public virtual InMemoryIntegerValueGenerator<TProperty> GetIntegerValueGenerator<TProperty>(
            IProperty property)
        {
            lock (_lock)
            {
                var entityType = property.DeclaringEntityType;
                var key = _useNameMatching ? (object)entityType.Name : entityType;

                return EnsureTable(key, entityType).GetIntegerValueGenerator<TProperty>(property);
            }
        }

        public virtual bool EnsureCreated(
            IUpdateAdapterFactory updateAdapterFactory,
            IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
        {
            lock (_lock)
            {
                var valuesSeeded = _tables == null;
                if (valuesSeeded)
                {
                    // ReSharper disable once AssignmentIsFullyDiscarded
                    _tables = CreateTables();

                    var updateAdapter = updateAdapterFactory.CreateStandalone();
                    var entries = new List<IUpdateEntry>();
                    foreach (var entityType in updateAdapter.Model.GetEntityTypes())
                    {
                        var key = _useNameMatching ? (object)entityType.Name : entityType;
                        //Load Data
                        var table = EnsureTable(key, entityType);

                        foreach (var targetSeed in entityType.GetSeedData())
                        {
                            var entry = updateAdapter.CreateEntry(targetSeed, entityType);
                            entry.EntityState = EntityState.Added;

                            ////update
                            if (table.Exists(entry))
                            {
                                entry.EntityState = EntityState.Modified;
                                foreach (var property in entry.EntityType.GetProperties().ToList())
                                {
                                    if (!property.IsKey())
                                    {
                                        entry.SetPropertyModified(property);
                                    }
                                }
                            }

                            entries.Add(entry);
                        }
                    }

                    ExecuteTransaction(entries, updateLogger);
                }

                return valuesSeeded;
            }
        }

        public virtual bool Clear()
        {
            lock (_lock)
            {
                if (_tables == null)
                {
                    return false;
                }

                _tables = null;

                return true;
            }
        }

        private static Dictionary<object, ISerializableTable> CreateTables()
            => new Dictionary<object, ISerializableTable>();

        public virtual IReadOnlyList<InMemoryTableSnapshot> GetTables(IEntityType entityType)
        {
            var data = new List<InMemoryTableSnapshot>();
            lock (_lock)
            {
                if (_tables != null)
                {
                    foreach (var et in entityType.GetDerivedTypesInclusive().Where(et => !et.IsAbstract()))
                    {
                        var key = _useNameMatching ? (object)et.Name : et;
                        if (_tables.TryGetValue(key, out var table))
                        {
                            data.Add(new InMemoryTableSnapshot(et, table.SnapshotRows()));
                        }
                    }
                }
            }

            return data;
        }

        public virtual int ExecuteTransaction(
            IList<IUpdateEntry> entries,
            IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
        {
            var rowsAffected = 0;

            lock (_lock)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    var entityType = entry.EntityType;

                    Debug.Assert(!entityType.IsAbstract());

                    var key = _useNameMatching ? (object)entityType.Name : entityType;
                    var table = EnsureTable(key, entityType);

                    if (entry.SharedIdentityEntry != null)
                    {
                        if (entry.EntityState == EntityState.Deleted)
                        {
                            continue;
                        }

                        table.Delete(entry);
                    }

                    switch (entry.EntityState)
                    {
                        case EntityState.Added:
                            table.Create(entry);
                            break;
                        case EntityState.Deleted:
                            table.Delete(entry);
                            break;
                        case EntityState.Modified:
                            table.Update(entry);
                            break;
                    }

                    rowsAffected++;
                }

                SaveTables();
            }

            updateLogger.ChangesSaved(entries, rowsAffected);

            return rowsAffected;
        }

        private void SaveTables()
        {
            foreach (KeyValuePair<object, ISerializableTable> table in _tables)
            {
                table.Value.Save();
            }
        }

        // Must be called from inside the lock
        private ISerializableTable EnsureTable(object key, IEntityType entityType)
        {
            if (_tables == null)
            {
                _tables = CreateTables();
            }

            if (!_tables.TryGetValue(key, out var table))
            {
                _tables.Add(key, table = (ISerializableTable)_tableFactory.Create(entityType));
            }

            return table;
        }
    }
}