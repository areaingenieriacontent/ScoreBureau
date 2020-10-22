using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class NewAttempts
    {

        [Key]
        public int NeAt_Id { get; set; }
        [Display(Name = "Fecha de Nuevo Intento")]
        public DateTime NeAt_DateInint { get; set; }
        public ATTEMPTS Attempts { get; set; }

        [ForeignKey("BankQuestion")]
        public int BaQu_Id { get; set; }
        public virtual BankQuestion BankQuestion { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}