using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.Lms
{
    public class OptionMultiple
    {
        [Key]
        public int OpMu_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string OpMu_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int OpMu_Score { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string OpMu_Description { get; set; }
        [AllowHtml]
        public string OpMult_Content { get; set; }
        [ForeignKey("BankQuestion")]
        public int BaQu_Id { get; set; }
        public virtual BankQuestion BankQuestion { get; set; }

        public virtual ICollection<AnswerOptionMultiple> AnswerOptionMultiple { get; set; }
        public virtual ICollection<AnswerOptionMultipleStudent> AnswerOptionMultipleStudent { get; set; }

    }
}