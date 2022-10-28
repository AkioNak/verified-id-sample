namespace VerifiedIdApi.ViewModels
{
    /// <summary>
    /// 証明書発行のステータス
    /// </summary>
    public class IssuanceResponseViewModel
    {
        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 有効期限
        /// </summary>
        public string? Expiry { get; set; }
    }
}
