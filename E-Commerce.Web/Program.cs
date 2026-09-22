using System.Text;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Persistence.Data.DataSeed;
using E_Commerce.Persistence.Data.DbContext;
using E_Commerce.Persistence.Repository;
using E_Commerce.Services;
using E_Commerce.Services_Abstraction;
using E_Commerce.Web.CustomMiddleWare;
using E_Commerce.Web.Extentions;
using E_Commerce.Web.Factory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Hangfire;
using E_Commerce.Web.Jobs;
using E_Commerce.Persistence.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using E_Commerce.Persistence.Data.DataExtensions;

namespace E_Commerce.Web
{
    public class Program
    {
        public static  async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Application Services  Database  Redis  CORS
            builder.Services.AddApplicationServices(builder.Configuration);
            // Identity + Data Initializers
            builder.Services.AddIdentityServices();

            builder.Services.AddRateLimiting();

            builder.Services.AddJwtAuthentication(builder.Configuration);
            // Hangfire
            builder.Services.AddInfrastructureServices(builder.Configuration);
            
            builder.Services.AddSwaggerDocumentation();

            var app = builder.Build();
            #region Seding Data
            await app.MigrateDatabaseAsync();
            await app.SeedDataAsync();
            await app.SeedIdentityDataAsync();
            #endregion
            app.UseMiddleware<ExceptionHandlerMiddleWare>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseCors("AngularDev");
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHangfireDashboard();

           // Register recurring Hangfire job to clean up expired refresh tokens every hour
           var jobManager = app.Services.GetRequiredService<IRecurringJobManager>();
            jobManager.AddOrUpdate<RefreshTokenCleanupJob>(
                recurringJobId: "cleanup-expired-refresh-tokens",
                methodCall: job => job.ExecuteAsync(),
                cronExpression: Cron.Hourly(1));

            app.Run();
        }
    }
}
