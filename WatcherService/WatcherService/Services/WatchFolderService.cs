using Microsoft.AspNetCore.Hosting;
using WatcherService.Models;
using WatcherService.Services.IServices;
using System.Security.Cryptography;
using WatcherService.Models.DTO;
using System.Net;
using Serilog;
using System.IO;

namespace WatcherService.Services;

public class WatchFolderService : BackgroundService
{
    private const int FileSettleDelayMs = 1000;

    private readonly IWebHostEnvironment env;
    private FileSystemWatcher? _fsw;
    private readonly IJwtTokenGenerator jwtTokenGenerator;
    private readonly IApiManageService apiManageService;
    private readonly IConfiguration configFile;
    private readonly IFileMover fileMover;
    private readonly ILogger<WatchFolderService> log;

    private readonly string watchedPath = string.Empty;
    private readonly string issuer = string.Empty;
    private readonly int exp;
    private readonly string targetPath = string.Empty;
    private readonly string secret = string.Empty;
    private readonly string? environmentKey;
    private readonly int limitFileToSave;

    public WatchFolderService(ILogger<WatchFolderService> _log, IConfiguration _configFile, IWebHostEnvironment _env,
        IJwtTokenGenerator _jwtTokenGenerator, IApiManageService _apiManageService, IFileMover _fileMover)
    {
        env = _env;
        jwtTokenGenerator = _jwtTokenGenerator;
        apiManageService = _apiManageService;
        configFile = _configFile;
        fileMover = _fileMover;
        log = _log;

        environmentKey = configFile["JWT_SECRET"];
        watchedPath = configFile.GetValue<string>("WatchedPath") ?? string.Empty;
        issuer = configFile.GetValue<string>("Issuer") ?? string.Empty;
        targetPath = configFile.GetValue<string>("ProcessedPath") ?? string.Empty;
        secret = configFile.GetValue<string>("Secret") ?? string.Empty;
        exp = configFile.GetValue<int>("TokenTTLMinutes");
        limitFileToSave = configFile.GetValue<int>("LimitFileToSave");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var folder = Path.Combine(env.ContentRootPath, watchedPath);
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
                _fsw.Changed -= OnChanged;
                _fsw.Dispose();
                _fsw = null;
            }
        });
        return Task.CompletedTask;
    }

    private async void OnCreated(object? sender, FileSystemEventArgs e)
    {
        try
        {
            await Task.Delay(FileSettleDelayMs);
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

            if (string.IsNullOrWhiteSpace(issuer) || exp <= 0 || string.IsNullOrWhiteSpace(targetPath))
            {
                log.LogError("Configuration values are missing or invalid.");
                return;
            }

            var signingKey = string.IsNullOrWhiteSpace(environmentKey) ? secret : environmentKey;
            token = jwtTokenGenerator.CreateToken(issuer, exp, signingKey);

            var result = await apiManageService.PostData(metadata, token);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var folder = Path.Combine(env.ContentRootPath, targetPath);

                Directory.CreateDirectory(folder);

                fileMover.MoveFile(fullPath, Path.Combine(env.ContentRootPath, targetPath), fi.Name);
                fileMover.CountFiles_Delete_LIMIT(folder, limitFileToSave);
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
