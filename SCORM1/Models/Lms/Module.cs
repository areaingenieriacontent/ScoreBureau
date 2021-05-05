using SCORM1.Enum;
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.Lms
{
    public class Module
    {
        [Key]
        public int Modu_Id { get; set; }
        [Display(Name = "Nombre Modulo")]
        public string Modu_Name { get; set; }
        [Display(Name = "Descripción")]
        public string Modu_Description { get; set; }
        [Display(Name = "Estado Modulo")]
        public MODULESTATE Modu_Statemodule { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Modu_InitDate { get; set; }
        [Display(Name = "Vigencia")]
        public int Modu_Validity { get; set; }
        [Display(Name = "Periodo")]
        public VIGENCIA Modu_Period { get; set; }
        [Display(Name = " Imagen")]
        public string Modu_ImageName { get; set; }
        [Display(Name = "Tipo de Recurso")]
        public string Modu_Image { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string Modu_Content { get; set; }
        [Display(Name = "Puntos Modulo")]
        public int Modu_Points { get; set; }
        [Display(Name = "Tipo de Curso")]
        public CURSO Modu_TypeOfModule { get; set; }
        [Display(Name = "Foro")]
        public FORO Modu_Forum { get; set; }
        [Display(Name = "Buena Practica")]
        public FORO Modu_BetterPractice { get; set; }
        [Display(Name = "Mejora")]
        public FORO Modu_Improvement { get; set; }
        [Display(Name = "Evaluación")]
        public FORO Modu_Test { get; set; }
        //[ForeignKey("CategoryModule")]
        //public int CaMo_Id { get; set; }
        //public virtual CategoryModule CategoryModule { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public int QSMActive { get; set; }//tiene QSM?
        public FORO hasProtectedFailure { get; set; }//tiene falla protegida?
        public virtual ICollection<Improvement> Improvement { get; set; }
        public virtual ICollection<BetterPractice> BetterPractice { get; set; }
        public virtual ICollection<TopicsCourse> TopicsCourse { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
        public virtual ICollection<AdvanceCourse> AdvanceCourse { get; set; }
        public virtual ICollection<Certification> Certification { get; set; }
    }
}