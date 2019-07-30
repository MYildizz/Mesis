using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mesis.Models
{
    public class CihazDurumu
    {

        public int cihaz { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public int seconds { get; set; }

    }
}