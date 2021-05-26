using SCORM1.Enum;
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Logs
{
    public class Log
    {
        [Key]
        public int Log_Id { get; set; }
        [Display(Name = "Descripción")]
        public string Log_Description { get; set; }
        [Display(Name = "Estado Modulo")]
        public LOGSTATE Log_StateLogs { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Log_Date { get; set; }
        [Display(Name = "Ip ")]
        public string Log_Ip { get; set; }


        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("CodeLogs")]
        public int CoLo_Id { get; set; }
        public virtual CodeLogs CodeLogs { get; set; }
        [ForeignKey("TableChange")]
        public int TaCh_Id { get; set; }
        public virtual TableChange TableChange { get; set; }
        [ForeignKey("IdChange")]
        public int IdCh_Id { get; set; }
        public virtual IdChange IdChange { get; set; }
        [ForeignKey("Company")]
        public int Company_Id { get; set; }
        public virtual Company Company { get; set; }
    }
}