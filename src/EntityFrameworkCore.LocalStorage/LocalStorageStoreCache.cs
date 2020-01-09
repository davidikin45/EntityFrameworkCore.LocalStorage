using EntityFrameworkCore.LocalStorage.Storage.Internal;
using Blazored.LocalStorage;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

namespace EntityFrameworkCore.LocalStorage.NewFolder
{
    public class LocalStorageStoreCache : IFileContextStoreCache
    {
        [JetBrains.Annotations.NotNull] private readonly ILoggingOptions _loggingOptions;
        [CanBeNull] private readonly ISyncLocalStorageService _localStorage;
        private readonly bool _useNameMatching;
        private readonly ConcurrentDictionary<IFileContextScopedOptions, IFileContextStore> _namedStores;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public LocalStorageStoreCache(
            [NotNull] ILoggingOptions loggingOptions,
            [CanBeNull] IFileContextSingletonOptions options,
            [CanBeNull] ISyncLocalStorageService localStorage)
        {
            _loggingOptions = loggingOptions;
            _localStorage = localStorage;

            if (options?.DatabaseRoot != null)
            {
                _useNameMatching = true;

                LazyInitializer.EnsureInitialized(
                    ref options.DatabaseRoot.Instance,
                    () => new ConcurrentDictionary<IFileContextScopedOptions, IFileContextStore>());

                _namedStores = (ConcurrentDictionary<IFileContextScopedOptions, IFileContextStore>)options.DatabaseRoot.Instance;
            }
            else
            {
                _namedStores = new ConcurrentDictionary<IFileContextScopedOptions, IFileContextStore>();
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IFileContextStore GetStore(IFileContextScopedOptions options)
        {
            return _namedStores.GetOrAdd(options, _ => new FileContextStore(new LocalStorageTableFactory(_loggingOptions, options, _localStorage), _useNameMatching));
        }
    }
}
