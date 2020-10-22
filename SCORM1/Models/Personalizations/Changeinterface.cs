using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Personalizations
{
    public class Changeinterface
    {
        [Key]
        public int ChIn_Id { get; set; }
        [Display(Name = "Tipografia")]
        public string ChIn_FontType { get; set; }
        [Display(Name = "Color Banner")]
        public string ChIn_ColorBanner { get; set; }
        [Display(Name = "Estilo de Botones")]
        public string ChIn_StyleButton { get; set; }
        [Display(Name = "Fondo ")]
        public string ChIn_Background { get; set; }
        [Display(Name = "Logo")]
        public string ChIn_Logo { get; set; }


        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}