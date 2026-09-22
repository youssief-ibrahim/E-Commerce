using Hangfire;
using Hangfire.SqlServer;

namespace E_Commerce.Web.Extentions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            // Hangfire
            services.AddHangfire(config =>
            {
                config.UseRecommendedSerializerSettings();

                config.SetDataCompatibilityLevel(
                    CompatibilityLevel.Version_170);

                config.UseSimpleAssemblyNameTypeSerializer()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"),
                        new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout =TimeSpan.FromMinutes(5),

                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),

                            QueuePollInterval = TimeSpan.Zero,

                            UseRecommendedIsolationLevel = true,

                            DisableGlobalLocks = true
                        });
            });

            services.AddHangfireServer();

            return services;
        }
    }
}
