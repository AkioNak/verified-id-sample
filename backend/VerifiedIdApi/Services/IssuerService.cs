using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using VerifiedIdApi.Models;
using VerifiedIdApi.Options;
using VerifiedIdApi.Utils;
using VerifiedIdApi.ViewModels;

namespace VerifiedIdApi.Services
{
    public class IssuerService
    {
        private readonly VerifiedIdOption Option;

        /// <summary>
        /// HTTP Client
        /// </summary>
        private readonly HttpClient HttpClient;

        protected IMemoryCache Cache;

        private readonly ILogger<IssuerService> Logger;

        public IssuerService(
            IOptions<VerifiedIdOption> option,
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            ILogger<IssuerService> logger)
        {
            Option = option.Value;
            HttpClient = httpClientFactory.CreateClient("VerifiedId");
            Cache = memoryCache;
            Logger = logger;
        }

        /// <summary>
        /// 証明書の発行をリクエストする
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <param name="vm">入力したユーザー情報</param>
        /// <returns></returns>
        public async Task<IssuanceRequestResultViewModel> IssuanceRequest(HttpRequest request, 
            ClaimViewModel vm)
        {
            var issuanceRequest = new IssuanceRequest();

            string state = Guid.NewGuid().ToString();

            var length = issuanceRequest.Pin.Length;
            var pinMaxValue = (int)Math.Pow(10, length) - 1;
            var randomNumber = RandomNumberGenerator.GetInt32(1, pinMaxValue);
            var newPin = string.Format("{0:D" + length.ToString() + "}", randomNumber);
            issuanceRequest.Pin.Value = newPin;

            issuanceRequest.Callback.State = state;

            issuanceRequest.Authority = Option.IssuerAuthority;

            string host = VerifiedIdUtil.GetRequestHostName(request);
            if (host.Contains("//localhost"))
            {
                throw new Exception("localhost not supported");
            }
            issuanceRequest.Callback.Url = string.Format("{0}/api/issuer/issuance-callback", host);
            issuanceRequest.Callback.Headers.ApiKey = Option.ApiKey;

            issuanceRequest.Manifest = Option.CredentialManifest;
            issuanceRequest.Type = Option.Type;

            // Claimsの登録
            issuanceRequest.Claims.Id = Guid.NewGuid().ToString();
            issuanceRequest.Claims.FamilyName = vm.LastName;
            issuanceRequest.Claims.GivenName = vm.FirstName;

            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var jsonString = JsonConvert.SerializeObject(issuanceRequest, jsonSerializerSettings);

            try
            {
                var accessToken = await VerifiedIdUtil.GetAccessToken(Option);

                var defaultRequestHeaders = HttpClient.DefaultRequestHeaders;
                defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                Logger.LogInformation("IssuanceRequest Start. Request Body = " + jsonString);

                // 証明書発行APIを呼び出す
                HttpResponseMessage res = await HttpClient.PostAsync(Option.ApiEndpoint + "verifiableCredentials/createIssuanceRequest", new StringContent(jsonString, Encoding.UTF8, "application/json"));
                var statusCode = res.StatusCode;
                var responseJson = await res.Content.ReadAsStringAsync();

                Logger.LogInformation("IssuanceRequest Sucess. Response =  " + responseJson);

                if (statusCode == HttpStatusCode.Created && !string.IsNullOrEmpty(responseJson))
                {
                    var result = JsonConvert.DeserializeObject<IssuanceRequestResult>(responseJson);

                    if (result != null)
                    {
                        var cacheData = new CacheData
                        {
                            Status = "notscanned",
                            Message = "Request ready, please scan with Authenticator",
                            Expiry = result.Expiry.ToString()
                        };

                        Cache.Set(state, JsonConvert.SerializeObject(cacheData));

                        var obj = new IssuanceRequestResultViewModel
                        {
                            Id = state,
                            Url = result.Url,
                            Pin = issuanceRequest.Pin.Value.ToString()
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
        /// 証明書発行のコールバック時の挙動
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task IssuanceCallback(HttpRequest request)
        {
            string content = await new StreamReader(request.Body).ReadToEndAsync();

            Logger.LogInformation("Issuance Callback. Content = " + content);

            request.Headers.TryGetValue("api-key", out var apiKey);
            if (Option.ApiKey != apiKey)
            {
                throw new Exception("api-key wrong or missing");
            }

            var issuanceCallback = JsonConvert.DeserializeObject<IssuanceCallback>(content);
            if (issuanceCallback != null)
            {
                var state = issuanceCallback.State;

                // ①QRコードが読み取られた時は「request_retrieved」が返される
                if (issuanceCallback.RequestStatus == "request_retrieved")
                {
                    var cacheData = new CacheData
                    {
                        Status = "request_retrieved",
                        Message = "QR Code is scanned. Waiting for issuance...",
                    };
                    Cache.Set(state, JsonConvert.SerializeObject(cacheData));
                }

                // ②証明書が承認された時は「issuance_successful」が返される
                if (issuanceCallback.RequestStatus == "issuance_successful")
                {
                    var cacheData = new CacheData
                    {
                        Status = "issuance_successful",
                        Message = "Credential successfully issued",
                    };
                    Cache.Set(state, JsonConvert.SerializeObject(cacheData));
                }

                if (issuanceCallback.RequestStatus == "issuance_error" && issuanceCallback.Error != null)
                {
                    var cacheData = new CacheData
                    {
                        Status = "issuance_error",
                        Payload = issuanceCallback.Error.Code,
                        Message = issuanceCallback.Error.Message
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
        /// 証明書発行のステータスを取得する
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IssuanceResponseViewModel? IssuanceResponse(HttpRequest request)
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
                        var result = new IssuanceResponseViewModel
                        {
                            Status = cacheData.Status,
                            Message = cacheData.Message
                        };

                        if (!string.IsNullOrEmpty(cacheData.Expiry))
                        {
                            result.Expiry = cacheData.Expiry;
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
