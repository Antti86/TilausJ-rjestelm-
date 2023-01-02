using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TilausJärjestelmä.Models.TilausModel
{
    public abstract class TilausViewModelBase
    {
        public string Asiakas { get; set; }

        public decimal? Hinta { get; set; }

        public string Toimitusosoite { get; set; }
        public string Postinumero { get; set; }

        public string Postitoimipaikka { get; set; }
        public Nullable<System.DateTime> Tilauspvm { get; set; }
        public Nullable<System.DateTime> Toimituspvm { get; set; }

        public int? TilausID { get; set; }
    }
}