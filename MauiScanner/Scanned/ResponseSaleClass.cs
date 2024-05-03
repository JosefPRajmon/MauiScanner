using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiScanner.Scanned
{
    public class ResponseSaleClass
    {
        [JsonPropertyName("status")]
        public string Status {  get; set; }
        public bool BoolStatus {  get; set; }
        [JsonPropertyName("saleName")]
        public string SaleName { get; set; }
        [JsonPropertyName("infotext")]
        public string InfoText { get; set; }
        [JsonPropertyName("sale")]
        public string? Sale {  get; set; }
        [JsonPropertyName("errorNo")]
        public int ErrorNo { get; set; }
        public string? Key {  get; set; }
    }
}
