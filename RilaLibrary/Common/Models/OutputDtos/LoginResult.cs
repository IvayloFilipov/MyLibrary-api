using System.Text.Json.Serialization;

namespace Common.Models.OutputDtos
{
    public class LoginResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("roles")]
        public ICollection<string> Roles { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        //[JsonPropertyName("refreshToken")]
        //public string RefreshToken { get; set; }
    }
}
