using CP4.Api.Middlewares;
using CP4.Application.Interfaces;
using CP4.Application.Services;
using CP4.Domain.Interfaces;
using CP4.Infrastructure.Data;
using CP4.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. OBSERVABILIDADE: Application Insights (Tracing e Métricas)
// ============================================================================
var appInsightsConnString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(appInsightsConnString))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = appInsightsConnString;
    });
}
else
{
    builder.Services.AddApplicationInsightsTelemetry();
}

// ============================================================================
// 2. CONTROLADORES
// ============================================================================
builder.Services.AddControllers();

// ============================================================================
// 3. BANCO DE DADOS (Entity Framework Core com Oracle)
// ============================================================================
var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString) && !connectionString.Contains("SEU_RM"))
    {
        options.UseOracle(connectionString);
    }
    else
    {
        // Fallback em memória para testes e demonstração se não houver Oracle ativo
        options.UseInMemoryDatabase("CP4_Filmes_DevDb");
    }
});

// ============================================================================
// 4. INJEÇÃO DE DEPENDÊNCIA (Clean Architecture)
// ============================================================================
builder.Services.AddScoped<IFilmeRepository, FilmeRepository>();
builder.Services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();
builder.Services.AddScoped<IFilmeService, FilmeService>();
builder.Services.AddScoped<IAvaliacaoService, AvaliacaoService>();

// ============================================================================
// 5. OTIMIZAÇÃO: Response Compression (Compressão de Dados Brotli e Gzip)
// ============================================================================
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// ============================================================================
// 6. SEGURANÇA E PERFORMANCE: Rate Limiting (Retorna 429 Too Many Requests)
// ============================================================================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Política 'fixed': Permite até 10 requisições a cada 10 segundos por cliente
    // (ideal para demonstrar o 429 facilmente durante a apresentação!)
    options.AddPolicy("fixed", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"statusCode\": 429, \"message\": \"Limite de requisicoes excedido. Tente novamente em alguns segundos.\"}",
            cancellationToken: token);
    };
});

// ============================================================================
// 7. OBSERVABILIDADE: Health Checks (Monitora a aplicação e o DbContext)
// ============================================================================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("Banco_De_Dados");

// ============================================================================
// 8. DOCUMENTAÇÃO AVANÇADA COM SWAGGER / OPENAPI ATTRIBUTES
// ============================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CP4 - API de Filmes e Avaliações",
        Version = "v1",
        Description = "API RESTful corporativa em .NET 8 com Clean Architecture, Paginação, Compressão de Dados, Rate Limiting, Health Checks e Application Insights.",
        Contact = new OpenApiContact
        {
            Name = "Grupo FIAP - 2TDSR 2026",
            Url = new Uri("https://www.fiap.com.br")
        }
    });

    c.EnableAnnotations();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// ============================================================================
// INICIALIZAÇÃO E SEED DO BANCO DE DADOS (Criação automática de tabelas e dados)
// ============================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DbInitializer.Initialize(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "ATENÇÃO: Falha ao inicializar o banco de dados e aplicar seed: {Message}", ex.Message);
    }
}

// ============================================================================
// PIPELINE HTTP
// ============================================================================

// Middleware Global de Tratamento de Exceções
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CP4 Filmes API v1");
    c.RoutePrefix = string.Empty; // Abre o Swagger diretamente na raiz (http://localhost:XXXX/)
});

// Response Compression
app.UseResponseCompression();

// Rate Limiter
app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapeamento dos Controllers
app.MapControllers();

// Endpoint de Health Check
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds + " ms",
            dependencies = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds + " ms"
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.Run();

// Necessário para os Testes de Integração com WebApplicationFactory
public partial class Program { }
