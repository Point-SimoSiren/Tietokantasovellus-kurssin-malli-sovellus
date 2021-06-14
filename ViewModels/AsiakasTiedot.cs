using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TilausDBMVC.Models;

namespace TilausDBMVC.ViewModels
{
    public class AsiakasTiedot
    {
        public AsiakasTiedot()
        {
            this.Tilaukset = new HashSet<AsiakasTiedot>();
        }

        public int AsiakasID { get; set; }
        public Nullable<int> PostiID { get; set; }

        [Display(Name = "Yritys")]
        public string Nimi { get; set; }

        //Lisätty yhdistävät nimikentät

        [Display(Name = "Asiakas Etunimi")]
        public string EtuNimiAsiakas { get; set; }
        [Display(Name = "Asiakas Sukunimi")]
        public string SukuNimiAsiakas { get; set; }

        [Display(Name = "Asiakas")]
        public string KokoNimiAsiakas
        {
            get { return EtuNimiAsiakas + " " + SukuNimiAsiakas; }
        }


        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Osoite { get; set; }
        public string Postinumero { get; set; }

        public string Postitoimipaikka { get; set; }

        public string Postnro { get; set; }
        public string Postplace { get; set; }



        public virtual Postitoimipaikat Postitoimipaikat { get; set; }

        public virtual ICollection<AsiakasTiedot> Tilaukset { get; set; }
    }
}