using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace SCORM1.Models.VSDR
{
    public class VsdrTeacherComment
    {
        [Key]
        public int comment_id { get; set; }
        [Column(Order = 0)]
        [ForeignKey("Enrollment")]
        public string user_id { get; set; }
        [Column(Order = 1)]
        [ForeignKey("Enrollment")]
        public int vsdr_id { get; set; }
        public string teacher_id { get; set; }
        [AllowHtml]
        public string content { get; set; }
        public DateTime commentDate { get; set; }
        public virtual VsdrEnrollment Enrollment { get; set; }
    }
}