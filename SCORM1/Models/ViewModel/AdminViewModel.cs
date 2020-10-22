using SCORM1.Models.PageCustomization;
using SCORM1.Models.Engagement;
using SCORM1.Models.Lms;
using SCORM1.Models.Logs;
using SCORM1.Models.Newspaper;
using SCORM1.Models.Personalizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ViewModel
{
    public class AdminViewModel : BaseViewModel
    {
        public Banner Banner { get; set; }
        public Changeinterface Changeinterface { get; set; }
        public List<Module> Module { get; set; }
        public List<Edition> Edition { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class AdminEditionViewModel : BaseViewModel
    {
        public List<Edition> Editions { get; set; }
    }

    public class AminLMSViewModel : BaseViewModel
    {
        public List<Module> Modules { get; set; }
    }

    public class AdminEngagementViewModel : BaseViewModel
    {
        public List<Prize> Prizes { get; set; }
        public List<Exchange> Exchanges { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }

    public class AdminManagementUser : BaseViewModel
    {
        public List<ApplicationUser> Users { get; set; }
    }

    public class AdminReportsViewModel : BaseViewModel
    {
        public List<Module> Modules { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<Edition> Editions { get; set; }
        public List<Exchange> Exchange { get; set; }

    }
    public class AdminLogsViewModel : BaseViewModel
    {
        public List<Log> Logs { get; set; }
    }
}