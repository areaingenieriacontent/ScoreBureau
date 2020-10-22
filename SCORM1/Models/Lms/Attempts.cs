using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class Attempts
    {
        [Key]
        public int Atte_Id { get; set; }
        [Display(Name = "Fecha de Primer Intento")]
        public DateTime Atte_InintDate { get; set; }
        [Display(Name = "Fecha de Primer Intento")]
        public DateTime Atte_FinishDate { get; set; }

        [ForeignKey("BankQuestion")]
        public int BaQu_Id { get; set; }
        public virtual BankQuestion BankQuestion { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

       
    }
}