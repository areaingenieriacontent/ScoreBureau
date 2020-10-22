
using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_SettingMp
    {
        [Key]
        public int Sett_Id { get; set; }
        public int Sett_Attemps { get; set; }
        public DateTime Sett_InitialDate { get; set; }
        public DateTime Sett_CloseDate { get; set; }
        public int Sett_NumberOfQuestions { get; set; }
        public string Sett_Instruction { get; set; }
        public string Sett_Audio1 { get; set; }
        public string Sett_Audio2 { get; set; }
        public string Sett_Audio3 { get; set; }
        public string Sett_Audio4 { get; set; }
        public string Sett_Audio5 { get; set; }


        [ForeignKey("MG_Template")]
        public int Plan_Id { get; set; }
        public virtual MG_Template MG_Template { get; set; }

        [ForeignKey("Company")]
        public int Company_Id { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<MG_Pairing> MG_Pairing { get; set; }
        public virtual ICollection<MG_Point> MG_Point { get; set; }
        public virtual ICollection<MG_Order> MG_Order { get; set; }
        public virtual ICollection<MG_MultipleChoice> MG_MultipleChoice { get; set; }
    }
}