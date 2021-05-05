using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.RigidCourse
{
    public class ProtectedFailureMultiChoiceAnswer
    {
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("ProtectedFailureAnswer")]
        public int AnswerId { get; set; }
        public virtual ProtectedFailureAnswer ProtectedFailureAnswer { get; set; }
    }
}