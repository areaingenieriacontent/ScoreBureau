using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SCORM1.Enum;
using System.Web.Mvc;

namespace SCORM1.Models.Lms
{
    public class Improvement
    {
        [Key]
        public int Impr_Id { get; set; }
        [Display(Name = "Puntos por Mejora")]
        public int Impr_Points { get; set; }
        [Display(Name = "Propuesta")]
        [AllowHtml]
        public string Impr_Comment { get; set; }
        [Display(Name = "Resultado Esperado")]
        [AllowHtml]
        public string Impr_Comment2 { get; set; }
        [Display(Name = "¿Qué pasaba?")]
        public string Impr_Title { get; set; }
        [Display(Name = "Recurso")]
        public string Impr_Resource { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string Impr_Name { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime? Impr_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        public DateTime? Impr_FinishDate { get; set; }
        [Display(Name = "Estado Mejora")]
        public IMPROVEMENTSTATE Impr_StateImprovement { get; set; }

        [ForeignKey("Module")]
        public int Modu_Id { get; set; }
        public virtual Module Module { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}