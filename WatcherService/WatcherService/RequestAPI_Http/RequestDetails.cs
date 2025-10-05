

namespace WatcherService.RequestAPI_Http
{
    public class RequestDetails
    {
        public string Url { get; set; }
        public object Data { get; set; }
        public string token { get; set; }

        public ApiType Method { get; set; } = ApiType.POST;

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
