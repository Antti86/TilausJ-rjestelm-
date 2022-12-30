using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TilausJärjestelmä.Models
{
    public class TilausLuontiVM
    {
        public IEnumerable<Asiakkaat> asiakkaat { get; set; }
        public string selectedAsiakas { get; set; }
        
        public IEnumerable<Tuotteet> tuotteet { get; set; }
        public string selectedTuote { get; set; }
        public int maara { get; set; } = 0;

        public decimal hinta { get; set; }

        public IEnumerable<Postitoimipaikat> postitoimipaikat { get; set; }

        public List<TilausRiviVM> aktiivisetRivit { get; set; } = new List<TilausRiviVM> { };

        public bool? uusirivi { get; set; } = false;

        static public int HaeTilausnumero(TilausDBEntities entity)
        {
            if (entity.Tilaukset.Count() == 0)
            {
                return 100000;
            }
            else
            {
                var tilauk = from i in entity.Tilaukset
                             select i.TilausID;

                return tilauk.Max() + 1;
            }
        }

        static public int HaeTilausRivi(TilausDBEntities entity)
        {
            if (entity.Tilaukset.Count() == 0)
            {
                return 100000;
            }
            else
            {
                var tilauk = from i in entity.Tilausrivit
                             select i.TilausriviID;

                return tilauk.Max() + 1;
            }
        }

    }
}