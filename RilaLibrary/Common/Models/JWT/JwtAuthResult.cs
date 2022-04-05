using System.Text.Json.Serialization;

namespace Common.Models.JWT
{
    public class JwtAuthResult
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }
}
