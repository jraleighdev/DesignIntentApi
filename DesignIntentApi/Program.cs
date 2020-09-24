using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.Identity;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DesignIntentApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var identityContext = services.GetRequiredService<AppIdentityContext>();

                    var dataContext = services.GetRequiredService<DataContext>();

                    await dataContext.Database.MigrateAsync();
                    await identityContext.Database.MigrateAsync();
                    await AppIdentitySeed.SeedUserAsync(userManager);

                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();

                    logger.LogError(ex, "An exception occured during migration");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
