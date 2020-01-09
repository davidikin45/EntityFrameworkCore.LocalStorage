// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Blazored.LocalStorage;
using FileContextCore.Infrastructure.Internal;
using FileContextCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace EntityFrameworkCore.LocalStorage.Storage.Internal
{

    public class LocalStorageTableFactory
        // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
        : Microsoft.EntityFrameworkCore.ChangeTracking.Internal.IdentityMapFactoryFactoryBase, IFileContextTableFactory
    {
        private readonly IFileContextScopedOptions _options;
        private readonly bool _sensitiveLoggingEnabled;
        private readonly ISyncLocalStorageService _localStorage;

        private readonly ConcurrentDictionary<IKey, Func<IFileContextTable>> _factories
            = new ConcurrentDictionary<IKey, Func<IFileContextTable>>();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public LocalStorageTableFactory([NotNull] ILoggingOptions loggingOptions, [NotNull] IFileContextScopedOptions options, [NotNull] ISyncLocalStorageService localStorage)
        {
            _options = options;
            _sensitiveLoggingEnabled = loggingOptions.IsSensitiveDataLoggingEnabled;
            _localStorage = localStorage;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IFileContextTable Create(IEntityType entityType)
            => _factories.GetOrAdd(entityType.FindPrimaryKey(), Create)();

        private Func<IFileContextTable> Create([NotNull] IKey key)
            => (Func<IFileContextTable>)typeof(LocalStorageTableFactory).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateFactory))
                .MakeGenericMethod(GetKeyType(key))
                .Invoke(null, new object[] { key, key.DeclaringEntityType, _sensitiveLoggingEnabled, _options, _localStorage });

        [UsedImplicitly]
        private static Func<IFileContextTable> CreateFactory<TKey>(IKey key, IEntityType entityType, bool sensitiveLoggingEnabled, IFileContextScopedOptions options, ISyncLocalStorageService localStorage)
            => () => new LocalStorageTable<TKey>(
                // WARNING: The in-memory provider is using EF internal code here. This should not be copied by other providers. See #15096
                Microsoft.EntityFrameworkCore.Metadata.Internal.KeyExtensions.GetPrincipalKeyValueFactory<TKey>(key),
                sensitiveLoggingEnabled,
                entityType,
                options,
                localStorage);
    }
}
