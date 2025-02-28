using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Web_Library.Data;
using Web_Library.API.Middleware;
using Web_Library.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

// ��������� ��������
MappingConfig.ConfigureMappings();

// ��������� ��������� ���� ������
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����������� ���������������� ��������
builder.Services.AddCustomServices(builder.Configuration);

// ��������� FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ��������� ����������� � �����������
builder.Services.AddAuthorization();
builder.Services.AddLogging();

// ���������� ������������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ��������� Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Description = "������� JWT �����"
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

// ���������� �������� � ��������� �������� ������
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

// ������������� �������������� �� ��� ��������� ����������
app.UseMiddleware<ExceptionHandlingMiddleware>();

// ������������ ��������
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

// ��������� HTTPS � CORS
app.UseHttpsRedirection();

app.UseCors(policy => policy
    .WithOrigins("http://localhost:5005")
    .AllowAnyHeader()
    .AllowAnyMethod()
);

// ��������� �������������� � �����������
app.UseAuthentication();
app.UseAuthorization();

// ���������� Swagger � ������ ����������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ��������� �������������
app.MapControllers();

// ������ ����������
app.Run();