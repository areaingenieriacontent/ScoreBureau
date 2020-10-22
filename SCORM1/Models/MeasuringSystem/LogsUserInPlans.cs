using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class LogsUserInPlans
    {
        [Key]
        public int idLog { get; set; }
        public string userid { get; set; }
        public DateTime date { get; set; }
        public Plan planid { get; set; }
    }
}