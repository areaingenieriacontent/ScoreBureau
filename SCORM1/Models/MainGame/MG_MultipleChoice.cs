using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_MultipleChoice
    {
        [Key]
        public int MuCh_ID { get; set;}
        public string MuCh_Description { get; set;}
        public string MuCh_NameQuestion { get; set;}
        public string MuCh_ImageQuestion { get; set; }
        public string MuCh_Feedback { get; set;}
        public LEVEL MuCh_Level { get; set;}


        [ForeignKey("MG_SettingMp")]
        public int Sett_Id { get; set; }
        public virtual MG_SettingMp MG_SettingMp { get; set; }

        public virtual ICollection<MG_AnswerMultipleChoice> MG_AnswerMultipleChoice { get; set; }
    }
}