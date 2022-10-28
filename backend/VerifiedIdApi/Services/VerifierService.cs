using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using VerifiedIdApi.Models;
using VerifiedIdApi.Options;
using VerifiedIdApi.Utils;
using VerifiedIdApi.ViewModels;

namespace VerifiedIdApi.Services
{
    public class VerifierService
    {
        private readonly VerifiedIdOption Option;

        /// <summary>
        /// HTTP Client
        /// </summary>
        private readonly HttpClient HttpClient;

        protected IMemoryCache Cache;

        private readonly ILogger<VerifierService> Logger;

        public VerifierService(
            IOptions<VerifiedIdOption> option,
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            ILogger<VerifierService> logger)
        {
            Option = option.Value;
            HttpClient = httpClientFactory.CreateClient("VerifiedId");
            Cache = memoryCache;
            Logger = logger;
        }

        /// <summary>
        /// 証明書の検証をリクエストする
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<PresentationRequestResultViewModel> PresentationRequest(HttpRequest request)
        {
            var presentationRequest = new PresentationRequest();

            string state = Guid.NewGuid().ToString();

            presentationRequest.Callback.State = state;
            presentationRequest.Authority = Option.VerifierAuthority;

            var requestCredential = new RequestCredential();
            requestCredential.Type = Option.Type;
            requestCredential.AcceptedIssuers.Add(Option.IssuerAuthority);
            presentationRequest.RequestedCredentials.Add(requestCredential);

            var host = VerifiedIdUtil.GetRequestHostName(request);
            if (host.Contains("//localhost"))
            {
                throw new Exception("localhost not supported");
            }
            presentationRequest.Callback.Url = string.Format("{0}/api/verifier/presentation-callback", host);
            presentationRequest.Callback.Headers.ApiKey = Option.ApiKey;

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var jsonString = JsonConvert.SerializeObject(presentationRequest, jsonSerializerSettings);

            try
            {
                var accessToken = await VerifiedIdUtil.GetAccessToken(Option);

                var defaultRequestHeaders = HttpClient.DefaultRequestHeaders;
                defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                Logger.LogInformation("PresentationRequest Start. Request Body = " + jsonString);

                // 証明書検証APIを呼び出す
                HttpResponseMessage res = await HttpClient.PostAsync(Option.ApiEndpoint + "verifiableCredentials/createPresentationRequest", new StringContent(jsonString, Encoding.UTF8, "application/json"));
                var statusCode = res.StatusCode;
                var responseJson = await res.Content.ReadAsStringAsync();

                Logger.LogInformation("PresentationRequest Sucess. Response =  " + responseJson);

                if (statusCode == HttpStatusCode.Created && !string.IsNullOrEmpty(responseJson))
                {
                    var result = JsonConvert.DeserializeObject<PresentationRequestResult>(responseJson);

                    if (result != null)
                    {
                        var cacheData = new CacheData
                        {
                            Status = "notscanned",
                            Message = "Request ready, please scan with Authenticator",
                            Expiry = result.Expiry.ToString()
                        };
                        Cache.Set(state, JsonConvert.SerializeObject(cacheData));

                        var obj = new PresentationRequestResultViewModel
                        {
                            Id = state,
                            Url = result.Url.ToString()
                        };

                        return obj;
                    }
                    else
                    {
                        throw new Exception("Something went wrong calling the API: " + responseJson);
                    }
                }
                else
                {
                    throw new Exception("Something went wrong calling the API: " + responseJson);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 証明書検証のコールバック時の挙動
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task PresentationCallback(HttpRequest request)
        {
            string content = await new StreamReader(request.Body).ReadToEndAsync();

            Logger.LogInformation("Presentation Callback. Content = " + content);

            request.Headers.TryGetValue("api-key", out var apiKey);
            if (Option.ApiKey != apiKey)
            {
                throw new Exception("api-key wrong or missing");
            }

            var presentationCallback = JsonConvert.DeserializeObject<PresentationCallback>(content);

            if (presentationCallback != null)
            {
                var state = presentationCallback.State;

                // ①QRコードが読み取られた時は「request_retrieved」が返される
                if (presentationCallback.RequestStatus == "request_retrieved")
                {
                    var cacheData = new CacheData
                    {
                        Status = "request_retrieved",
                        Message = "QR Code is scanned. Waiting for validation...",
                    };
                    Cache.Set(state, JsonConvert.SerializeObject(cacheData));
                }

                // ②証明書が承認された時は「presentation_verified」が返される
                if (presentationCallback.RequestStatus == "presentation_verified")
                {
                    var cacheData = new CacheData
                    {
                        Status = "presentation_verified",
                        Message = "Presentation verified",
                        Payload = JsonConvert.SerializeObject(presentationCallback.VerifiedCredentialsData),
                        Subject = presentationCallback.Subject,
                        FirstName = presentationCallback.VerifiedCredentialsData!.First().Claims.FirstName,
                        LastName = presentationCallback.VerifiedCredentialsData!.First().Claims.LastName,
                        PresentationResponse = JsonConvert.SerializeObject(presentationCallback)
                    };
                    Cache.Set(state, JsonConvert.SerializeObject(cacheData));
                }
            }
            else
            {
                throw new Exception("Callback data is incorrect" + content);
            }
        }

        /// <summary>
        /// 証明書検証のステータスを取得する
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PresentationResponseViewModel? PresentationResponse(HttpRequest request)
        {
            try
            {
                string state = request.Query["id"];
                if (string.IsNullOrEmpty(state))
                {
                    throw new Exception("Missing argument 'id'");
                }
                if (Cache.TryGetValue(state, out string buf))
                {
                    var cacheData = JsonConvert.DeserializeObject<CacheData>(buf);

                    if (cacheData != null)
                    {
                        var result = new PresentationResponseViewModel
                        {
                            Status = cacheData.Status,
                            Message = cacheData.Message
                        };

                        if (!string.IsNullOrEmpty(cacheData.Expiry))
                        {
                            result.Expiry = cacheData.Expiry;
                        }

                        if (cacheData.Status == "presentation_verified")
                        {
                            result.Payload = cacheData.Payload;
                            result.Subject = cacheData.Subject;
                            result.LastName = cacheData.LastName;
                            result.FirstName = cacheData.FirstName;
                        }

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}
