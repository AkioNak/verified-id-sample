using System.Globalization;

namespace VerifiedIdApi.Options
{
    public class VerifiedIdOption
    {
        /// <summary>
        /// Endpoint
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// VCServiceScope
        /// </summary>
        public string VCServiceScope { get; set; } = string.Empty;

        /// <summary>
        /// Instance
        /// </summary>
        public string Instance { get; set; } = string.Empty;

        /// <summary>
        /// テナントID
        /// </summary>
        public string TenantId { get; set; } = string.Empty;

        /// <summary>
        /// ADアプリケーションID
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// ADアプリケーションシークレット
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// 分散化識別子
        /// </summary>
        public string IssuerAuthority { get; set; } = string.Empty;

        /// <summary>
        /// 分散化識別子
        /// </summary>
        public string VerifierAuthority { get; set; } = string.Empty;

        /// <summary>
        /// マニフェストURL
        /// </summary>
        public string CredentialManifest { get; set; } = string.Empty;

        /// <summary>
        /// 資格情報の種類
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// APIキー
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// アクセストークン取得用のAuthority
        /// </summary>
        public string Authority
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Instance, TenantId);
            }
        }

        /// <summary>
        /// API Endpoint
        /// </summary>
        public string ApiEndpoint
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Endpoint, TenantId);
            }
        }
    }
}
