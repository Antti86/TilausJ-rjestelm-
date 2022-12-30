using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TilausJärjestelmä.Models
{
    public class TilausRiviVM //ViewModel luokka tilausriveille
    {
        public TilausRiviVM(string Tuote_in, int maara_in, decimal Ahinta_in, int TilausriviID_in)
        {
            Tuote = Tuote_in;
            maara = maara_in;
            Ahinta = Ahinta_in;
            TilausriviID = TilausriviID_in;
        }

        static public List<TilausRiviVM> GetViewModelList(int id)   //Factory funktio, luo listan tämän luokan objekteja tietokannasta
        {
            TilausDBEntities db = new TilausDBEntities();
            List<TilausRiviVM> p = new List<TilausRiviVM>();

            var rivit = from r in db.Tilausrivit
                        where r.TilausID == id
                        select r;

            foreach (var i in rivit)
            {
                var tuoteet = from tuo in db.Tuotteet
                              where tuo.TuoteID == i.TuoteID
                              select tuo;

                p.Add(new TilausRiviVM(tuoteet.First().Nimi, (int)i.Maara, (decimal)i.Ahinta, i.TilausriviID));
            }
            db.Dispose();
            return p;
        }
        public string Tuote { get; set; }

        public int maara { get; set; }

        public decimal Ahinta { get; set; }

        public int TilausriviID { get; set; }
    }
}