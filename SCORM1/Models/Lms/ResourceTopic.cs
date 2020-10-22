using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class ResourceTopic
    {
        [Key]
        public int ReTo_Id { get; set; }
        [Display(Name = "Nombre")]
        public string ReTo_Name { get; set; }
        [Display(Name = "Ubicación")]
        public string ReTo_Location { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        [ForeignKey("TopicsCourse")]
        public int ToCo_Id { get; set; }
        public virtual TopicsCourse TopicsCourse { get; set; }


    }
}