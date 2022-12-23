using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TilausJärjestelmä.Models
{
    using System;
    using System.Collections.Generic;
    public partial class TilausViewModel
    {
        public TilausViewModel(string Asiakas_in, decimal Hinta_in, string Toimitusosoite_in, string Postinumero_in, DateTime Tilauspvm_in,
            DateTime Toimituspvm_in, int TilausID_in)
        {
            Asiakas = Asiakas_in;
            Hinta = Hinta_in;
            Toimitusosoite = Toimitusosoite_in;
            Postinumero = Postinumero_in;
            Tilauspvm = Tilauspvm_in;
            Toimituspvm = Toimituspvm_in;
            TilausID = TilausID_in;
        }
        static public List<TilausViewModel> GetViewModelList()  //Factory funktio, luo listan tämän luokan objekteja tietokannasta
        {
            TilausDBEntities db = new TilausDBEntities();
            List<TilausViewModel> p = new List<TilausViewModel>();
            var tilaukset = from t in db.Tilaukset
                            select t;

            foreach (var i in tilaukset)
            {
                var asiakas = from a in db.Asiakkaat
                              where a.AsiakasID == i.AsiakasID
                              select a;

                var tilausrivi = from tr in db.Tilausrivit
                                 where tr.TilausID == i.TilausID
                                 select tr;
                decimal sum = (decimal)tilausrivi.Sum(Ah => Ah.Ahinta);

                p.Add(new TilausViewModel(asiakas.First().Nimi, sum, i.Toimitusosoite, i.Postinumero,
                    (DateTime)i.Tilauspvm, (DateTime)i.Toimituspvm, i.TilausID));
            }
            db.Dispose();
            return p;
        }

        public string Asiakas { get; set; }

        public decimal Hinta { get; set; }

        public string Toimitusosoite { get; set; }
        public string Postinumero { get; set; }

        public Nullable<System.DateTime> Tilauspvm { get; set; }
        public Nullable<System.DateTime> Toimituspvm { get; set; }

        public int TilausID { get; set; }
    }
}