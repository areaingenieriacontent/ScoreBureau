using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCORM1.Models.Lms;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{

    public class Category
    {
        [Key]
        public int Cate_Id { get; set; }

        [Display(Name = "Nombre Categoría")]
        public string Cate_Name { get; set; }

        [Display(Name = "Descripción Categoría")]
        public string Cate_Desc { get; set; }

        [Display(Name = "Tema")]
        [ForeignKey("TopicCourse")]
        public int ToCo_Id { get; set; }

        public virtual TopicsCourse TopicCourse { get; set; }



    }
}