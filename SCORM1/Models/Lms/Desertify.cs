using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class Desertify
    {
        [Key]
        public int Dese_Id { get; set; }
        [Display(Name = "Fecha de Certificación")]
        public DateTime Dese_Date { get; set; }


        [ForeignKey("BankQuestion")]
        public int BaQu_Id { get; set; }
        public virtual BankQuestion BankQuestion { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("Enrollment")]
        public int Enro_Id { get; set; }
        public virtual Enrollment Enrollment { get; set; }
    }
}