namespace VerifiedIdApi.ViewModels
{
    /// <summary>
    /// 証明書検証のリクエスト結果
    /// </summary>
    public class PresentationRequestResultViewModel
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
    }
}
