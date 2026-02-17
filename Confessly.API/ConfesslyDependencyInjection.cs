using Confessly.Configuration;
using Confessly.Contracts.Authentication;
using Confessly.Domain;
using Confessly.Repository;
using Confessly.Repository.Core;
using Microsoft.EntityFrameworkCore;

namespace Confessly.API
{
    public static class ConfesslyDependencyInjection
    {
        public static IServiceCollection AddConfesslyDependencyInjection(
            this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddTransient<IUserContext, UserContext>();

            #region DbContext
            services.AddDbContextPool<ConfesslyDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                options.UseNpgsql(ConfesslyConfiguration.ConnectionString("Confessly"));
            });
            #endregion

            #region Repositories
            services.AddScoped<IRepository<User>, EntityRepository<User>>();
            #endregion

            #region UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            return services;
        }
    }
}
