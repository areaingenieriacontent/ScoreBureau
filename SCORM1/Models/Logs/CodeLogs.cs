using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Logs
{
    public class CodeLogs
    {
        [Key]
        public int CoLo_Id { get; set; }
        [Display(Name = "Codigo Logs")]
        public int CoLo_Code { get; set; }
        [Display(Name = "Descripción")]
        public string CoLo_Description { get; set; }

        public virtual ICollection<Log> Log { get; set; }
    }
}