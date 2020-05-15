// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Blazored.LocalStorage;
using EntityFrameworkCore.LocalStorage.NewFolder;
using FileContextCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.JSInterop;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalStorageServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkLocalStorageDatabase([NotNull] this IServiceCollection serviceCollection, IJSRuntime jSRuntime)
        {
            serviceCollection.AddSingleton<IJSRuntime>(jSRuntime);

            serviceCollection.AddSingleton<IFileContextStoreCache, LocalStorageStoreCache>();

            serviceCollection.AddBlazoredLocalStorage();

            serviceCollection.AddEntityFrameworkFileContextDatabase();

            return serviceCollection;
        }

        public static IJSRuntime GetJSRuntime([NotNull] this IServiceCollection serviceCollection)
        {
            return (IJSRuntime)serviceCollection.Where(sd => sd.ServiceType == typeof(IJSRuntime)).First().ImplementationInstance;
        }
    }
}
