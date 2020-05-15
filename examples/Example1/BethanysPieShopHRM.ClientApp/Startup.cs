using BethanysPieShopHRM.ClientApp.Interceptors;
using BethanysPieShopHRM.ClientApp.Services;
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

            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseLocalStorageDatabase(services.GetJSRuntime(), databaseName: "db");
            //});

            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseLocalStorageDatabase(services.GetJSRuntime(), databaseName: "db", password: "password");
            //});

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "db");
            });

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
