using EntityFrameworkCore.LocalStorage.StoreManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.InMemory.Internal;
using Microsoft.EntityFrameworkCore.InMemory.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EntityFrameworkCore.LocalStorage.Storage.Internal
{
    public class SerializableTable<TKey> : ISerializableTable
    {
        // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
        private readonly IPrincipalKeyValueFactory<TKey> _keyValueFactory;
        private readonly bool _sensitiveLoggingEnabled;
        private readonly IEntityType _entityType;
        private readonly IStoreManager _storeManager;
        private readonly Dictionary<TKey, object[]> _rows;

        private Dictionary<int, IInMemoryIntegerValueGenerator> _integerGenerators;

        public SerializableTable(
            IEntityType entityType,
            bool sensitiveLoggingEnabled,
            IStoreManager storeManager)
        {
            _keyValueFactory = entityType.FindPrimaryKey().GetPrincipalKeyValueFactory<TKey>();
            _sensitiveLoggingEnabled = sensitiveLoggingEnabled;
            _entityType = entityType;
            _storeManager = storeManager;

            _rows = Init();
        }

        public virtual InMemoryIntegerValueGenerator<TProperty> GetIntegerValueGenerator<TProperty>(IProperty property)
        {
            if (_integerGenerators == null)
            {
                _integerGenerators = new Dictionary<int, IInMemoryIntegerValueGenerator>();
            }

            // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
            var propertyIndex = Microsoft.EntityFrameworkCore.Metadata.Internal.PropertyBaseExtensions.GetIndex(property);
            if (!_integerGenerators.TryGetValue(propertyIndex, out var generator))
            {
                generator = new InMemoryIntegerValueGenerator<TProperty>(propertyIndex);
                _integerGenerators[propertyIndex] = generator;

                foreach (var row in _rows.Values)
                {
                    generator.Bump(row);
                }
            }

            return (InMemoryIntegerValueGenerator<TProperty>)generator;
        }


        public virtual IReadOnlyList<object[]> SnapshotRows()
            => _rows.Values.ToList();

        private static List<ValueComparer> GetStructuralComparers(IEnumerable<IProperty> properties)
            => properties.Select(GetStructuralComparer).ToList();

        private static ValueComparer GetStructuralComparer(IProperty p)
            => p.GetStructuralValueComparer() ?? p.FindTypeMapping()?.StructuralComparer;


        public virtual void Create(IUpdateEntry entry)
        {
            var row = entry.EntityType.GetProperties()
                .Select(p => SnapshotValue(p, GetStructuralComparer(p), entry))
                .ToArray();

            _rows.Add(CreateKey(entry), row);

            BumpValueGenerators(row);
        }


        public virtual void Delete(IUpdateEntry entry)
        {
            var key = CreateKey(entry);

            if (_rows.ContainsKey(key))
            {
                var properties = entry.EntityType.GetProperties().ToList();
                var concurrencyConflicts = new Dictionary<IProperty, object>();

                for (var index = 0; index < properties.Count; index++)
                {
                    IsConcurrencyConflict(entry, properties[index], _rows[key][index], concurrencyConflicts);
                }

                if (concurrencyConflicts.Count > 0)
                {
                    ThrowUpdateConcurrencyException(entry, concurrencyConflicts);
                }

                _rows.Remove(key);
            }
            else
            {
                throw new DbUpdateConcurrencyException(InMemoryStrings.UpdateConcurrencyException, new[] { entry });
            }
        }

        private static bool IsConcurrencyConflict(
            IUpdateEntry entry,
            IProperty property,
            object rowValue,
            Dictionary<IProperty, object> concurrencyConflicts)
        {
            if (property.IsConcurrencyToken
                && !StructuralComparisons.StructuralEqualityComparer.Equals(
                    rowValue,
                    entry.GetOriginalValue(property)))
            {
                concurrencyConflicts.Add(property, rowValue);

                return true;
            }

            return false;
        }


        public virtual void Update(IUpdateEntry entry)
        {
            var key = CreateKey(entry);

            if (_rows.ContainsKey(key))
            {
                var properties = entry.EntityType.GetProperties().ToList();
                var comparers = GetStructuralComparers(properties);
                var valueBuffer = new object[properties.Count];
                var concurrencyConflicts = new Dictionary<IProperty, object>();

                for (var index = 0; index < valueBuffer.Length; index++)
                {
                    if (IsConcurrencyConflict(entry, properties[index], _rows[key][index], concurrencyConflicts))
                    {
                        continue;
                    }

                    valueBuffer[index] = entry.IsModified(properties[index])
                        ? SnapshotValue(properties[index], comparers[index], entry)
                        : _rows[key][index];
                }

                if (concurrencyConflicts.Count > 0)
                {
                    ThrowUpdateConcurrencyException(entry, concurrencyConflicts);
                }

                _rows[key] = valueBuffer;

                BumpValueGenerators(valueBuffer);
            }
            else
            {
                throw new DbUpdateConcurrencyException(InMemoryStrings.UpdateConcurrencyException, new[] { entry });
            }
        }

        private void BumpValueGenerators(object[] row)
        {
            if (_integerGenerators != null)
            {
                foreach (var generator in _integerGenerators.Values)
                {
                    generator.Bump(row);
                }
            }
        }

        // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
        private TKey CreateKey(IUpdateEntry entry)
            => _keyValueFactory.CreateFromCurrentValues((Microsoft.EntityFrameworkCore.ChangeTracking.Internal.InternalEntityEntry)entry);

        private static object SnapshotValue(IProperty property, ValueComparer comparer, IUpdateEntry entry)
            => SnapshotValue(comparer, entry.GetCurrentValue(property));

        private static object SnapshotValue(ValueComparer comparer, object value)
            => comparer == null ? value : comparer.Snapshot(value);

        public void Save()
        {
            _storeManager.Serialize(ConvertToProvider(_rows));
        }

        /// <summary>
        ///     Throws an exception indicating that concurrency conflicts were detected.
        /// </summary>
        /// <param name="entry"> The update entry which resulted in the conflict(s). </param>
        /// <param name="concurrencyConflicts"> The conflicting properties with their associated database values. </param>
        protected virtual void ThrowUpdateConcurrencyException(IUpdateEntry entry, Dictionary<IProperty, object> concurrencyConflicts)
        {

            if (_sensitiveLoggingEnabled)
            {
                throw new DbUpdateConcurrencyException(
                    InMemoryStrings.UpdateConcurrencyTokenExceptionSensitive(
                        entry.EntityType.DisplayName(),
                        entry.BuildCurrentValuesString(entry.EntityType.FindPrimaryKey().Properties),
                        entry.BuildOriginalValuesString(concurrencyConflicts.Keys),
                        "{" + string.Join(", ", concurrencyConflicts.Select(c => c.Key.Name + ": " + Convert.ToString(c.Value, CultureInfo.InvariantCulture))) + "}"),
                    new[] { entry });
            }

            throw new DbUpdateConcurrencyException(
                InMemoryStrings.UpdateConcurrencyTokenException(
                    entry.EntityType.DisplayName(),
                    concurrencyConflicts.Keys.Format()),
                new[] { entry });
        }

        private Dictionary<TKey, object[]> Init()
        {
            Dictionary<TKey, object[]> newList = new Dictionary<TKey, object[]>(_keyValueFactory.EqualityComparer);
            return ConvertFromProvider(_storeManager.Deserialize(newList));
        }

        private Dictionary<TKey, object[]> ApplyValueConverter(Dictionary<TKey, object[]> list, Func<ValueConverter, Func<object, object>> conversionFunc)
        {
            var result = new Dictionary<TKey, object[]>(_keyValueFactory.EqualityComparer);
            var converters = _entityType.GetProperties().Select(p => p.GetValueConverter()).ToArray();
            foreach (var keyValuePair in list)
            {
                result[keyValuePair.Key] = keyValuePair.Value.Select((value, index) =>
                {
                    var converter = converters[index];
                    return converter == null ? value : conversionFunc(converter)(value);
                }).ToArray();
            }
            return result;
        }

        private Dictionary<TKey, object[]> ConvertToProvider(Dictionary<TKey, object[]> list)
        {
            return ApplyValueConverter(list, converter => converter.ConvertToProvider);
        }

        private Dictionary<TKey, object[]> ConvertFromProvider(Dictionary<TKey, object[]> list)
        {
            return ApplyValueConverter(list, converter => converter.ConvertFromProvider);
        }


        public bool Exists(IUpdateEntry entry)
        {
            var key = CreateKey(entry);
            return _rows.ContainsKey(key);
        }
    }
}


