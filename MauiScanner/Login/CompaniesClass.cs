using System.Text.Json.Serialization;

namespace MauiScanner.Login
{
    public class CompaniesClass
    {
        [JsonPropertyName( "name" )]
        public string Name { get; set; }
        [JsonPropertyName( "provozovny" )]
        public Dictionary<string, string> Worksshop { get; set; }
    }
}
