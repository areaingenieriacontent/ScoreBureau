using SCORM1.Enum;
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class BankQuestion
    {
        [Key]
        public int BaQu_Id { get; set; }
        [Display(Name = "Descripción de la Pregunta")]
        public string BaQu_Description { get; set; }
        [Display(Name = "Nombre Pregunta")]
        public string BaQu_Name { get; set; }
        [Display(Name = "Porcentaje de Evaluación")]
        public int BaQu_Porcentaje { get; set; }
        [Display(Name = "Porcentaje de Evaluación")]
        public int BaQu_Porcentaje2 { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime BaQu_InintDate { get; set; }
        [Display(Name = "Fecha de Finalización")]
        public DateTime BaQu_FinishDate { get; set; }
        [Display(Name = "Total Preguntas")]
        public int BaQu_TotalQuestion { get; set; }
        [Display(Name = "Preguntas para el usuario")]
        public int BaQu_QuestionUser { get; set; }
        [Display(Name = "Seleccionar Preguntas")]
        public FORO BaQu_SelectQuestion { get; set; }
        [Display(Name = "Attempts")]
        public int BaQu_Attempts { get; set; }


        [ForeignKey("TopicsCourse")]
        public int ToCo_Id { get; set; }
        public virtual TopicsCourse TopicsCourse { get; set; }
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<OpenQuestion> OpenQuestion { get; set; }
        public virtual ICollection<TrueOrFalse> TrueOrFalse { get; set; }
        public virtual ICollection<TrueOrFalseStudent> TrueOrFalseStudent { get; set; }
        public virtual ICollection<Pairing> Pairing { get; set; }
        public virtual ICollection<OptionMultiple> OptionMultiple { get; set; }
        public virtual ICollection<Attempts> Attempts { get; set; }
        public virtual ICollection<NewAttempts> NewAttempts { get; set; }
        public virtual ICollection<Desertify> Desertify { get; set; }
    }
}