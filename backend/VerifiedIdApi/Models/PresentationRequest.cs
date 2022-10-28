using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace VerifiedIdApi.Models
{
    public class PresentationRequest
    {
        /// <summary>
        /// QRコードを含めるか
        /// </summary>
        [JsonProperty("includeQRCode")]
        public bool IncludeQRCode { get; set; } = true;

        [JsonProperty("includeReceipt")]
        public bool IncludeReceipt { get; set; } = true;

        /// <summary>
        /// 分散化識別子
        /// </summary>
        [JsonProperty("authority")]
        public string Authority { get; set; } = string.Empty;

        [JsonProperty("registration")]
        public Registration Registration { get; set; } = new Registration();

        /// <summary>
        /// コールバックの情報
        /// </summary>
        [JsonProperty("callback")]
        public Callback Callback { get; set; } = new Callback();

        [JsonProperty("requestedCredentials")]
        public List<RequestCredential> RequestedCredentials { get; set; } = new List<RequestCredential>();
    }

    public class RequestCredential
    {
        /// <summary>
        /// 資格情報の種類
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 資格情報の利用目的
        /// </summary>
        [JsonProperty("purpose")]
        public string Purpose { get; set; } = "So we can see that you a veriable credentials expert";

        [JsonProperty("acceptedIssuers")]
        public List<string> AcceptedIssuers { get; set; } = new List<string>();
    }
}
