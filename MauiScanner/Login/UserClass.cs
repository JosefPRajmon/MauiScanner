using SQLite;

namespace MauiScanner.Login
{
    [Table( "user" )]
    public class UserClass
    {
        [PrimaryKey]
        [Column( "userName" )]
        public string UserName { get; set; }
        [Column( "password" )]
        public string Password { get; set; }
        [Column( "id" )]
        public string Id { get; set; }
        [Column( "usePrice" )]
        public bool UsePrice { get; set; }
        public string XUser { get; set; }
        public UserClass()
        {
            UsePrice = true;
        }
    }
}
