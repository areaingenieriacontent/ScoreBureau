using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.Lms
{
    public class TrueOrFalseStudent
    {
        [Key]
        public int TrFa_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string TrFa_Question { get; set; }
        [Display(Name = "Descripción")]
        public string TrFa_Description { get; set; }
        [Display(Name = "Respuesta Falsa")]
        [AllowHtml]
        public string TrFa_Content { get; set; }

        public string TrFa_FalseAnswer { get; set; }
        [Display(Name = "Respuesta Verdadera")]
        public string TrFa_TrueAnswer { get; set; }
        [Display(Name = "Puntaje")]
        public string TrFa_Score { get; set; }
        [Display(Name = "Estado Pregunta")]
        public OPTIONANSWER TrFa_State { get; set; }
        public DateTime Date_Present_test { get; set; }

        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("BankQuestion")]
        public int BaQu_Id { get; set; }
        public virtual BankQuestion BankQuestion { get; set; }
    }
}