using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Logs
{
    public class TableChange
    {
        [Key]
        public int TaCh_Id { get; set; }
        [Display(Name = "Nombre Tablas")]
        public string TaCh_TableName { get; set; }


        public virtual ICollection<Log> Log { get; set; }
    }
}