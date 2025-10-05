using LoggerService.Services;
using LoggerService.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//***************************DI***************************************
builder.Services.AddScoped<ILogFileCreator,LogFileCreator>();
//********************************************************************

var environmentKey = builder.Configuration["JWT_SECRET"];
var issuer = builder.Configuration["Jwt:Issuer"];
var secret = builder.Configuration["Jwt:Secret"];

var signingKey = string.IsNullOrEmpty(environmentKey) ? secret : environmentKey;


var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(signingKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, 

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Logging.ClearProviders();

var contentRoot = builder.Environment.ContentRootPath;
var filePath = Path.Combine(contentRoot, "logger/LoggerService-.log");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()                 
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File(
        path: filePath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
