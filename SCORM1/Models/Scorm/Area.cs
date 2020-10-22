using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.SCORM1
{
    public class Area
    {
        [Key]
        [Display(Name = "Area")]
        public int AreaId { get; set; }
        [Display(Name = "Nombre de Area")]
        public string AreaName { get; set; }
        public string UserId { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        [ForeignKey("UserId")]

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}