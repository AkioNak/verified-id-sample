using Newtonsoft.Json;

namespace VerifiedIdApi.Models
{
    public class IssuanceRequest
    {
        /// <summary>
        /// QRコードを含めるか
        /// </summary>
        [JsonProperty("includeQRCode")]
        public bool IncludeQRCode { get; set; } = true;

        /// <summary>
        /// コールバックの情報
        /// </summary>
        [JsonProperty("callback")]
        public Callback Callback { get; set; } = new Callback();

        /// <summary>
        /// 分散化識別子
        /// </summary>
        [JsonProperty("authority")]
        public string Authority { get; set; } = string.Empty;

        [JsonProperty("registration")]
        public Registration Registration { get; set; } = new Registration();

        /// <summary>
        /// 資格情報の種類
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 資格情報のマニフェストURL
        /// </summary>
        [JsonProperty("manifest")]
        public string Manifest { get; set; } = string.Empty;

        /// <summary>
        /// 資格情報に含める属性情報
        /// </summary>
        [JsonProperty("claims")]
        public Claims Claims { get; set; } = new Claims();

        /// <summary>
        /// PIN
        /// </summary>
        [JsonProperty("pin")]
        public Pin Pin { get; set; } = new Pin();
    }

    public class Callback
    {
        /// <summary>
        /// コールバックURL
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 状態ID
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// コールバックURL呼び出し時のヘッダー
        /// </summary>
        [JsonProperty("headers")]
        public Headers Headers { get; set; } = new Headers();
    }

    public class Headers
    {
        /// <summary>
        /// APIキー
        /// </summary>
        [JsonProperty("api-key")]
        public string ApiKey { get; set; } = string.Empty;
    }

    public class Registration
    {
        /// <summary>
        /// 資格情報の発行者の表示名
        /// </summary>
        [JsonProperty("clientName")]
        public string ClientName { get; set; } = "Veriable Credential Expert Verifier";
    }

    /// <summary>
    /// PINの情報
    /// </summary>
    public class Pin
    {
        /// <summary>
        /// PINの値
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// PINの長さ
        /// </summary>
        [JsonProperty("length")]
        public int Length { get; set; } = 4;
    }

    /// <summary>
    /// 証明書に含める属性情報
    /// </summary>
    public class Claims
    {
        /// <summary>
        /// ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 姓
        /// </summary>
        [JsonProperty("family_name")]
        public string FamilyName { get; set; } = string.Empty;

        /// <summary>
        /// 名
        /// </summary>
        [JsonProperty("given_name")]
        public string GivenName { get; set; } = string.Empty;
    }
}
