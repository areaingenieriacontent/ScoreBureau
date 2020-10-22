using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Engagement
{
    public class TypePoint
    {
        [Key]
        public int TyPo_Id { get; set; }
        [Display(Name = "Descripción")]
        public string TyPo_Description { get; set; }
        public TYPEPOINTS Poin_TypePoints { get; set; }

        public virtual ICollection<Point> Point { get; set; }
    }
}