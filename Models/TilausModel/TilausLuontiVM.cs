﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TilausJärjestelmä.Models.TilausModel;

namespace TilausJärjestelmä.Models
{
    public class TilausLuontiVM : TilausViewModelBase //Model luokka tilauksen luontiin ja logiikkaan
    {
        //Asiakkaat
        public IEnumerable<Asiakkaat> asiakkaat { get; set; }
        public string selectedAsiakas { get; set; }
        public int? AsiakasID { get; set; }
        public void TyhjennäAsiakasTiedot()
        {
            AsiakasID = null;
            Toimitusosoite = null;
            TilausID = null;
            Postitoimipaikka = null;
            Postinumero = null;
            Tilauspvm = null;
            Toimituspvm = null;
            ToimitusAika = null;
        }

        //Tuotteet
        public IEnumerable<Tuotteet> tuotteet { get; set; }
        public string selectedTuote { get; set; }
        public int maara { get; set; } = 0;
        public int? Tuotenmr { get; set; }
        public void TyhjennäTuoteTeidot()
        {
            maara = 0;
            Tuotenmr = null;
            Hinta = null;
            Rivisumma = null;
        }

        //Rivit
        public decimal? Rivisumma { get; set; }
        public List<TilausRiviVM> aktiivisetRivit { get; set; } = new List<TilausRiviVM> { };
        public bool uusirivi { get; set; } = false;
        public bool rivinpoisto { get; set; } = false;
        public bool tyhjennäKaikkiRivit { get; set; } = false;
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


        //Kokonaistilaisuus
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
        public decimal? KokonaisSumma { get; set; }
        public int? ToimitusAika { get; set; }

        public static void HaeAsiakkaatJaTuotteet(TilausLuontiVM model, TilausDBEntities db)
        {
            model.asiakkaat = from a in db.Asiakkaat
                              select a;
            model.asiakkaat = model.asiakkaat.OrderBy(t => t.Nimi);

            model.tuotteet = from t in db.Tuotteet
                             select t;
            model.tuotteet = model.tuotteet.OrderBy(t => t.Nimi);
        }

    }
}