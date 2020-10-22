using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_Pairing
    {
        [Key]
        public int Pairi_Id { get; set; }
        public string Pairi_NameQuestion { get; set; }
        public string Pairi_Description { get; set; }
        public string Pairi_Feedback { get; set; }
        public LEVEL Pairi_Level { get; set;}


        [ForeignKey("MG_SettingMp")]
        public int Sett_Id { get; set; }
        public virtual MG_SettingMp MG_SettingMp { get; set; }

        public virtual ICollection<MG_AnswerPairing> MG_AnswerPairing { get; set; }

    }
}