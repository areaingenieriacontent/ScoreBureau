using SCORM1.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_Order
    {
        [Key]
        public int Order_Id { get; set;}
        public string Order_NameQuestion { get; set; }
        public string Order_Description { get; set;}
        public LEVEL Order_Level { get; set; }

        [ForeignKey("MG_SettingMp")]
        public int Sett_Id { get; set; }
        public virtual MG_SettingMp MG_SettingMp { get; set; }

        public virtual ICollection<MG_AnswerOrder> MG_AnswerOrder { get; set; }
    }
}