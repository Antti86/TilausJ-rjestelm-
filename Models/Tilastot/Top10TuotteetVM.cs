using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace TilausJärjestelmä.Models.Tilastot
{
    public class Top10TuotteetVM
    {
        public Top10TuotteetVM()
        {

        }
        private Top10TuotteetVM(string _Nimi, decimal _Arvo, int? _Maara)
        {
            Nimi = _Nimi;
            Arvo = _Arvo;
            Maara = _Maara;
        }
        public string Nimi { get; private set; }

        public decimal Arvo { get; private set; }

        public int? Maara { get; private set; }

        public override string ToString()
        {
            return Nimi;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this.Nimi == ((Top10TuotteetVM)obj).Nimi;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public List<Top10TuotteetVM> Top10 
        {
            get
            {
                List<Top10TuotteetVM> _Top10 = new List<Top10TuotteetVM>(); //Luodaan lista mihin kasataan arvot
                TilausDBEntities db = new TilausDBEntities();

                var rivit = from r in db.Tilausrivit    //Haetaan tietokannasta kaikki tilausrivit
                            select r;

                foreach (var i in rivit)
                {
                    var nimi = from t in db.Tuotteet        //Haetaan tuotteen nimi id:n perusteella tuotetaulusta
                               where i.TuoteID == t.TuoteID
                               select t;

                    //Luodaan objekti tietokannan tiedoista
                    Top10TuotteetVM temp = new Top10TuotteetVM(nimi.First().Nimi, (decimal)i.Ahinta, i.Maara);

                    if (_Top10.Contains(temp))  //Tarkistetaan onko tuote jo listalla, jos on niin lisätään sen arvoa
                    {                           //Contain methodi toimii pelkällä temp parametrillä,
                                                //koska tämä luokka overridaa Equals() methodin

                        int e = _Top10.IndexOf(temp);
                        _Top10[e].Arvo += (decimal)i.Ahinta;
                    }
                    else     //Jos ei ole niin lisätään tuote listalle, nimen perusteella
                    {
                        _Top10.Add(temp);
                    }
                }

                _Top10.Sort((a, b) => b.Arvo.CompareTo(a.Arvo));    //Järjestellään lista isoimman arvon mukaan
                _Top10.RemoveRange(10, _Top10.Count - 10);  //Poistetaan listalta kaikki 10:n jälkeen
                db.Dispose();
                return _Top10;
            }
        }
    }
}