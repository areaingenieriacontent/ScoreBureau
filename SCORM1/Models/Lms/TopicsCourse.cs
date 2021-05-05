using SCORM1.Enum;
using SCORM1.Models.ratings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.Lms
{
    public class TopicsCourse
    {
        [Key]
        public int ToCo_Id { get; set; }
        [Display(Name = "Nombre Tema")]
        public string ToCo_Name { get; set; }
        [Display(Name = "Descripción")]
        public string ToCo_Description { get; set; }
        [Display(Name = "Intentos Permitidos")]
        public int ToCo_Attempt { get; set; }
        [Display(Name = "Puntaje Esperado")]
        public int ToCo_ExpectedScore { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ToCo_Content { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string ToCo_ContentVirtual { get; set; }
        [Display(Name = "Portada")]

        public string Toco_Image { get; set; }
        [Display(Name = "Total Preguntas")]
        public int ToCo_TotalQuestion { get; set; }
        [Display(Name = "Requerido para Evaluación")]
        public REQUIREDEVALUATION ToCo_RequiredEvaluation { get; set; }

        [Display(Name = "Tipo")]
        public TYPE ToCo_Type { get; set; }

        [Display(Name = "Tipo")]
        public FORO ToCo_Test { get; set; }
        [Display(Name = "Visible")]
        public FORO ToCo_Visible { get; set; }

        [Display(Name ="Primer Tema")]
        public FORO First_Topic { get; set; }

        [Display(Name = "Tema Prerequisito")]
        public int ConditionedTopic { get; set; }

        [Display(Name = "Primer Tema")]
        public bool Topic_Available { get; set; }
        

        [ForeignKey("Module")]
        public int Modu_Id { get; set; }

        public virtual Module Module { get; set; }

        public virtual ICollection<Link> Link { get; set; }
        public virtual ICollection<BankQuestion> BankQuestion { get; set; }
        public virtual ICollection<AdvanceUser> AdvanceUser { get; set; }
        public virtual ICollection<ResourceTopic> ResourceTopic { get; set; }
        public virtual ICollection<ResourceTopics> ResourceTopics { get; set; }
        public virtual ICollection<Job> Job { get; set; }


    }
}