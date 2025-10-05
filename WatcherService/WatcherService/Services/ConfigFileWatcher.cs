//using Newtonsoft.Json;
//using WatcherService.Models;

//namespace WatcherService.Services
//{
//    public class ConfigFileWatcher
//    {
//        private readonly IWebHostEnvironment _env;
//        private FileSystemWatcher? _fsw;
//        string folder = string.Empty;

//        public ConfigFileWatcher(IWebHostEnvironment env)
//        {
//            _env = env;
             
//        }

//        public Task StartAsync(CancellationToken ct)
//        {
//            // נבנה את הנתיב: <ContentRoot>/configFile
//            folder = Path.Combine(_env.ContentRootPath, "configFile");
//           // Directory.CreateDirectory(folder);

//            _fsw = new FileSystemWatcher(folder)
//            {
//                NotifyFilter = NotifyFilters.LastWrite,     // נעקוב רק אחרי שינוי בתוכן
//                Filter = "WatcherSettings.json",            // רק הקובץ הזה
//                EnableRaisingEvents = true
//            };

//            _fsw.Changed += OnChanged;

//            return Task.CompletedTask;
//        }

//        public Task StopAsync(CancellationToken ct)
//        {
//            if (_fsw is not null)
//            {
//                _fsw.Changed -= OnChanged;
//                _fsw.Dispose();
//                _fsw = null;
//            }
//            return Task.CompletedTask;
//        }

//        private void OnChanged(object? sender, FileSystemEventArgs e)
//        {

//            string json = File.ReadAllText(folder);
//            var settings = JsonConvert.DeserializeObject<UpdateWatcherSettingsRequest>(json);

//            var v = new UpdateWatcherSettingsRequest
//            {
//                Issuer = settings.Issuer,
//                LoggerUrl = settings.LoggerUrl,
//                ProcessedPath = settings.ProcessedPath,
//                TokenTTLMinutes = settings.TokenTTLMinutes,
//                WatchedPath = settings.WatchedPath
//            };
//        }
//    }
//}
