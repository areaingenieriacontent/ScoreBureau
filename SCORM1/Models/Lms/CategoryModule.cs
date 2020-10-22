using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Lms
{
    public class CategoryModule
    {
        [Key]
        public int CaMo_Id { get; set; }
        [Display(Name = "Categoria")]
        public string CaMo_Category { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        //public virtual ICollection<Module> Module { get; set; }
    }
}