using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.RigidCourse
{
    public class CategoryQuestionBank
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Category")]
        public int Cate_Id { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("ProtectedFailureTest")]
        public int Modu_Id { get; set; }

        [Display(Name = "Cantidad de preguntas a evaluar")]
        public int EvaluatedQuestionQuantity { get; set; }

        [Display(Name = "Porcentaje de aprobación de la categoría")]
        public float AprovedCategoryPercentage { get; set; }

        public virtual Category Category { get; set; }

        public virtual ProtectedFailureTest ProtectedFailureTest { get; set; }
    }
}