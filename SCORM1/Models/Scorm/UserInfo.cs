using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.SCORM1
{
    public class UserInfo
    {
        [Key]
        [Display(Name = "UserInfo")]
        public int Nombreprueba { get; set; }
        [Display(Name = "Nombrepreuab")]
        public string Jajaja { get; set; }
       
    }
}