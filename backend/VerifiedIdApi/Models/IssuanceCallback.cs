using Newtonsoft.Json;

namespace VerifiedIdApi.Models
{
    public class IssuanceCallback
    {
        /// <summary>
        /// リクエストID
        /// </summary>
        [JsonProperty("requestId")]
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        /// リクエストステータス
        /// ・request_retrieved：QRスキャン
        /// ・issuance_successful：発行成功
        /// ・issuance_error：エラー発生
        /// </summary>
        [JsonProperty("requestStatus")]
        public string RequestStatus { get; set; } = string.Empty;

        /// <summary>
        /// 状態の値
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// エラー
        /// </summary>
        [JsonProperty("error")]
        public Error? Error { get; set; }
    }
    public class Error
    {
        /// <summary>
        /// エラーコード
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
