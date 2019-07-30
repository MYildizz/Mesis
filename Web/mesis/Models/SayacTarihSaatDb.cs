using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mesis.Models
{
    public class SayacTarihSaatDb : DbContext
    {
        public SayacTarihSaatDb() : base("verilerDb")
        {

        }

        public DbSet<SayacTarihSaat> time { get; set; }


    }
}