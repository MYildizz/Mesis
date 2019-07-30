using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mesis.Models
{
    public class Veritabani : DbContext
    {
        public Veritabani():base("verilerDb")
        {

        }

        public DbSet<tablo> verilerDb { get; set; }
       // public DbSet<sayac> count { get; set; }


    }
}