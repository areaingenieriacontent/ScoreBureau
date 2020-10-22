using SCORM1.Models.SCORM1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_Template
    {
        [Key]
        public int Plant_Id { get; set; }
        public string Plant_Color { get; set; }
        public string Plant_Img_instructions { get; set; }
        public string Plant_Img_Questions { get; set; }


        [ForeignKey("Company")]
        public int Company_Id { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<MG_SettingMp> MG_SettingMp { get; set; }
    }
}