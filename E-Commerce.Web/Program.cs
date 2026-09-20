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
            builder.Services.AddDbContext<EcomerceDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddKeyedScoped<IDataInitializer, DataInitializer>("Default");
            builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitilaizer>("Identity");
            builder.Services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = true;
                opt.Password.RequiredLength = 6;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>().AddEntityFrameworkStores<EcomerceDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

            builder.Services.AddAutoMapper(cfg => { }, typeof(ProductService).Assembly);
            builder.Services.Configure<ApiBehaviorOptions>(option =>
            {
                option.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;
            });

            #region Services

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddSingleton<IConnectionMultiplexer>(s =>
            {
                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!);
            });
            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularDev", policy =>
                    policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            // hangfire configuration
            builder.Services.AddHangfire(config =>
            {
                config.UseRecommendedSerializerSettings();
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                config.UseSimpleAssemblyNameTypeSerializer()
                .UseSqlServerStorage(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new Hangfire.SqlServer.SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true, // Concurrency
                    DisableGlobalLocks = true
                });
            });
            builder.Services.AddHangfireServer();

            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUserService, UserService>();

            var jwtSecret = builder.Configuration["Jwt:SecretKey"];
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.RequireHttpsMetadata = true;
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!))
                };

            });

            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ECommerce PlatForm",
                    Description = "Api for Restaurant"
                });
                
                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the tex"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                             {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                            }
                         },
                         new string[] {}
                     }
                });
            });

            #endregion

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
