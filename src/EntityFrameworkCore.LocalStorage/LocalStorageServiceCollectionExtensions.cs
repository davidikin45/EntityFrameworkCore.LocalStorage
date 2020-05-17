// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Blazored.LocalStorage;
using EntityFrameworkCore.LocalStorage;
using EntityFrameworkCore.LocalStorage.Storage.Internal;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.JSInterop;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalStorageServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkLocalStorageDatabase(this IServiceCollection serviceCollection, IJSRuntime jSRuntime, LocalStorageOptions options)
        {
            serviceCollection.AddSingleton<IJSRuntime>(jSRuntime);
            serviceCollection.AddSingleton(options);

            //Override ProviderSpecificServices

            serviceCollection.AddSingleton<IInMemoryStoreCache, SerializableStoreCache>();
            serviceCollection.AddSingleton<IInMemoryTableFactory, SerializableTableFactory>();

            serviceCollection.AddBlazoredLocalStorage();

            serviceCollection.AddEntityFrameworkInMemoryDatabase();

            return serviceCollection;
        }

        public static IJSRuntime GetJSRuntime(this IServiceCollection serviceCollection)
        {
            return (IJSRuntime)serviceCollection.Where(sd => sd.ServiceType == typeof(IJSRuntime)).First().ImplementationInstance;
        }
    }
}
