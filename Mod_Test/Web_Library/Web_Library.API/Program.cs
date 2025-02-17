using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Web_Library.API.Data;
using Web_Library.API.Middleware;
using Web_Library.API.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure MS SQL Server connection
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register custom services
        builder.Services.AddCustomServices(builder.Configuration);

        // Add services to the container
        builder.Services.AddAuthorization();
        builder.Services.AddLogging();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Description = "Enter JWT token"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] { }
                }
            });
        });

        var app = builder.Build();

        // Apply migrations automatically
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        // Configure middleware
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Logging middleware
        app.Use(async (context, next) =>
        {
            Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
            await next.Invoke();
        });

        // Enable HTTPS redirection
        app.UseHttpsRedirection();

        // Enable CORS with specific origins
        app.UseCors(p =>
            p.WithOrigins("http://localhost:5005")  // ”казывайте точный адрес вашего UI
             .AllowAnyHeader()
             .AllowAnyMethod()
        );

        // Ensure that authentication is properly set up
        app.UseAuthentication();

        // Ensure that authorization is properly set up
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Map controllers
        app.MapControllers();

        // Run the application
        app.Run();
    }
}
