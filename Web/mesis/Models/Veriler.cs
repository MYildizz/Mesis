using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mesis.Models
{
    public class Veriler
    {
        public  CihazDurumu status { get; set; }
        public Stack<tablo> gunluk { get; set; }
        public Stack<tablo> haftalik { get; set; }
        public Stack<tablo> aylik { get; set; }
        public Stack<tablo> yillik { get; set; }
    }
}