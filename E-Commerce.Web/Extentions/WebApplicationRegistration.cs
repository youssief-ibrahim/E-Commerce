using E_Commerce.Domain.Contracts;
using E_Commerce.Persistence.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Web.Extentions
{
    public static class WebApplicationRegistration
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EcomerceDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await dbContext.Database.MigrateAsync();
            }
            return app;
        }
        public static async Task<WebApplication> SeedDataAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            //var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
            var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Default");
            await dataInitializer.InitializeAsync();
            return app;
        }
        //public static async Task<WebApplication> SeedIdentityDataAsync(this WebApplication app)
        //{
        //    await using var scope = app.Services.CreateAsyncScope();
        //    //var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
        //    var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Identity");
        //    await dataInitializer.InitializeAsync();
        //    return app;
        //}
    }
}
