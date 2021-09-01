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
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            // add service for CORS
            services.AddCors();

            // Personal Services
            // we need to specify its lifetime (how long this should be alive for once we start it)
            // the one most suitable for an http request is AddScoped. This start when an http request comes in and lives till the http request is finished.
            // AddScoped is scoped to the lifetime of the httprequest
            services.AddScoped<ITokenService, TokenService>();

            // add dependency injection for photo service (we use cloudinary)
            services.AddScoped<IPhotoService, PhotoService>();

            // add dependency injection for repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

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