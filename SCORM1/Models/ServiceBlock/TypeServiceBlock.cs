using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ServiceBlock
{
    public class TypeServiceBlock
    {
        [Key]
        public int TySe_Id { get; set; }
        [Display(Name = "Descripción")]
        public string TySe_Description { get; set; }

        public virtual ICollection<BlockService> BlockService { get; set; }
    }
}