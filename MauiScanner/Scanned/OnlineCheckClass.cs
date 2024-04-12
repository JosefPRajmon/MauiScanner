using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiScanner.Scanned
{
    public class OnlineCheckClass
    {

        public OnlineCheckClass() { }

        private async Task<ScannedResponseClass> Caller(string subakce, string userID,string cardNumber,string price = "0")
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType == NetworkAccess.Internet)
                {
                try
                {
                    string url = $"http://test.as4u.local/redakce/json.php?akce=sale&subakce={subakce}&xuser={userID}&card_numberFind={cardNumber}";
                    if (price != "0")
                    {
                        url += $"&price={price}";
                    }
                    string json = await Task.Run(async () =>
                    {
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        HttpClient client = new HttpClient(handler);
                        HttpResponseMessage response = await client.GetAsync(url);
                        string responseS = await response.Content.ReadAsStringAsync();
                        return responseS;
                    });
                    ScannedResponseClass responseO = new ScannedResponseClass();
                    if (json.Contains("status"))
                    {
                        try
                        {
                            responseO.status = System.Text.RegularExpressions.Regex.Unescape(GetStringFromURL(json, "\"status\":\"", out int endindex, "\",\""));
                        }
                        catch (Exception)
                        {

                        }
                        try
                        {
                            string stringg = System.Text.RegularExpressions.Regex.Unescape(GetStringFromURL(json, "\"infotext\":\"", out int endindex, "\",\""));
                            responseO.infotext = stringg;
                        }
                        catch (Exception)
                        {

                        }
                        try
                        {
                            responseO.drzitel = System.Text.RegularExpressions.Regex.Unescape(GetStringFromURL(json, "\"drzitel\":\"", out int endindex, "\",\""));
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            responseO.sale = GetStringFromURL(json, "\"sale\":\"", out int endindex, "\",\"");
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            responseO.priceAfterSale = $"{GetStringFromURL(json, "\"priceAfterSale\":", out int endindex, "}")}";
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {

                        App.Current.Resources.TryGetValue("ServerDisconect", out object Error);
                        responseO.status = "error";
                        responseO.infotext = (string)Error;
                    }
                    return responseO;
                }
                catch (Exception e)
                {

                    App.Current.Resources.TryGetValue("ServerOut", out object Error);
                    string retuntText = (string)Error;
                    return new ScannedResponseClass() { status="error",infotext = retuntText };
                }
            }
            else
            {
                App.Current.Resources.TryGetValue("InternetOut", out object Error);
                return new ScannedResponseClass() { status = "error", infotext = (string) Error };
            }
        }
        private string GetStringFromURL(string json, string key, out int endIndex, string Lastkey = ",")
        {
            try
            {
                int startIndex;


                startIndex = json.IndexOf(key) + key.Length;
                if (Lastkey == "}" && !json.Contains("}"))
                {
                    json = json + "}";
                }

                endIndex = json.IndexOf(Lastkey, startIndex);
                //peaplePlacesModel.length_priority = int.Parse(json.Substring(startIndex, endIndex - startIndex));
                var i = json.Substring(startIndex, endIndex - startIndex);
                if (Lastkey == "," && i.Contains(":"))
                {
                    i = i.Split(":"[0])[1];
                }

                return i;
            }
            catch (Exception e)
            {
                endIndex = 0;
                return "";
            }
        }
        public async Task<ScannedResponseClass> CheckSale(string userID, string cardNumber, string price = "0")
        {
            ScannedResponseClass scannedResponseClass = await Caller("sale_basic_test", userID, cardNumber,price);
            return scannedResponseClass;
        }
        public async Task<ScannedResponseClass> UseSale(string userID, string cardNumber)
        {
            ScannedResponseClass scannedResponseClass = await Caller("sale_basic_write", userID, cardNumber);
            return scannedResponseClass;
        }
    }
}