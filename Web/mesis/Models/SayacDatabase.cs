using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mesis.Models
{
    public class SayacDatabase : DbContext
    {

        public SayacDatabase() : base("verilerDb")
        {

        }

        public DbSet<sayac> Count { get; set; }
    }
}