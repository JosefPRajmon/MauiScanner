using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScanner
{
    [Table("customer")]
    public class Customer
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }
        [Column("name")]
#pragma warning disable CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null. Zvažte možnost deklarovat ho jako pole s možnou hodnotou null.
        public string Name { get; set; }
#pragma warning restore CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null. Zvažte možnost deklarovat ho jako pole s možnou hodnotou null.
    }
}
