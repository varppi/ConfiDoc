using Confidoc.Server;
using Confidoc.Server.Database;
using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Elasticsearch;


Configuration.InitConfigs();


var builder = WebApplication.CreateBuilder(args);

// Logging
var logConfig = new LoggerConfiguration();
switch(Configuration.GetEnvVariable("LOG_LEVEL"))
{
    case "debug":
        logConfig = logConfig.MinimumLevel.Debug();
        break;
    case "info":
        logConfig = logConfig.MinimumLevel.Information();
        break;  
    case "error":
        logConfig = logConfig.MinimumLevel.Error();
        break; 
    default:
        logConfig = logConfig.MinimumLevel.Information();
        break;
}
switch(Configuration.GetEnvVariable("LOG_TYPE"))
{
    case "console":
        logConfig = logConfig.WriteTo.Console();
        break;
    case "elasticsearch":
        logConfig = logConfig.WriteTo.Elasticsearch(
            new ElasticsearchSinkOptions(new Uri(Configuration.GetEnvVariable("LOG_OUT")))
        );
        break;
    case "file":
        logConfig = logConfig.WriteTo.File(Configuration.GetEnvVariable("LOG_OUT"));
        break;
    default:
        logConfig = logConfig.WriteTo.Console();
        break;
}
Log.Logger = logConfig.CreateLogger();
builder.Services.AddSerilog(Log.Logger);

// Database
var db = Configuration.GetEnvVariable("CONFIDOC_DATABASE");
Log.Debug($"env database is \"{db}\"");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (db)
    {
        case "sqlite":
            options.UseSqlite(Configuration.GetEnvVariable("CONFIDOC_CONNECTION"));
            break;

        default:
            throw new Exception("no database or an invalid one was selected. please check the documentation for configuration instructions.");
    }
});
builder.Services.AddIdentity<ConfidocUser, ConfidocRole>(options =>
{
    options.Password.RequireDigit           = Configuration.GetEnvVariable("PASSWORD_REQUIRE_DIGITS") == "true";
    options.Password.RequireNonAlphanumeric = Configuration.GetEnvVariable("PASSWORD_REQUIRE_NONALPHA") == "true";
    options.Password.RequireUppercase       = Configuration.GetEnvVariable("PASSWORD_REQUIRE_UPPER") == "true";
    options.Password.RequireLowercase       = Configuration.GetEnvVariable("PASSWORD_REQUIRE_LOWER") == "true";

}).AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = false,
            ValidateIssuer   = false,
            ValidateLifetime = false,
            ValidAudience    = Configuration.GetEnvVariable("CONFIDOC_JWT_AUDIENCE"),
            ValidIssuer      = Configuration.GetEnvVariable("CONFIDOC_JWT_ISSUER"),
            ClockSkew        = TimeSpan.FromMinutes(1),
            IssuerSigningKey = Jwt.GetSecretKey(),
        };
    });

// Confidoc
builder.Services.AddConfidocActions();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Observability
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.MapFallbackToFile("/index.html");

app.Run();
