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
        [Column( "compani" )]
        public string Companie { get; set; } = string.Empty;
        [Column( "companiName" )]
        public string CompanieName { get; set; } = string.Empty;
        [Column( "workshop" )]
        public string Workshop { get; set; } = string.Empty;
        [Column( "workshopName" )]
        public string WorkshopName { get; set; } = string.Empty;

        public UserClass()
        {
            UsePrice = true;
        }


    }
}
