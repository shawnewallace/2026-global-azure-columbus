using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using TaskLibrary.Api;
using TaskLibrary.Api.Task;
using TaskLibrary.Infrastructure.Task;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? throw new InvalidOperationException("No database connection string configured. Set POSTGRES_CONNECTION_STRING or ConnectionStrings:DefaultConnection.");

builder.Services.AddDbContext<TaskLibraryDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddTaskServices(builder.Configuration);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:5173"])
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Logging.Configure(o =>
{
    o.ActivityTrackingOptions = ActivityTrackingOptions.SpanId | ActivityTrackingOptions.TraceId;
});

var otelBuilder = builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithLogging(logging => logging.AddOtlpExporter());

var aiConnStr = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
if (!string.IsNullOrEmpty(aiConnStr))
{
    otelBuilder
        .WithTracing(t => t.AddAzureMonitorTraceExporter(o => o.ConnectionString = aiConnStr))
        .WithMetrics(m => m.AddAzureMonitorMetricExporter(o => o.ConnectionString = aiConnStr))
        .WithLogging(l => l.AddAzureMonitorLogExporter(o => o.ConnectionString = aiConnStr));
}

var app = builder.Build();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors();
app.UseMiddleware<CorrelationIdMiddleware>();
app.RegisterEndpoints();

await app.RunAsync();

public partial class Program { }
