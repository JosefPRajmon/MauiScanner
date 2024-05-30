using System.Text.Json;
using System.Text.Json.Serialization;

namespace MauiScanner.Login
{
    public class loginResponse
    {
        public JsonElement securid { get; set; }
        public string? reason { get; set; }
        public string? name_surname { get; set; }
        public string? email { get; set; }
        public string? telefon { get; set; }
        [JsonPropertyName( "companies" )]
        public Dictionary<string, CompaniesClass> Companies { get; set; }
    }
}
