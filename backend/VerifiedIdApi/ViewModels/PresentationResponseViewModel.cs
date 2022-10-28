namespace VerifiedIdApi.ViewModels
{
    public class PresentationResponseViewModel
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

        public string? Payload { get; set; }

        public string? Subject { get; set; }

        /// <summary>
        /// 証明書の姓
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// 証明書の名
        /// </summary>
        public string? LastName { get; set; }
    }
}
