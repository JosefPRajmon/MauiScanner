using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiScanner.Login
{
    public class loginResponse
    {
        public JsonElement securid { get; set; }
        public string? reason { get; set; }
        public string? name_surname { get; set; }
        public string? email { get; set; }
        public string? telefon { get; set; }
    }
}
