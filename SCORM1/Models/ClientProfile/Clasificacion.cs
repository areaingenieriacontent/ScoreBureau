using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ClientProfile
{
    public class Clasificacion
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Clasificación")]
        public string name { get; set; }
    }
}