using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }
        [Display(Name = "Descripcion")]
        public string PlanDescription { get; set; }
        [Display(Name = "Puntaje Minimo")]
        public int PlanMinScore { get; set; }
        [Display(Name = "Puntaje Maximo")]
        public int PlanMaxScore { get; set; }
        [Display(Name = "Tipo de Plan")]
        public PLAN_TO PlanTo { get; set; }
        [Display(Name = "Recurso")]
        public int ResourceId { get; set; }
        public int ProficiencyId { get; set; }

        public virtual Proficiency Proficiency { get; set; }
        public ICollection<Resource> Resource { get; set; }
    }
}