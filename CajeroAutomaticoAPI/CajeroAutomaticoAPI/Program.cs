using CajeroAutomaticoAPI.DAOs;
using CajeroAutomaticoAPI.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddPolicy("Blazor", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Cajero Automático API", Version = "v1" });
});

// DAOs
builder.Services.AddScoped<CuentahabienteDAO>();
builder.Services.AddScoped<CuentaDAO>();
builder.Services.AddScoped<TarjetaDAO>();
builder.Services.AddScoped<TransaccionDAO>();
builder.Services.AddScoped<BitacoraDAO>();

// Services
builder.Services.AddScoped<CajeroAutomaticoAPI.Services.CuentahabienteService>();
builder.Services.AddScoped<CajeroAutomaticoAPI.Services.CuentaService>();
builder.Services.AddScoped<CajeroAutomaticoAPI.Services.TarjetaService>();
builder.Services.AddScoped<CajeroAutomaticoAPI.Services.TransaccionService>();
builder.Services.AddScoped<CajeroAutomaticoAPI.Services.BitacoraService>();

var app = builder.Build();

app.UseCors("Blazor");
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cajero Automático API v1"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Manejo global de errores SQL
app.Use(async (context, next) =>
{
    try { await next(); }
    catch (SqlException ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
    }
});

app.Run();
