using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiScanner.Scanned
{
    public class ScannedResponseClass
    {
        [JsonPropertyName("status")]
        public string? Status {  get; set; }

        [JsonPropertyName("card")]
        public ResponseCardClass? Card {  get; set; }
        [JsonPropertyName("sales")]
        public Dictionary<string, ResponseSaleClass>? Sales { get; set; }
        [JsonPropertyName("infotext")]
        public string? Infotext { get; set; }

        [JsonPropertyName("errorNo")]
        public int ErrorNo { get; set; }
    }
}
