using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.SCORM1
{
    public class City
    {
        [Key]
        public int City_Id { get; set; }
        [Display(Name = "Ciudad")]
        public string City_Name { get; set; }


        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }
}