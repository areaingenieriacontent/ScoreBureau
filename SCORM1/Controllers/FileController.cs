using SCORM1.Models;
using SCORM1.Models.MeasuringSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGrease.Activities;

namespace SCORM1.Controllers
{
    public class FileController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        public FileController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
        }

        // GET: File
        public ActionResult GetFile(int id)
        {
            Resource resourceToLoad = ApplicationDbContext.Resources.Find(id);
            return File(resourceToLoad.Content, resourceToLoad.ResourceType);
        }

        public ActionResult GetFileEmail(string id)
        {
            string idUser = id.Substring(0, 35);
            string plans = id.Substring(37);
            LogsUserInPlans Logs = new LogsUserInPlans { userid = idUser, date = DateTime.Now, planid = ApplicationDbContext.Plans.Find(Int32.Parse(plans)) };
            Resource resource = ApplicationDbContext.Resources.Find(Int32.Parse(plans));
            ApplicationDbContext.LogsUserInPlans.Add(Logs);
            ApplicationDbContext.SaveChanges();
            return File(resource.Content, resource.ResourceType);
        }
    }
}