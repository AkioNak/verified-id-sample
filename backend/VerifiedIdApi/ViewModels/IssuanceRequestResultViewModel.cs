namespace VerifiedIdApi.ViewModels
{
    /// <summary>
    /// 証明書発行のリクエスト結果
    /// </summary>
    public class IssuanceRequestResultViewModel
    {
        /// <summary>
        /// ID（証明書発行・検証の状態取得用）
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 証明書発行・検証URL
        /// このURLを元にQRコードを生成する
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// PINコード（証明書発行用）
        /// </summary>
        public string Pin { get; set; } = string.Empty;
    }
}
