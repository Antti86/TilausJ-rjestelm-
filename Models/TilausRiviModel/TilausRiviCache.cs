using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TilausJärjestelmä.Models
{

    public class Cacher //Wrapperi luokka HttpContext web memory Cachelle, tee geneerinen tarvittaessa.. Käyttää aina samaa avainta..
    {
        private List<TilausRiviVM> _CacheLista = null; //Tämä että lista palauttaa tyhjän listan eikä null arvoa
        public List<TilausRiviVM> CacheLista
        {
            get
            {
                if (_CacheLista == null)
                {
                    _CacheLista = (HttpContext.Current.Cache["List"] as List<TilausRiviVM>);
                    if (_CacheLista == null)
                    {
                        _CacheLista = new List<TilausRiviVM>();
                        HttpContext.Current.Cache.Insert("List", _CacheLista);
                    }
                }
                return _CacheLista;
            }
            set
            {
                HttpContext.Current.Cache.Insert("List", _CacheLista);
            }
        }

        public void PoistaRivi(int index)
        {
            if (CacheLista.Count > 0)
            {
                CacheLista.RemoveAt(index);
            }

        }
        public void TyhjennäLista() //Tyhjentää koko listan
        {
            HttpContext.Current.Cache.Remove("List");
        }

    }
}