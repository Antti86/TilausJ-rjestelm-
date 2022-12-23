using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TilausJärjestelmä.Models
{
    public class TilausLuontiVM
    {
        public IEnumerable<Asiakkaat> asiakkaat { get; set; }

        public IEnumerable<Tuotteet> tuotteet { get; set; }

        public IEnumerable<Postitoimipaikat> postitoimipaikat { get; set; }
    }
}