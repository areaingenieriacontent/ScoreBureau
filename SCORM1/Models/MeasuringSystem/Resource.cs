using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }
        [Display(Name = "Tipo de Recurso")]
        public string ResourceType { get; set; }
        [Display(Name = "Nombre del Recurso")]
        public string ResourceName { get; set; }
        [Display(Name = "Contenido")]
        [Required]
        public byte[] Content { get; set; }


        [ForeignKey("Plan")]
        public int PlanId { get; set; }
        public virtual Plan Plan { get; set; }
    }
}