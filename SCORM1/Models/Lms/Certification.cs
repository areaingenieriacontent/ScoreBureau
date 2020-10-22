using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class Certification
    {
        [Key]
        public int Cert_Id { get; set; }
        [Display(Name = "Fecha de Certificación")]
        public DateTime Cert_Date { get; set; }


        [ForeignKey("Enrollment")]
        public int Enro_Id { get; set; }
        public virtual Enrollment Enrollment { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}