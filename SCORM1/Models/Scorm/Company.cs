using SCORM1.Enum;
using SCORM1.Models.Engagement;
using SCORM1.Models.Lms;
using SCORM1.Models.Logs;
using SCORM1.Models.MainGame;
using SCORM1.Models.MeasuringSystem;
using SCORM1.Models.Newspaper;
using SCORM1.Models.Personalizations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCORM1.Models.SCORM1
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        [Display(Name = "Tipo")]
        public COMPANY_TYPE CompanyType { get; set; }
        [Display(Name = "Sector")]
        public COMPANY_SECTOR CompanySector { get; set; }
        [Display(Name = "Nombre")]
        public string CompanyName { get; set; }
        [Display(Name = "Nit")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(12)]
        [Index(IsUnique = true)]
        public string CompanyNit { get; set; }
        [Display(Name = "Juego")]
        public GAME CompanyGame { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
        public virtual ICollection<Area> Area { get; set; }
        public virtual ICollection<Location> Location { get; set; }
        public virtual ICollection<Position> Position { get; set; }
        public virtual ICollection<Edition> Edition { get; set; }
        public virtual ICollection<Prize> Prize { get; set; }
        public virtual ICollection<Measure> Measure { get; set; }
        public virtual ICollection<Changeinterface> Changeinterface { get; set; }
        public virtual ICollection<CategoryModule> CategoryModule { get; set; }
        public virtual ICollection<CategoryPrize> CategoryPrize { get; set; }
        public virtual ICollection<ResourceTopic> ResourceTopic { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
        public virtual ICollection<PointManagerCategory> PointManagerCategory { get; set; }
        public virtual ICollection<Module> Module { get; set; }
        public virtual ICollection<ImageUpload> ImageUpload { get; set; }
        public virtual ICollection<Log> Log { get; set; }
        public virtual ICollection<MG_SettingMp> MG_SettingMp { get; set; }
        public virtual ICollection<MG_Template> MG_Template { get; set; }
       
    }
}