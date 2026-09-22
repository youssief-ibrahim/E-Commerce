using E_Commerce.Domain.Contracts;
using E_Commerce.Persistence.Data.DbContext;
using E_Commerce.Persistence.Email;
using E_Commerce.Persistence.Repository;
using E_Commerce.Services;
using E_Commerce.Services_Abstraction;
using E_Commerce.Web.Factory;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace E_Commerce.Web.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Database Redis CORS
            //  Database
            services.AddDbContext<EcomerceDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            //  Rides
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var connectionString = configuration.GetConnectionString("RedisConnection");

                return ConnectionMultiplexer.Connect(connectionString!);
            });

            //  CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AngularDev", policy =>
                {
                    policy.WithOrigins(configuration["Cunsumer:Url"]!)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            #endregion


            #region Repository and UnitofWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            #endregion


            #region Servics
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailSender, EmailSender>();
            #endregion


            #region AutoMapper and Validation

            services.AddAutoMapper(cfg => { },typeof(ProductService).Assembly);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory =ApiResponseFactory.GenerateApiValidationResponse;
            });
            #endregion

            return services;
        }
    }
}
