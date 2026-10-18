using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Asp.Versioning;
using MangaHub.Api.Data;
using MangaHub.Api.Middleware;
using MangaHub.Api.Repositories;
using MangaHub.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Los enums (EstadoLectura) se envían y reciben como texto: "Pendiente", "Leyendo", "Terminado"
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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
        Description = "Pega aquí el token JWT obtenido en /api/v1/Auth/login"
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

// Autenticación JWT. La llave, el emisor y la audiencia vienen de variables de entorno.
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta la variable de entorno Jwt__Key.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // los claims se leen con el nombre que tienen en el token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = "sub",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// La cadena de conexión viene de la variable de entorno ConnectionStrings__Default.
// AddDbContext registra el DbContext como Scoped (una instancia por petición HTTP).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Los repositorios son Scoped, porque dependen del DbContext.
builder.Services.AddScoped<IMangaRepository, MangaRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();

// El servicio de tokens no guarda estado ni usa el DbContext, así que puede ser Singleton.
builder.Services.AddSingleton<ITokenService, TokenService>();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Crea el usuario Admin la primera vez (si las variables Seed__* están definidas)
await DbSeeder.SeedAdminAsync(app.Services, app.Configuration);

app.Run();
