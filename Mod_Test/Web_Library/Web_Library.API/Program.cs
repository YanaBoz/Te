using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Web_Library.Data;
using Web_Library.API.Middleware;
using Web_Library.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

// Настройка маппинга
MappingConfig.ConfigureMappings();

// Настройка контекста базы данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация пользовательских сервисов
builder.Services.AddCustomServices(builder.Configuration);

// Настройка FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Настройка авторизации и логирования
builder.Services.AddAuthorization();
builder.Services.AddLogging();

// Добавление контроллеров
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Настройка Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Description = "Введите JWT токен"
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Применение миграций и начальная загрузка данных
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        dbContext.Database.Migrate();
        DbSeeder.Seed(dbContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration failed: {ex.Message}");
    }
}

// Использование промежуточного ПО для обработки исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Логгирование запросов
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

// Настройка HTTPS и CORS
app.UseHttpsRedirection();

app.UseCors(policy => policy
    .WithOrigins("http://localhost:5005")
    .AllowAnyHeader()
    .AllowAnyMethod()
);

// Настройка аутентификации и авторизации
app.UseAuthentication();
app.UseAuthorization();

// Добавление Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Настройка маршрутизации
app.MapControllers();

// Запуск приложения
app.Run();