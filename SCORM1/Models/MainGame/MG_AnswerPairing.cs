using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_AnswerPairing
    {

        [Key]
        public int AnPa_Id { get; set; }
        public string Anpa_Answer { get; set; }
        public string AnPa_Question { get; set; }
        public string Anpa_Feedback { get; set; }


        [ForeignKey("MG_Pairing")]
        public int Pairi_Id { get; set; }
        public virtual MG_Pairing MG_Pairing { get; set; }




    }
}