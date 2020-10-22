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
    public class BetterPractice
    {
        [Key]
        public int BePr_Id { get; set; }
        [Display(Name = "Puntos por Buena Practica")]
        public int BePr_Points { get; set; }
        [Display(Name = "Resultado Obtenido")]
        [AllowHtml]
        public string BePr_Comment { get; set; }
        [Display(Name = "¿Qué se Hizo?")]
        public string BePr_TiTle { get; set; }
        [Display(Name = "Recurso")]
        public string BePr_Resource { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string BePr_Name { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime? BePr_InitDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        public DateTime? BePr_FinishDate { get; set; }
        [Display(Name = "Estado Mejora")]
        public BETTERPRACTICESTATE BePr_StateBetterpractice { get; set; }


        [ForeignKey("Module")]
        public int Modu_Id { get; set; }
        public virtual Module Module { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<ResourceBetterPractice> ResourceBetterPractice { get; set; }
    }
}