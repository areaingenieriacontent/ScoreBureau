using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class ResourceTopics
    {
        [Key]
        public int ReMo_Id { get; set; }
        [Display(Name = "Nombre")]
        public string ReMo_NameResource { get; set; }
        [Display(Name = "Nombre Archivo")]
        public string ReMo_Name { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime? ReMo_InitDate { get; set; }

        [ForeignKey("TopicsCourse")]
        public int ToCo_Id { get; set; }

        public virtual TopicsCourse TopicsCourse { get; set; }

    }
}