using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Logs
{
    public class IdChange
    {
        [Key]
        public int IdCh_Id { get; set; }
        [Display(Name = "Id Tablas de Cambio")]
        public string IdCh_IdChange { get; set; }


        public virtual ICollection<Log> Log { get; set; }
    }
}