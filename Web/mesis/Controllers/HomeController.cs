using mesis.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mesis.Controllers
{




    public class HomeController : Controller 
    {
        // GET: Home
        public ActionResult Index()
        {
            //tablo bilgileri 
            Stack<tablo> gunluk = new Stack<tablo>();
            Stack<tablo> haftalik = new Stack<tablo>();
            Stack<tablo> aylik = new Stack<tablo>();
            Stack<tablo> yillik = new Stack<tablo>();

            //Zaman ayarları

            DateTime dt = DateTime.Today;
            string yil =""+ dt.Year;
            string ay;
            string gun;

            int week = dt.Day;
            int weekUst;
            int weekAlt;
            if(week<8)
            {
                weekUst = week;
                weekAlt = 31 - (7 - week);
            }
            else
            {
                weekUst = week;
                weekAlt = week - 7;
            }

            if(dt.Month<10)
            {
                 ay = "0" + dt.Month;
            }
            else
            {
                ay = "" + dt.Month;
            }

            if(dt.Day<10)
            {
                 gun = "0" + dt.Day;
            }
            else
            {
                 gun = "" + dt.Day;
            }
            
            //veritabanından tablo bilgilerini al
            Veritabani bilgiler = new Veritabani();

            List<tablo> bilgilerDb = bilgiler.verilerDb.ToList();


            //bilgileri tabloya işle
            foreach (var item in bilgilerDb)
            {
                if(item.tarih.Substring(0,2).Equals(gun) && item.tarih.Substring(3, 2).Equals(ay))
                {
                    gunluk.Push(new tablo() { Id = item.Id,cihazNo=item.cihazNo, durum = item.durum, sure = item.sure, tarih = item.tarih});
                }

                if(weekUst>weekAlt)
                {
                    if (Convert.ToInt32(item.tarih.Substring(0, 2)) >= weekAlt && Convert.ToInt32(item.tarih.Substring(0, 2)) <= weekUst && item.tarih.Substring(3, 2).Equals(ay))
                    {
                        haftalik.Push(new tablo() { Id = item.Id, cihazNo = item.cihazNo, durum = item.durum, sure = item.sure, tarih = item.tarih });
                    }
                }
                else
                {
                    if (Convert.ToInt32(item.tarih.Substring(0, 2)) <= weekAlt && Convert.ToInt32(item.tarih.Substring(0, 2)) >= weekUst && item.tarih.Substring(3, 2).Equals(ay))
                    {
                        haftalik.Push(new tablo() { Id = item.Id, cihazNo = item.cihazNo, durum = item.durum, sure = item.sure, tarih = item.tarih });
                    }
                }


                if (item.tarih.Substring(3, 2).Equals(ay))
                {
                    aylik.Push(new tablo() { Id = item.Id, cihazNo = item.cihazNo, durum = item.durum, sure = item.sure, tarih = item.tarih});
                }

                if (item.tarih.Substring(6, 4).Equals(yil))
                {
                    yillik.Push(new tablo() { Id = item.Id, cihazNo = item.cihazNo, durum = item.durum, sure = item.sure, tarih = item.tarih});
                }



            }

            //son zamanı veritabanindan al
            SayacTarihSaatDb SonTarihSaat = new SayacTarihSaatDb();

            List<SayacTarihSaat> saatiAl = SonTarihSaat.time.ToList();

            //Son Zaman
            DateTime SonTarih = new DateTime(saatiAl[0].Years, saatiAl[0].Mounths, saatiAl[0].Days, saatiAl[0].Hours, saatiAl[0].Minutes, saatiAl[0].Seconds);
            //Güncel Zaman
            DateTime AnlikTarih = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);


            //farkı hesapla

            TimeSpan ZamanFark = AnlikTarih - SonTarih;

            int saatFarki;
            int dakikaFarki;
            int saniyeFarki;

            saatFarki = ZamanFark.Hours + Convert.ToInt32(ZamanFark.TotalHours);
            dakikaFarki = ZamanFark.Minutes;
            saniyeFarki = ZamanFark.Seconds;

            //en güncel sayaç değerini al

            SayacDatabase asdf = new SayacDatabase();

            List<sayac> sayac = asdf.Count.ToList();

            int sayacSaat = sayac[0].Hours;
            int sayacDakika = sayac[0].Minutes;
            int sayacSaniye = sayac[0].Seconds;


            int saat=0, dakika=0, saniye=0;

            if(sayacSaniye+saniyeFarki>=60)
            {
                saniye = (sayacSaniye + saniyeFarki)-60;
                dakika += 1;
            }
            else
            {
                saniye = sayacSaniye + saniyeFarki;
            }

            if(sayacDakika+dakikaFarki>=60)
            {
                dakika=(sayacDakika + dakikaFarki)-60;
                saat += 1;
            }
            else
            {
                dakika = sayacDakika + dakikaFarki;
            }
            saat = sayacSaat + saatFarki;


            //Cihaz durumu verilerini işle

            CihazDurumu status = new CihazDurumu();

            status.cihaz = 1;
            status.hours = saat;
            status.minutes = dakika;
            status.seconds = saniye;

            //Indexe gönderilecek veriler

            Veriler veriler = new Veriler();

            veriler.gunluk = gunluk;
            veriler.haftalik = haftalik;
            veriler.aylik = aylik;
            veriler.yillik = yillik;
            veriler.status = status;        

            return View(veriler);
        }



        //Veritabanina tablo değerlerini kaydet

        [HttpPost]
        public ActionResult Dbkayit(string sure,string sicaklikNem)
        {
            //Zaman al
            DateTime dt = DateTime.Today;
            string yil = "" + dt.Year;
            string ay;
            string gun;

            var date = DateTime.Now;
            string saat;
            string dakika;

            //saati işle
            if (date.Hour < 10)
            {
                saat = "0" + date.Hour;
            }
            else
            {
                saat = "" + date.Hour;
            }

            if (date.Minute < 10)
            {
                dakika = "0" + date.Minute;
            }
            else
            {
                dakika = "" + date.Minute;
            }


            if (dt.Month < 10)
            {
                ay = "0" + dt.Month;
            }
            else
            {
                ay = "" + dt.Month;
            }

            if (dt.Day < 10)
            {
                gun = "0" + dt.Day;
            }
            else
            {
                gun = "" + dt.Day;
            }
            //tablo yaratıp bilgileri işle
            tablo table = new tablo();

            table.cihazNo = "" + 1;
            table.durum = sicaklikNem;
            table.Id = 22;
            table.tarih =""+gun+"."+ay+"."+yil+" - "+saat + "." + dakika;
            table.sure = sure;

            //Veritabanina gönder
           Veritabani veriler = new Veritabani();

            veriler.verilerDb.Add(table);

            veriler.SaveChanges();

            return Json("gelen sayi " + sicaklikNem );
        }

        //Sayacı veritabanina kaydet
        [HttpPost]
        public ActionResult SayacDb(int saat ,int dakika ,int saniye)
        {

            //Sayaci Kaydet
            SayacDatabase asdf = new SayacDatabase();

            var sayac = asdf.Count.Find(1);

            sayac.Hours = saat;
            sayac.Minutes = dakika;
            sayac.Seconds = saniye;

            asdf.SaveChanges();
            
            //Güncel Zamanı kaydet
            SayacTarihSaatDb ZamanKayit = new SayacTarihSaatDb();

            var zaman = ZamanKayit.time.Find(2);

            zaman.Years = DateTime.Now.Year;
            zaman.Mounths = DateTime.Now.Month;
            zaman.Days = DateTime.Now.Day;
            zaman.Hours = DateTime.Now.Hour;
            zaman.Minutes = DateTime.Now.Minute;
            zaman.Seconds = DateTime.Now.Second;

            ZamanKayit.SaveChanges();
            return Json("");
        }


        //iletisim Sayfasi
        public ActionResult iletisim()
        {
            return View();
        }


        //Hakkimizda Sayfası
        public ActionResult hakkimizda()
        {
            return View();
        }

        //Ürünümüz Sayfası
        public ActionResult projemiz()
        {
            return View();
        }
    }
}