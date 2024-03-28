using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScanner.Login
{
    [Table("user")]
    public class UserClass
    {

        [Column("userName")]
        public string UserName { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("id")]
        public string id { get; set; }
    }
}
