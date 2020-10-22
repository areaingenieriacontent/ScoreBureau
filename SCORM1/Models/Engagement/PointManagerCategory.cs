using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Engagement
{
    public class PointManagerCategory
    {

        [Key]
        public int PoMaCa_Id { get; set; }
        [Display(Name = "Puntos de comentarios periodico")]
        public int PoMaCa_Periodical { get; set; }
        [Display(Name = "Puntos de comentarios buenas Practicas ")]
        public int PoMaCa_course { get; set; }
        [Display(Name = "Puntos Comentarios Mejoras ")]
        public int PoMaCa_Improvements { get; set; }


        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<Prize> Prize { get; set; }

      
        
    }
}