using Microsoft.AspNetCore.Hosting;
using WatcherService.Models;
using WatcherService.Services.IServices;
using System.Security.Cryptography;
using WatcherService.Models.DTO;
using System.Net;

namespace WatcherService.Services;

public class WatchFolderService : BackgroundService
{
    private readonly IWebHostEnvironment env;
    private FileSystemWatcher _fsw;
    private readonly IJwtTokenGenerator jwtTokenGenerator;
    private readonly IApiManageService apiManageService;
    private readonly IConfiguration configFile;
    private readonly IFileMover fileMover;
    ILogger<WatchFolderService> log;

    private string WatchedPath;
    private string issuer;
    private int exp;
    private string targetPath;
    private string Secret;
    private string enviromentKey;


    public WatchFolderService(ILogger<WatchFolderService> _log, IConfiguration _configFile, IWebHostEnvironment _env,
        IJwtTokenGenerator _jwtTokenGenerator, IApiManageService _apiManageService, IFileMover _fileMover)
    {
        env = _env;
        jwtTokenGenerator = _jwtTokenGenerator;
        apiManageService = _apiManageService;
        configFile = _configFile;
        fileMover = _fileMover;
        log = _log;


        enviromentKey = configFile["JWT_SECRET"];
        WatchedPath = configFile["WatchedPath"];
        issuer = configFile["Issuer"];
        exp = int.Parse(configFile["TokenTTLMinutes"]);
        targetPath = configFile["ProcessedPath"];
        Secret = configFile["Secret"];
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var folder = Path.Combine(env.ContentRootPath, WatchedPath);
        Directory.CreateDirectory(folder);

        _fsw = new FileSystemWatcher(folder)
        {
            Filter = "*.*",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };

        _fsw.Created += OnCreated;
        _fsw.Changed += OnChanged;
        stoppingToken.Register(() =>
        {
            if (_fsw != null)
            {
                _fsw.Created -= OnCreated;
                //_fsw.Changed -= OnChanged;
                _fsw.Dispose();
                _fsw = null;
            }
        });
        return Task.CompletedTask;
    }
    //public Task StartAsync(CancellationToken ct)
    //{
    //    var folder = Path.Combine(env.ContentRootPath, "watched");

    //    Directory.CreateDirectory(folder);

    //    _fsw = new FileSystemWatcher(folder)
    //    {
    //        NotifyFilter = NotifyFilters.FileName,
    //        EnableRaisingEvents = true
    //    };
    //    _fsw.Created += OnCreated;
    //    return Task.CompletedTask;
    //}

    //public Task StopAsync(CancellationToken ct)
    //{
    //    if (_fsw is not null)
    //    {
    //        _fsw.Created -= OnCreated;
    //        _fsw.Dispose();
    //        _fsw = null;
    //    }
    //    return Task.CompletedTask;
    //}

    private async void OnCreated(object? sender, FileSystemEventArgs e)
    {
        try
        {
            await Task.Delay(1000);
            await Process(e.FullPath);
        }
        catch (Exception ex)
        {
            log.LogError("OnCreated error: " + ex);
        }
    }
    private async void OnChanged(object? sender, FileSystemEventArgs e)
    {
        try
        {
            await Task.Delay(1000);
            await Process(e.FullPath);
        }
        catch (Exception ex)
        {
            log.LogError("OnChanged error: " + ex);
        }
    }


    private async Task Process(string fullPath)
    {
        try
        {
            string token;
            var fi = new FileInfo(fullPath);

            string hash;
            using (var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                hash = Sha256Hex(stream);
            }

            FileMetadata metadata = new FileMetadata
            {
                filename = fi.Name,
                created_at = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                file_size = fi.Length,
                hash = hash
            };

            //var issuer = configFile["Issuer"];
            //var exp = configFile["TokenTTLMinutes"];
            //var targetPath = configFile["ProcessedPath"];
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(exp.ToString()) || string.IsNullOrWhiteSpace(targetPath))
            {
                log.LogError("Configuration values are missing or invalid.");
                return;
            }

            var signingKey = string.IsNullOrWhiteSpace(enviromentKey) ? Secret : enviromentKey;

            token = jwtTokenGenerator.CreateToken(issuer, exp, signingKey);
            

            var result = await apiManageService.PostData(metadata, token);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var folder = Path.Combine(env.ContentRootPath, targetPath);

                Directory.CreateDirectory(folder);

                fileMover.MoveFile(fullPath, Path.Combine(env.ContentRootPath, targetPath), fi.Name);
                fileMover.CountFiles_Delete_LIMIT(folder, 5);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing file in Process function" + ex);
        }
    }

    private static string Sha256Hex(Stream s)//hash of the file  i used sha256 
    {
        using var sha = SHA256.Create();
        s.Position = 0;
        return Convert.ToHexString(sha.ComputeHash(s)).ToLowerInvariant();
    }
}
