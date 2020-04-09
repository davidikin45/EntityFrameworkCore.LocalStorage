using BethanysPieShopHRM.ClientApp.Interceptors;
using BethanysPieShopHRM.ClientApp.Services;
using Blazor.IndexedDB.Framework;
using EntityFrameworkCore.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace BethanysPieShopHRM.ClientApp
{


    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
            services.AddSingleton<AppIndexedDb>(sp => sp.GetRequiredService<IIndexedDbFactory>().Create<AppIndexedDb>().Result);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseLocalStorageDatabase(services.GetJSRuntime(), databaseName: "db");
            });

            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseLocalStorageDatabase(services.GetJSRuntime(), databaseName: "db", password: "password");
            //});

            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseInMemoryDatabase(databaseName: "db").ForBlazorWebAssembly();
            //});

            //Spinner
            services.AddScoped<ISpinnerService, SpinnerService>();
            services.AddScoped<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
            Type MonoWasmHttpMessageHandlerType = Assembly.Load("WebAssembly.Net.Http").GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");
            services.AddScoped(MonoWasmHttpMessageHandlerType);

            //HttpClient Factory does not work with Client side blazor
            services.AddBaseAddressHttpClient();
            services.Remove(services.Single(x => x.ServiceType == typeof(HttpClient)));

            services.AddScoped<HttpClient>(s =>
            {
                var blazorDisplaySpinnerAutomaticallyHttpMessageHandler = s.GetRequiredService<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
                blazorDisplaySpinnerAutomaticallyHttpMessageHandler.InnerHandler = (HttpMessageHandler)s.GetService(MonoWasmHttpMessageHandlerType);

                var client = new HttpClient(blazorDisplaySpinnerAutomaticallyHttpMessageHandler) { BaseAddress = new System.Uri("https://localhost:44340/") };
                return client;
            });


        }
    }
}
