using AutoMapper.Internal;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using WatcherService.RequestAPI_Http;
using WatcherService.ResponseAPI_Http;
using WatcherService.Services.IServices;

namespace WatcherService.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory httpClient;
        public BaseService(IHttpClientFactory _httpClient)
        {
            httpClient = _httpClient;
        }

        public async Task<APIResponse> SendAsync(RequestDetails apiRequest)
        {
            var client = httpClient.CreateClient();

            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");

            message.RequestUri = new Uri(apiRequest.Url);

            if (apiRequest.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
            }
            else
            {
                message.Content = new StringContent("", Encoding.UTF8, "application/json");
            }

            switch (apiRequest.Method)
            {
                case RequestDetails.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case RequestDetails.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case RequestDetails.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage apiResponse = null;

            if (!string.IsNullOrEmpty(apiRequest.token))
            {
                // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.token);
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.token);
            }

            apiResponse = await client.SendAsync(message);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.Unauthorized:           // 401
                    return new APIResponse
                    {
                        IsSuccess = false,
                        StatusCode = apiResponse.StatusCode,
                        ErrorMessages = "401 Unauthorized (invalid or missing token)"
                    };
                case HttpStatusCode.BadRequest:             // 400
                    {
                        return new APIResponse
                        {
                            IsSuccess = false,
                            StatusCode = apiResponse.StatusCode,
                            ErrorMessages = "400 Bad Request"
                        };
                    }
                case HttpStatusCode.InternalServerError:    // 500
                    {
                        return new APIResponse
                        {
                            IsSuccess = false,
                            StatusCode = apiResponse.StatusCode,
                            ErrorMessages = "500 Internal Server Error"
                        };
                    }
                case HttpStatusCode.OK:               // 404
                    {
                        return new APIResponse
                        {
                            IsSuccess = true,
                            StatusCode = apiResponse.StatusCode
                        };
                    }
                default:
                    {
                        return new APIResponse
                        {
                            IsSuccess = false,
                            StatusCode = apiResponse.StatusCode,
                            ErrorMessages = $"status code {apiResponse.StatusCode} Problen check"
                        };
                    }
            }

            //var apiContent = await apiResponse.Content.ReadAsStringAsync();
            //var apiResponseDTO = JsonConvert.DeserializeObject<APIResponse>(apiContent);

            //return new APIResponse
            //{
            //    IsSuccess = true,
            //    StatusCode = apiResponse.StatusCode,
            //    ErrorMessages = "everything is ok"
            //};
        }
    }
}


