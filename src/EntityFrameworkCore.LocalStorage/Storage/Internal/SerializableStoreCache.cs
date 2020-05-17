using Microsoft.EntityFrameworkCore.InMemory.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using System.Collections.Concurrent;
using System.Threading;

namespace EntityFrameworkCore.LocalStorage.Storage.Internal
{
    public class SerializableStoreCache : IInMemoryStoreCache
    {
        private readonly IInMemoryTableFactory _tableFactory;
        private readonly bool _useNameMatching;
        private readonly ConcurrentDictionary<string, IInMemoryStore> _namedStores;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public SerializableStoreCache(
            IInMemoryTableFactory tableFactory,
            IInMemorySingletonOptions options)
        {
            _tableFactory = tableFactory;

            if (options?.DatabaseRoot != null)
            {
                _useNameMatching = true;

                LazyInitializer.EnsureInitialized(
                    ref options.DatabaseRoot.Instance,
                    () => new ConcurrentDictionary<string, IInMemoryStore>());

                _namedStores = (ConcurrentDictionary<string, IInMemoryStore>)options.DatabaseRoot.Instance;
            }
            else
            {
                _namedStores = new ConcurrentDictionary<string, IInMemoryStore>();
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IInMemoryStore GetStore(string name)
            => _namedStores.GetOrAdd(name, _ => new SerializableStore(_tableFactory, _useNameMatching));
    }
}
