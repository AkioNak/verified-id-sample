using Newtonsoft.Json;

namespace VerifiedIdApi.Models
{
    public class PresentationRequestResult
    {
        /// <summary>
        /// リクエストID
        /// </summary>
        [JsonProperty("requestId")]
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        /// 証明書検証のURL
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 有効期限
        /// </summary>
        [JsonProperty("expiry")]
        public int Expiry { get; set; }

        /// <summary>
        /// 証明書検証のQRコード
        /// </summary>
        [JsonProperty("qrCode")]
        public string QrCode { get; set; } = string.Empty;
    }
}
