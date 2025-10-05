using WatcherService.Models;
using WatcherService.Services.IServices;
using WatcherService.Services;
using WatcherService.AutoMapper;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//****************************AutoMapper********************************
builder.Services.AddAutoMapper(typeof(MappingConfig));
//**********************************************************************

var contentRoot = builder.Environment.ContentRootPath; 
var filePath = Path.Combine(contentRoot, "configFile", "WatcherSettings.json");

builder.Configuration.AddJsonFile(filePath, optional: false, reloadOnChange: true);


//****************************DI services**********************************
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddHostedService<WatchFolderService>();

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IBaseService, BaseService>();
builder.Services.AddSingleton<IApiManageService, ApiManageService>();

builder.Services.AddSingleton<IFileMover, FileMover>();

builder.Services.AddHttpClient();
//**********************************************************************


//******************serylog***************************************

builder.Logging.ClearProviders();

var root = builder.Environment.ContentRootPath;
var filePathLog = Path.Combine(contentRoot, "logger/LoggerService-.log");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()                 
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File(
        path: filePathLog,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true)
    .CreateLogger();

builder.Host.UseSerilog();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
