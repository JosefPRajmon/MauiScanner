using SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MauiScanner.Login
{
    internal class LoginClass
    {
        public UserClass _userClass;
        private const string DB_NAME = "user_db_test.DB3";
        private readonly SQLiteAsyncConnection _connection;

        public LoginClass()
        {
            _connection = new SQLiteAsyncConnection( Path.Combine( FileSystem.AppDataDirectory, DB_NAME ) );
            _connection.CreateTableAsync<UserClass>().Wait();
        }

        public async Task<List<string>> Login( string username, string password )
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if( accessType == NetworkAccess.Internet )
            {
                loginResponse responseO = JsonSerializer.Deserialize<loginResponse>(
                     await Task.Run( async () =>
                     {
                         HttpClient client = new HttpClient();
                         HttpResponseMessage response = await client.GetAsync( "https://www.as4u.cz/mobile/json.php?akce=login&name=" + username + "&pass=" + password );
                         string responseS = await response.Content.ReadAsStringAsync();
                         return responseS;
                     } )
                    );
                if( responseO.securid.ValueKind == JsonValueKind.String )
                {
                    return new List<string>() { responseO.securid.ToString(), "" };
                }
                else
                {
                    return new List<string>() { "0", responseO.reason };
                }
            }
            else
            {
                App.Current.Resources.TryGetValue( "InternetOut", out object Error );
                return new List<string>() { "0", (string)Error };
            }
        }

        public async Task<string> GetUserID( Boolean reload = false )
        {
            if( reload ) _userClass = await GetUser();
            try
            {
                return _userClass.XUser;
            }
            catch( Exception )
            {
                _userClass = await GetUser();
                return _userClass.XUser;
            }
        }

        public async Task<Boolean> IsLoggedIn()
        {
            _userClass = await GetUser();
            return _userClass != null;

        }
        public async void LoginSaver( string usernName, string password )
        {
            UserClass userClass = new UserClass();
            userClass.UserName = usernName;
            userClass.Password = password;
            Create( userClass );
        }

        public async Task<UserClass> GetUser()
        {
            return (UserClass)await _connection.Table<UserClass>().FirstOrDefaultAsync();
        }
        public async Task Create( UserClass customer )
        {
            await _connection.InsertAsync( customer );
        }
        public async Task Update( UserClass customer )
        {
            List<UserClass> user = await _connection.Table<UserClass>().ToListAsync();
            user[ 0 ] = customer;
            await _connection.UpdateAsync( customer );
        }

        public async Task<bool> ReLogin()
        {
            UserClass user = await GetUser();
            List<string> responseO = await Login( user.UserName, user.Password.ToLower() );
            user.XUser = responseO[ 0 ];
            await Update( user );



            return user.XUser == await GetUserID( true );

        }

        public string CreateMD5( string input )
        {
            // Vytvoření nové instance MD5CryptoServiceProvider
            using( MD5 md5 = MD5.Create() )
            {
                // Převod vstupního řetězce na pole bajtů a výpočet hashe
                byte[] inputBytes = Encoding.ASCII.GetBytes( input );
                byte[] hashBytes = md5.ComputeHash( inputBytes );

                // Převod pole bajtů na řetězec hexadecimálních čísel
                StringBuilder sb = new StringBuilder();
                for( int i = 0; i < hashBytes.Length; i++ )
                {
                    sb.Append( hashBytes[ i ].ToString( "X2" ) );
                }
                return sb.ToString();
            }
        }


    }
}
