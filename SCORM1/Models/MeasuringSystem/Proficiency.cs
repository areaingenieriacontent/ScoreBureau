using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class Proficiency
    {
        [Key]
        public int ProficiencyId { get; set; }
        [Display(Name = "Descripcion")]
        public string ProficiencyDescription { get; set; }
    }
}