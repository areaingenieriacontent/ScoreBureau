using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_AnswerMultipleChoice
    {
        [Key]
        public int AnMul_ID { get; set;}
        public string AnMul_Description { get; set;}
        public OPTIONANSWER AnMul_TrueAnswer { get; set; }

        [ForeignKey("MG_MultipleChoice")]
        public int MuCh_ID { get; set; }
        public virtual MG_MultipleChoice MG_MultipleChoice { get; set; }

        public virtual ICollection<MG_AnswerUser> MG_AnswerUser { get; set; }
    }
}