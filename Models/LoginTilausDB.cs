using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TilausDBMVC.Models
{
    public class LoginTilausDB
    {
        public int LoginID { get; set; }

        //[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        [Required(ErrorMessage = "Anna käyttäjätunnus")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")]
        [Required(ErrorMessage = "Anna salasana")]
        public string PassWord { get; set; }
        
        public string LoginErrorMessage { get; set; }
    }
}