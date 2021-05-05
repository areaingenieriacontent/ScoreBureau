using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCORM1.Models.Lms;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{
    public class ProtectedFailureResults
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Enrollment")]
        public int Enro_id { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Category")]
        public int Cate_Id { get; set; }
        
        public int correctAnswersQuantity { get; set; }

        public float Score { get; set; }

        public virtual Enrollment Enrollment { get; set; }

        public virtual Category Category { get; set; }
    }
}