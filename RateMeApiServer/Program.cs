using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RateMeApiServer.Data;
using RateMeApiServer.Repositories;
using RateMeApiServer.Services;

namespace RateMeApiServer;

class Program
{
    static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        // Add services to the container.
        builder.Services.AddHttpLogging(_ => { });

        builder.Services.AddControllers();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
        builder.Services.AddScoped<ISubjectService, SubjectService>();
        builder.Services.AddScoped<IElementsRepository, ElementsRepository>();
        builder.Services.AddScoped<IElementService, ElementService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        WebApplication app = builder.Build();

        app.UseHttpLogging();
            
            
        // Migrations with retries in case postgres is not ready yet

        using IServiceScope scope = app.Services.CreateScope();
        using AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
        const int maxRetries = 10;
        int retries = 0;
        bool migrated = false;

        while (!migrated && retries < maxRetries)
        {
            try
            {
                dbContext.Database.Migrate();
                migrated = true;
            }
            catch (Npgsql.PostgresException e) when (e.SqlState == "57P03") // DB starting up
            {
                retries++;
                Console.WriteLine($"PostgreSQL not ready yet (try {retries}/{maxRetries}), waiting...");
                Thread.Sleep(2000);
            }
            catch (Exception e)
            {
                Console.WriteLine("Migration failed: " + e);
                throw;
            }
        }

        // Configure the HTTP request pipeline.
            
        // For now swagger always available
        //if (app.Environment.IsDevelopment()) 
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGet("/", () => "Hello World!");

        app.UseAuthorization();

        app.MapControllers();
        
        Console.WriteLine("Hello world");

        app.Run();
    }
}