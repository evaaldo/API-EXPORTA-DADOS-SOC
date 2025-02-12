using API_INT_SOC_EXPORTA_DADOS.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<SocService>();

var app = builder.Build();

app.MapGet("/HealthCheck", () => "API de Integração com o SOC funcionando corretamente!");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
