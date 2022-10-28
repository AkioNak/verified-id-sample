using Microsoft.Identity.Client;
using VerifiedIdApi.Options;

namespace VerifiedIdApi.Utils
{
    public class VerifiedIdUtil
    {
        /// <summary>
        /// ホスト名を取得する
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns></returns>
        public static string GetRequestHostName(HttpRequest request)
        {
            var scheme = request.Scheme;
            var originalHost = request.Headers["x-original-host"];
            if (!string.IsNullOrEmpty(originalHost))
            {
                return string.Format("{0}://{1}", scheme, originalHost);
            }
            else
            {
                return string.Format("{0}://{1}", scheme, request.Host);
            }
        }

        /// <summary>
        /// アクセストークンを取得する
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<string> GetAccessToken(VerifiedIdOption option)
        {
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(option.ClientId)
                .WithClientSecret(option.ClientSecret)
                .WithAuthority(new Uri(option.Authority))
                .Build();

            string[] scopes = new string[] { option.VCServiceScope };

            AuthenticationResult? result;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync();
            }
            catch (MsalServiceException e) when (e.Message.Contains("AADSTS70011"))
            {
                throw new Exception("Scope provided is not supported");
            }
            catch (MsalServiceException e)
            {
                throw new Exception("Something went wrong getting an access token for the client API:" + e.Message);
            }

            return result.AccessToken;
        }
    }
}
