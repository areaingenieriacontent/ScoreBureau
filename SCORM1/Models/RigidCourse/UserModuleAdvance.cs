using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCORM1.Models.Lms;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{
    public class UserModuleAdvance
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Enrollment")]
        public int Enro_id { get; set; }
        [Key]
        [Column(Order = 1)]
        public int ToCo_id { get; set; }

        public int Completed { get; set; }

        public virtual Enrollment Enrollment { get; set; }

        public virtual TopicsCourse TopicsCourse { get; set; }
    }
}