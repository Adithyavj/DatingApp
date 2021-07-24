using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    // An extension method of IServiceCollection 
    // We write all our personal services here
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // add service for DbContext
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            // add service for CORS
            services.AddCors();

            // Personal Services
            // we need to specify its lifetime (how long this should be alive for once we start it)
            // the one most suitable for an http request is AddScoped. This start when an http request comes in and lives till the http request is finished.
            // AddScoped is scoped to the lifetime of the httprequest
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}