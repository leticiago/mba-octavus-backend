using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octavus.Infra.Persistence;
using System;

namespace Octavus.Infra.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnectionString = configuration.GetConnectionString("SqlDefault");

            if (string.IsNullOrWhiteSpace(sqlConnectionString))
            {
                throw new InvalidOperationException("Connection string 'SqlDefault' is missing.");
            }

            services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(sqlConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Context).Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null
                    );
                });

                options.EnableSensitiveDataLogging();
            });
        }

    }
}