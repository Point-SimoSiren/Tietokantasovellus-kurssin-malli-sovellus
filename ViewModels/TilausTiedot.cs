using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TilausDBMVC.Models;

namespace TilausDBMVC.ViewModels
{
    public class TilausTiedot
    {
        public TilausTiedot()
        {
            this.Tilausrivit = new HashSet<TilausTiedot>();
        }

        public int TilausID { get; set; }
        public int? AsiakasID { get; set; }
        public int? PostiID { get; set; }
        public string Toimitusosoite { get; set; }
        public string Nimi { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy\\-MM\\-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tilaus pvm")]
        public Nullable<System.DateTime> Tilauspvm { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]   
        //[DisplayFormat(DataFormatString = "{0:yyyy\\-MM\\-dd}", ApplyFormatInEditMode = true)]     
        [Display(Name = "Toimitus pvm")]      
        public DateTime? Toimituspvm { get; set; }
        
        public string Postinumero { get; set; }
        public string Postitoimipaikka { get; set; }

        public virtual Asiakkaat Asiakkaat { get; set; }
        public virtual Postitoimipaikat Postitoimipaikat { get; set; }
      
        public virtual ICollection<TilausTiedot> Tilausrivit { get; set; }
    }
}