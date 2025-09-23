using Aplication.UseCases.Location;
using Infraestructure.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Riders.Config.ExternalServicesInjectionsConfig;
using Riders.Config.Injections;
using Riders.Config.Injections.Singleton;
using Riders.Config.Middleware;
using Riders.Config.Token;
using Riders.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(conn, npgsql =>
    {
        npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorCodesToAdd: null);
    });
});



builder.Services.AddHealthChecks();
builder.Services.AddSignalR();
builder.Services.AddRepositoryesInjections();
builder.Services.AddServicesInjections();
builder.Services.AddManageHubInjetions();
builder.Services.AddExternalServices(builder.Configuration);
builder.Services.AddControllers();

builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Limpa para aceitar cabeçalhos de qualquer proxy (ou adicione IP do host se quiser restringir)
    o.KnownNetworks.Clear();
    o.KnownProxies.Clear();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("https://api.riders.lat") // aceitar apenas via Traefik
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    options.AddPolicy("PublicForHub", policyBuilder =>
    {
        policyBuilder
            .SetIsOriginAllowed(_ => true) // permitir qualquer origem
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

});

// Configurar JWT passando a configuration
builder.Services.AddTokenService(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        logger.LogInformation("Migrations aplicadas com sucesso.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Falha ao aplicar migrations.");
        throw; // falha o container para reiniciar e tentar novamente
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseErrorHandling();

app.UseHttpsRedirection();

app.UseForwardedHeaders();

app.UseCors("CorsPolicy");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
HubsRegistration.MapAllHubs(app);
app.MapHealthChecks("/health"); // Endpoint de health check
app.Run();
