using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TilausJärjestelmä.Models.Tilastot
{
    public class TilastoKokoelmaVM
    {
        public Top10TuotteetVM Top10Tuotelista { get; set; } = new Top10TuotteetVM();

        public TilauksetViikonPaivaVM ViikkoTilauslista { get; set; } = new TilauksetViikonPaivaVM();
    }
}