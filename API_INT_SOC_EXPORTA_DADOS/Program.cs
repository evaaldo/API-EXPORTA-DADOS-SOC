using API_INT_SOC_EXPORTA_DADOS.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<SocService>();

var app = builder.Build();

app.MapGet("/HealthCheck", (IConfiguration config) => $"API de Integração com o SOC funcionando corretamente! Ambiente: {builder.Environment.EnvironmentName}");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
