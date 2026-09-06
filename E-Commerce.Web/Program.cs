
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Persistence.Data.DbContext;
using E_Commerce.Persistence.Repository;
using E_Commerce.Web.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Web
{
    public class Program
    {
        public static async void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<EcomerceDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>().AddEntityFrameworkStores<EcomerceDbContext>();

           
            #region Services

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            #endregion

            var app = builder.Build();
            #region Seding Data
            await app.MigrateDatabaseAsync();
            await app.SeedDataAsync();
            #endregion
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
