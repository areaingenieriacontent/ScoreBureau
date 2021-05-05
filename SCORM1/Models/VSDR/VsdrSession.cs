using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace SCORM1.Models.VSDR
{
    public class VsdrSession
    {
        [Key]
        public int id { get; set; }
        [Display(Name ="Título de la sesión")]
        public string name { get; set; }
        [Display(Name ="Caso o contenido")]
        [AllowHtml]
        public string case_content { get; set; }
        [Display(Name ="Fecha de inicio")]
        public DateTime start_date { get; set; }
        [Display(Name = "Fecha de finalización")]
        public DateTime end_date { get; set; }
        [Display(Name = "URL para recurso adicional")]
        public string resource_url { get; set; }
        [Display(Name = "Visible")]
        public bool available { get; set; }
        [Display(Name ="Disponible para todos")]
        public bool open { get; set; }
    }
}