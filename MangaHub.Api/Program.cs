using MangaHub.Api.Data;
using MangaHub.Api.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// La cadena de conexión viene de la variable de entorno ConnectionStrings__Default.
// AddDbContext registra el DbContext como Scoped (una instancia por petición HTTP).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Los repositorios también son Scoped, porque dependen del DbContext.
builder.Services.AddScoped<IMangaRepository, MangaRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();

var app = builder.Build();

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
