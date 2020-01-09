using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BethanysPieShopHRM.ClientApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
    
            //Seed Data
            using(var scope = host.Services.CreateScope())
            {
                using var context = scope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();
            }

            host.Run();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
