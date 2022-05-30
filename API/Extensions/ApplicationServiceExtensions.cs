using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
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
                // // Sqlite connection
                // options.UseSqlite(config.GetConnectionString("DefaultConnection"));
                // // Postgres connection
                // options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
                
                // Heroku Connection string:
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    //connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;Trust Server Certificate=true";

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}; SSL Mode=Require; Trust Server Certificate=true";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseNpgsql(connStr);
            });

            // Personal Services
            // we need to specify its lifetime (how long this should be alive for once we start it)
            // the one most suitable for an http request is AddScoped. This start when an http request comes in and lives till the http request is finished.
            // AddScoped is scoped to the lifetime of the httprequest
            services.AddScoped<ITokenService, TokenService>();

            // add dependency injection for photo service (we use cloudinary)
            services.AddScoped<IPhotoService, PhotoService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Adding automapper DI
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            // Adding service for cloudinarySettings- for accessing the settings via the CloudinarySettings class
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // Adding service to set the user's lastactive date
            services.AddScoped<LogUserActivity>();

            // Add service for presence tracker (SignalR) to be shared among all connections coming to server
            services.AddSingleton<PresenceTracker>();

            return services;
        }
    }
}