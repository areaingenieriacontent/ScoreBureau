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
    public class AnswerOptionMultiple
    {
        [Key]
        public int AnOp_Id { get; set; }
        [Display(Name = "Opciones de Respuesta")]
        public string AnOp_OptionAnswer { get; set; }
        [Display(Name = "Respuesta Verdadera")]
        public OPTIONANSWER AnOp_TrueAnswer { get; set; }

        [AllowHtml]
        public string Answer_OpMult_Content { get; set; }

        [ForeignKey("OptionMultiple")]
        public int OpMu_Id { get; set; }
        public virtual OptionMultiple OptionMultiple { get; set; }
    }
}