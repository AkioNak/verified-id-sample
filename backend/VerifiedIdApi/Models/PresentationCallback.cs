using Newtonsoft.Json;

namespace VerifiedIdApi.Models
{
    public class PresentationCallback
    {
        /// <summary>
        /// リクエストID
        /// </summary>
        [JsonProperty("requestId")]
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        /// リクエストステータス
        /// ・request_retrieved：QRスキャン
        /// ・presentation_verified：検証成功
        /// </summary>
        [JsonProperty("requestStatus")]
        public string RequestStatus { get; set; } = string.Empty;

        /// <summary>
        /// 状態の値
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// 検証可能な資格情報ユーザー DID
        /// </summary>
        [JsonProperty("subject")]
        public string? Subject { get; set; }

        [JsonProperty("verifiedCredentialsData")]
        public List<VerifiedCredential>? VerifiedCredentialsData { get; set; }

        /// <summary>
        /// ウォレットから検証可能な資格情報サービスに送信された元のペイロード
        /// </summary>
        [JsonProperty("receipt")]
        public Receipt? Receipt { get; set; }
    }

    public class VerifiedCredential
    {
        /// <summary>
        /// 発行者の DID
        /// </summary>
        [JsonProperty("issuer")]
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// 検証可能な資格情報の種類
        /// </summary>
        [JsonProperty("type")]
        public List<string> Type { get; set; } = new List<string>();

        /// <summary>
        /// 取得された要求
        /// </summary>
        [JsonProperty("claims")]
        public CallbackClaims Claims { get; set; } = new CallbackClaims();

        /// <summary>
        /// 検証可能な資格情報の発行者のドメイン検証状態
        /// </summary>
        [JsonProperty("credentialState")]
        public CredentialState CredentialState { get; set; } = new CredentialState();

        /// <summary>
        /// 検証可能な資格情報の発行者のドメイン
        /// </summary>
        [JsonProperty("domainValidation")]
        public DomainValidation DomainValidation { get; set; } = new DomainValidation();
    }

    public class CallbackClaims
    {
        /// <summary>
        /// 姓
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// 名
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; } = string.Empty;
    }

    public class CredentialState
    {
        [JsonProperty("revocationStatus")]
        public string RevocationStatus { get; set; } = string.Empty;
    }

    public class DomainValidation
    {
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
    }

    public class Receipt
    {
        [JsonProperty("id_token")]
        public string IdToken { get; set; } = string.Empty;

        [JsonProperty("vp_token")]
        public string VpToken { get; set; } = string.Empty;

        [JsonProperty("state")]
        public string State { get; set; } = string.Empty;
    }
}
