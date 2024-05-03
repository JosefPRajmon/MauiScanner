using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiScanner.Scanned
{
    public class ResponseCardClass
    {
        [JsonPropertyName("holderID")]
        public string? HolderID {  get; set; }
        [JsonPropertyName("holder_name")]
        public string? HolderName { get; set; }
        [JsonPropertyName("holder_year_birth")]
        public string? HolderYearBirth { get; set;}
    }
}
