using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MainGame
{
    public class MG_AnswerOrder
    {
        [Key]
        public int AnOr_ID { get; set;}
        public string AnOr_Description { get; set;}

        [ForeignKey("MG_Order")]
        public int Order_Id { get; set; }
        public virtual MG_Order MG_Order { get; set; }

    }
}