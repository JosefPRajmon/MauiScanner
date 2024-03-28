using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiScanner.Login
{
    internal class LoginClass
    {
        private UserClass _userClass;
        private const string DB_NAME = "user_db.DB3";
        private readonly SQLiteAsyncConnection _connection;

        public LoginClass()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<UserClass>().Wait();
        }
        
        public async Task<loginResponse> Login(string username, string password)
        {
           
            return  JsonSerializer.Deserialize<loginResponse>(
                 await Task.Run(async () =>
                 {
                     HttpClient client = new HttpClient();
                     HttpResponseMessage response = await client.GetAsync("https://www.as4u.cz/mobile/json.php?akce=login&name=" + username + "&pass=" + password);
                     string responseS = await response.Content.ReadAsStringAsync();
                     return responseS;
                 })
                );
        }

        public string GetUserID()
        {
            return _userClass.id;
        }

        public async Task<Boolean> IsLoggedIn()
        {
            _userClass = await GetUser();
            return _userClass != null;

        }
        public async void LoginSaver(string usernName, string password)
        {
            UserClass userClass = new UserClass();
            userClass.UserName = usernName;
            userClass.Password = password;
            Create(userClass);

        }

        public async Task<UserClass> GetUser()
        {
            return (UserClass)await _connection.Table<UserClass>().FirstOrDefaultAsync();
        }
        public async Task Create(UserClass customer)
        {
            await _connection.InsertAsync(customer);
        }
        public string CreateMD5(string input)
        {
            // Vytvoření nové instance MD5CryptoServiceProvider
            using (MD5 md5 = MD5.Create())
            {
                // Převod vstupního řetězce na pole bajtů a výpočet hashe
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Převod pole bajtů na řetězec hexadecimálních čísel
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }


    }
}
