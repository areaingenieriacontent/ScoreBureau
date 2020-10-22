using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [Display(Name = "Descripción")]
        public string QuestionDescription { get; set; }
        [Display(Name = "Competencia")]
        public int ProficiencyId { get; set; }
        [Display(Name = "Tipo de Pregunta")]
        public QUESTION_TYPE QuestionType { get; set; }

        [ForeignKey("ProficiencyId")]
        public virtual Proficiency Proficiency { get; set; }

        public virtual ICollection<Test> Tests { get; set; }
    }
}