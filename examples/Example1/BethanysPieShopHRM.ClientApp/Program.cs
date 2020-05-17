using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            new Startup().ConfigureServices(builder.Configuration, builder.Services, builder.HostEnvironment.BaseAddress);
            builder.RootComponents.Add<App>("app");

            var host = builder.Build();

            //Seed Data
            using (var scope = host.Services.CreateScope())
            {
                using var context = scope.ServiceProvider.GetService<AppDbContext>();
                await context.Database.EnsureCreatedAsync();
            }

            await host.RunAsync();
        }
    }
}
