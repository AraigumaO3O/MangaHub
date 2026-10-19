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

// enums como texto en el JSON
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();

// versionamiento por URL
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

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MangaHub API",
        Version = "v1",
        Description = "API REST para gestionar un catálogo de mangas y la biblioteca personal de cada lector."
    });

    // comentarios /// de los controladores
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // boton Authorize para el token
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

// autenticacion JWT, los valores vienen de variables de entorno
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta la variable de entorno Jwt__Key.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
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

// la cadena de conexion viene de una variable de entorno (el DbContext queda Scoped)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// repositorios Scoped, igual que el DbContext
builder.Services.AddScoped<IMangaRepository, MangaRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();

// Singleton porque no guarda estado
builder.Services.AddSingleton<ITokenService, TokenService>();

var app = builder.Build();

// middleware propio, antes del manejo de errores para que registre tambien los 500
app.UseMiddleware<RequestLoggingMiddleware>();

// errores no controlados -> ProblemDetails
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

// crea el admin la primera vez (si estan las variables Seed__)
await DbSeeder.SeedAdminAsync(app.Services, app.Configuration);

app.Run();
