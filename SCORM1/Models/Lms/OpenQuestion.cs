using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class OpenQuestion
    {
        [Key]
        public int OpQu_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string OpQu_Question { get; set; }
        [Display(Name = "Respuesta")]
        public string OpQu_Answer { get; set; }
        [Display(Name = "Puntaje")]
        public int OpQu_Score { get; set; }

        [ForeignKey("BankQuestion")]
        public int BaQu_Id { get; set; }
        public virtual BankQuestion BankQuestion { get; set; }
        
    }
}