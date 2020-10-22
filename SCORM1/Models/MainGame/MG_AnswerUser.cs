using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_AnswerUser
    {
        [Key]
        public int AnUs_Id { get; set; }
        public int Attemps { get; set; }
        public RESPUESTA Respuesta { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public CMD Comodin { get; set; }

        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("MG_AnswerMultipleChoice")]
        public int AnMul_ID { get; set; }
        public virtual MG_AnswerMultipleChoice MG_AnswerMultipleChoice { get; set; }
    }
}