using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Engagement
{
    public class CategoryPrize
    {
        [Key]
        public int Capr_Id { get; set; }
        [Display(Name = "Nombre categoria")]
        public string Capr_category { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<Prize> Prize { get; set; }
    }
}