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
    public class Enrollment
    {
        [Key]
        public int Enro_Id { get; set; }
        [Display(Name = "Tipo de Usuario")]
        public string Enro_TypeUser { get; set; }
        [Display(Name = "Comentario")]
        public ENROLLMENTSTATE Enro_StateEnrollment { get; set; }
        public ROLEENROLLMENT Enro_RoleEnrollment { get; set; }
        [Display(Name = "Fecha de Creación")]
        public DateTime Enro_InitDateModule { get; set; }
        [Display(Name = "Vigencia")]
        public DateTime Enro_FinishDateModule { get; set; }

        [ForeignKey("Module")]
        public int Modu_Id { get; set; }
        public virtual Module Module { get; set; }
        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<AdvanceCourse> AdvanceCourse { get; set; }
        public virtual ICollection<Certification> Certification { get; set; }
        public virtual ICollection<Desertify> Desertify { get; set; }
    }
}