using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using SCORM1.Models.Lms;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class UploadController : Controller
    {

        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }


        public UploadController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

        // GET: Upload
        public ActionResult Index(string id)
        {
            return View(new UploadViewModel
            {
                track = id
            });
        }



        [HttpPost]
        public ActionResult UploadFile()
        {
            var company = GetActualUserId().Company;
            var file = Request.Files[0];
            if (file != null && file.ContentLength > 0)
            {

                var fileName = Path.GetFileName(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + file.FileName);
                var path = Path.Combine(Server.MapPath("/App_Files/Upload"), fileName);
                file.SaveAs(path);
                string Name = fileName;
                string Ruta = path;
                ImageUpload image = new ImageUpload
                {
                    Company = company,
                    CompanyId = company.CompanyId,
                    ImUp_Name = Name,
                    ImUp_Url = Ruta
                };
                ApplicationDbContext.ImageUpload.Add(image);
                ApplicationDbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult ListFiles()
        {
            var company = GetActualUserId().CompanyId;
            var fileData = new List<ViewDataUploadFileResults>();

            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("/App_Files/Upload"));
            if (dir.Exists)
            {
                string[] extensions = MimeTypes.ImageMimeTypes.Keys.ToArray();

                FileInfo[] files = dir.EnumerateFiles()
                                      .Where(f => extensions.Contains(f.Extension.ToLower()))
                                      .ToArray();
                var listimage = ApplicationDbContext.ImageUpload.Where(x => x.CompanyId == company).ToList();
                if (listimage.Count > 0)
                {
                    foreach (var file in listimage)
                    {
                        var relativePath = VirtualPathUtility.ToAbsolute("/App_Files/Upload") + "/" + file.ImUp_Name;

                        fileData.Add(new ViewDataUploadFileResults()
                        {
                            url = relativePath,
                            name = file.ImUp_Name,

                        });
                    }
                }
            }

            return Json(fileData, JsonRequestBehavior.AllowGet);
        }
    }
}