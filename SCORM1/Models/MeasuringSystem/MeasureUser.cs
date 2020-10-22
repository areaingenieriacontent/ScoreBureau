using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class MeasureUser
    {
        [Key]
        public int MausureUserID { get; set; }

        [ForeignKey("Measure")]
        public int MausureId { get; set; }
        public virtual Measure Measure { get; set; }


        [ForeignKey("ApplicationUser")]
        public string UsersId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public string UserEvaluate { get; set; }


    }

}