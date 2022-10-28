using Newtonsoft.Json;

namespace VerifiedIdApi.Models
{
    public class CacheData
    {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        [JsonProperty("expiry")]
        public string? Expiry { get; set; }

        [JsonProperty("payload")]
        public string? Payload { get; set; }

        [JsonProperty("subject")]
        public string? Subject { get; set; }

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("presentationResponse")]
        public string? PresentationResponse { get; set; }
    }
}
