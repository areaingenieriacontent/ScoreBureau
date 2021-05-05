using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCORM1.Models.Lms;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{
    public class ProtectedFailureTest
    {
        [Key, ForeignKey("Module")]
        public int Modu_Id { get; set; }

        [Display(Name = "Nombre Test")]
        public string PF_Name { get; set; }

        [Display(Name = "Descripción Test")]
        public string PF_Description { get; set; }

        [Display(Name = "Porcentaje de aprobación general")]
        public float GeneralAproveScore { get; set; }
        public DateTime DateCreated { get; set; }
        public bool testAvailable { get; set; }

        public virtual Module Module { get; set; }
    }
}