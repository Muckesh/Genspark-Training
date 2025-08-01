
using Newtonsoft.Json;

namespace Ecommerce.Models.DTOs
{
    public class CaptchaResponseDto
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}