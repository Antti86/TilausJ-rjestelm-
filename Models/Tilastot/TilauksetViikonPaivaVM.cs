using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TilausJärjestelmä.Models.Tilastot
{
    public class TilauksetViikonPaivaVM
    {

        public Dictionary<string, int> Viikko
        {
            get
            {
                Dictionary<string, int> _Viikko = new Dictionary<string, int>()
                {
                    {"Maanantai", 0 },
                    {"Tiistai", 0 },
                    {"Keskiviikko", 0 },
                    {"Torstai", 0 },
                    {"Perjantai", 0 },
                    {"Lauantai", 0 },
                    {"Sunnuntai", 0 }
                };
                TilausDBEntities db = new TilausDBEntities();

                var tilaukset = from t in db.Tilaukset
                                select t.Tilauspvm;

                foreach( var i in tilaukset )
                {
                    var culture = new System.Globalization.CultureInfo("fi-FI");
                    string day = culture.DateTimeFormat.GetDayName(i.Value.DayOfWeek);
                    day = char.ToUpper(day[0]) + day.Substring(1);
                    _Viikko[day] += 1;
                }
                return _Viikko;
            }
        }


    }
}