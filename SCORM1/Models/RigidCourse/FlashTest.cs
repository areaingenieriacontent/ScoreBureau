using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCORM1.Models.Lms;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{
    public class FlashTest
    {
        [Key]
        public int FlashTestId { get; set; }

        [ForeignKey("TopicsCourse")]
        public int ToCo_Id { get; set; }

        public string FlashTestName { get; set; }

        public float AprovedPercentage { get; set; }

        public virtual TopicsCourse TopicsCourse { get; set; }

        public virtual List<FlashQuestion> questionList { get; set; }
    }
}