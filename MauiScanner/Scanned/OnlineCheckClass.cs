using MauiScanner.Login;
using System.Text;

namespace MauiScanner.Scanned
{
    public class OnlineCheckClass
    {

        public OnlineCheckClass() { }

        /// <summary>
        /// function to call server and check or use card sales
        /// </summary>
        /// <param name="subakce">string action </param>
        /// <param name="userID"></param>
        /// <param name="cardNumber"></param>
        /// <param name="usedSale"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> Caller( string subakce, string userID, string cardNumber, string usedSale = "-1" )
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if( accessType == NetworkAccess.Internet )
            {
                try
                {
                    LoginClass loginClass = new LoginClass();
                    UserClass user = await loginClass.GetUser();
                    string url = string.Format("https://karta.bilina.cz/redakce/json.php?akce=sale&subakce={0}&xuser={1}&card_numberFind={2}&companiesID={3}&establishmentsID={4}", subakce, userID, cardNumber, user.Companie, user.Workshop );
                    if( usedSale != "-1" )
                    {
                        url += $"&salesUsed={usedSale}";
                    }
                    return await Task.Run( async () =>
                    {
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        HttpClient client = new HttpClient( handler );
                        HttpResponseMessage response = await client.GetAsync( url );
                        //string responseS = await response.Content.ReadAsStringAsync();
                        return response;
                    } );
                    //ScannedResponseClass responseO = new ScannedResponseClass();
                    /*if (json.Contains("status"))
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
                    return responseO;*/
                }
                catch( Exception )
                {

                    App.Current.Resources.TryGetValue( "ServerOut", out object Error );
                    string retuntText = (string)Error;
                    return new HttpResponseMessage() { Content = new StringContent( $"{{\"status\":\"error\",\"infotext\":\"{retuntText}\",\"errorNo\":1}}", Encoding.UTF8, "application/json" ) };//new ScannedResponseClass() { status="error",infotext = retuntText };
                }
            }
            else
            {
                App.Current.Resources.TryGetValue( "InternetOut", out object Error );
                return new HttpResponseMessage() { Content = new StringContent( $"{{\"status\":\"error\",\"infotext\":\"{Error}\",\"errorNo\":1}}", Encoding.UTF8, "application/json" ) };
            }
        }
        private string GetStringFromURL( string json, string key, out int endIndex, string Lastkey = "," )
        {
            try
            {
                int startIndex;


                startIndex = json.IndexOf( key ) + key.Length;
                if( Lastkey == "}" && !json.Contains( "}" ) )
                {
                    json = json + "}";
                }

                endIndex = json.IndexOf( Lastkey, startIndex );
                //peaplePlacesModel.length_priority = int.Parse(json.Substring(startIndex, endIndex - startIndex));
                var i = json.Substring( startIndex, endIndex - startIndex );
                if( Lastkey == "," && i.Contains( ":" ) )
                {
                    i = i.Split( ":"[ 0 ] )[ 1 ];
                }

                return i;
            }
            catch( Exception e )
            {
                endIndex = 0;
                return "";
            }
        }
        public async Task<HttpResponseMessage> CheckSale( string userID, string cardNumber )
        {
            return await Caller( "sale_basic_test", userID, cardNumber );
        }
        public async Task<HttpResponseMessage> UseSale( string userID, string cardNumber, string usedSales )
        {
            return await Caller( "sale_basic_write", userID, cardNumber, usedSales );
        }
    }
}