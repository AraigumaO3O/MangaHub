using System.Reflection;
using Asp.Versioning;
using MangaHub.Api.Data;
using MangaHub.Api.Middleware;
using MangaHub.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Versionamiento por URL: /api/v1/...
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Documentación Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MangaHub API",
        Version = "v1",
        Description = "API REST para gestionar un catálogo de mangas y la biblioteca personal de cada lector."
    });

    // Comentarios /// de los controladores (requiere GenerateDocumentationFile en el .csproj)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Botón "Authorize" para pegar el token JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega aquí el token JWT obtenido en /api/v1/auth/login"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// La cadena de conexión viene de la variable de entorno ConnectionStrings__Default.
// AddDbContext registra el DbContext como Scoped (una instancia por petición HTTP).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Los repositorios también son Scoped, porque dependen del DbContext.
builder.Services.AddScoped<IMangaRepository, MangaRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();

var app = builder.Build();

// Middleware propio de logging (antes del manejador de errores, para registrar también los 500)
app.UseMiddleware<RequestLoggingMiddleware>();

// Errores no controlados -> respuesta ProblemDetails (JSON) con código 500
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
