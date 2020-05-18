using BethanysPieShopHRM.ClientApp.Interceptors;
using BethanysPieShopHRM.ClientApp.Services;
using EntityFrameworkCore.LocalStorage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace BethanysPieShopHRM.ClientApp
{


    public class Startup
    {
        public void ConfigureServices(IConfiguration configuration, IServiceCollection services, string baseAddress)
        {
            //services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
            //services.AddSingleton<AppIndexedDb>(sp => sp.GetRequiredService<IIndexedDbFactory>().Create<AppIndexedDb>().Result);

            //https://docs.microsoft.com/en-us/aspnet/core/blazor/dependency-injection?view=aspnetcore-3.1
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseLocalStorageDatabase(services.GetJSRuntime(), databaseName: "db");
            }, ServiceLifetime.Transient);

            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseLocalStorageDatabase(services.GetJSRuntime(), databaseName: "db", password: "password");
            //}, ServiceLifetime.Transient);

            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseInMemoryDatabase(databaseName: "db");
            //}, ServiceLifetime.Transient);

            //var connection = new SqliteConnection("DataSource=:memory:");
            //connection.Open();
            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseSqlite(connection);
            //}, ServiceLifetime.Singleton);

            //Spinner
            services.AddScoped<ISpinnerService, SpinnerService>();
            services.AddScoped<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
            //Type MonoWasmHttpMessageHandlerType = Assembly.Load("WebAssembly.Net.Http").GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");
            //services.AddScoped(MonoWasmHttpMessageHandlerType);

            //HttpClient Factory does not work with Client side blazor
            services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
            //services.Remove(services.Single(x => x.ServiceType == typeof(HttpClient)));
     
            //services.AddTransient<HttpClient>(s =>
            //{
            //    var blazorDisplaySpinnerAutomaticallyHttpMessageHandler = s.GetRequiredService<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
            //    blazorDisplaySpinnerAutomaticallyHttpMessageHandler.InnerHandler = (HttpMessageHandler)s.GetService(MonoWasmHttpMessageHandlerType);

            //    var client = new HttpClient(blazorDisplaySpinnerAutomaticallyHttpMessageHandler) { BaseAddress = new Uri(baseAddress) };
            //    return client;
            //});


        }
    }
}
