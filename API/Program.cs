var builder = WebApplication.CreateBuilder(args);

// add services to container - ConfigureServices in .NET 5 

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddCors();
builder.Services.AddControllers();

// Configure the http request pipeline
var app = builder.Build();

// use our custom middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// add cors to accept the requests from Angular running in localhost:4200
app.UseCors(policy => policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials() // for signalR
                            .WithOrigins("https://localhost:4200"));

// middleware for authentication
app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
// SignalR routes
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
app.MapFallbackToController("Index", "Fallback"); // Angular routes.

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// to pass in context to the seed class seeduser() method and populate db with dummy data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>(); // create UserManager of type AppUser
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync(); // used to apply all migrations to db(in case no db, create db)
    await Seed.SeedUser(userManager, roleManager); // pass in the userManager to SeedUser to insert the seed data to the AppUser table.
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

await app.RunAsync();