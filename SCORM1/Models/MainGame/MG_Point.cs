using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_Point
    {
        [Key]
        public int point_Id { get; set;}
        public int point_pointOfUser { get; set;}


        [ForeignKey("MG_SettingMp")]
        public int Sett_Id { get; set; }
        public virtual MG_SettingMp MG_SettingMp { get; set; }


        [ForeignKey("ApplicationUser")]
        public string User_Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}