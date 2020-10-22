using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.Games
{
    public class PointsObtainedForUser
    {
        [Key]
        public int PointObtainedId { get; set; }
        [Required]
        public int PointsAssigned { get; set; }
        [Required]
        public string IdUser { get; set; }
        public DateTime? Date { get; set; }
        [Required]
        public int IdLevelCode { get; set; }
        [Required]
        public int IdGame { get; set; }
    }
}