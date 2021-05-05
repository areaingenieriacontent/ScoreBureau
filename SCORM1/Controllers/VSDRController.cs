
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using SCORM1.Models.PageCustomization;
using PagedList;
using SCORM1.Models.Logs;
using SCORM1.Models.ratings;
using System.Security.Cryptography.X509Certificates;
using SCORM1.Models.RigidCourse;
using iTextSharp.text.pdf.events;
using SCORM1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models.ViewModel;
using SCORM1.Models.VSDR;
using System.Collections.Generic;
using System;

namespace SCORM1.Controllers
{
    public class VSDRController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public VSDRController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET: VSDR
        public ActionResult Index()
        {
            return View();
        }
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

        #region VSDR Users
        #endregion

        #region VSDR Teachers
        //Returns all available VSDR sessions available
        [Authorize]
        public ActionResult VsdrUserList()
        {
            VsdrUserVM vsdrModel = new VsdrUserVM();
            //Get list from data base;
            List<VsdrSession> vsdrList= new List<VsdrSession>();
            List<VsdrSession> tempList= new List<VsdrSession>();
            vsdrList = ApplicationDbContext.VsdrSessions.ToList();

            //Removes not available and date expired vsdr sessions To-Do
            if (vsdrList.Count > 0)
            {
                foreach (VsdrSession debateRoom in vsdrList)
                {
                    if (!debateRoom.available)
                    {
                        tempList.Add(debateRoom);
                    }
                    else if (debateRoom.end_date < DateTime.Now)
                    {
                        tempList.Add(debateRoom);
                    }
                }
                foreach (VsdrSession debateRoom in tempList)
                {
                    vsdrList.Remove(debateRoom);
                }
            }
            //To-Do, filter list by enrollment and open
            vsdrModel.listOfVsdr = vsdrList;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }

        [Authorize]
        public ActionResult VsdrContent(int id)
        {
            var actualUser = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            VsdrUserVM vsdrModel = new VsdrUserVM();
            VsdrSession vsdrToReturn = ApplicationDbContext.VsdrSessions.Find(id);
            List<VsdrUserFile> vsdrUserIssuedFiles = ApplicationDbContext.VsdrUserFiles.Where(x => x.user_id == actualUser.Id).ToList();
            List<VsdrTeacherComment> vsdrTeacherComments = ApplicationDbContext.VsdrTeacherComments.Where(x => x.user_id == actualUser.Id).ToList();
            VsdrUserFile file = new VsdrUserFile(); 
            if (vsdrToReturn.end_date.Subtract(DateTime.Now).TotalMinutes < 15)
            {
                vsdrModel.meetingAvailable = true;
            }
            //add loaded data to view model
            vsdrModel.vsdrFileToAdd = file;
            vsdrModel.actualVsdr = vsdrToReturn;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }

        //Function thats receive the view model information and an uploaded file
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVSDRFile(VsdrUserVM vsdrModel, HttpPostedFileBase upload)
        {
            var actualUser = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var vsdre = ApplicationDbContext.VsdrSessions.Find(vsdrModel.actualVsdr.id);
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            if (upload != null && upload.ContentLength > 0 && upload.ContentLength <= (2 * 1000000))
            {
                string[] allowedExtensions = new[] { ".pdf", ".doc", ".pptx", ".xls", ".xlsx", ".docx" };
                var ext = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                var file = "";
                foreach(var extention in allowedExtensions)
                {
                    if (extention.Contains(ext))
                    {
                        file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                        upload.SaveAs(Server.MapPath("~/VSDRUploads/" + file));
                        VsdrUserFile fileToAdd = new VsdrUserFile
                        {
                            user_id = actualUser.Id,
                            vsdr_id = vsdrModel.actualVsdr.id,
                            register_name = vsdrModel.vsdrFileToAdd.register_name,
                            file_description = vsdrModel.vsdrFileToAdd.file_description,
                            file_extention = ext,
                            file_name = file,
                            registered_date = DateTime.Now
                        };
                        ApplicationDbContext.VsdrUserFiles.Add(fileToAdd);
                        ApplicationDbContext.SaveChanges();
                        TempData["Info"] = "Archivo cargado exitosamente";
                        List<VsdrUserFile> fileList = ApplicationDbContext.VsdrUserFiles.Where(x => x.user_id == actualUser.Id).ToList();
                        List<VsdrTeacherComment> teacherComments = ApplicationDbContext.VsdrTeacherComments.Where(x => x.user_id == actualUser.Id).ToList();
                        vsdrModel.listOfIssuedFiles = fileList;
                        vsdrModel.listOfComments = teacherComments;
                        return View("VsdrContent", vsdrModel);
                    }
                }
                TempData["Info"] = "El formato del archivo no es valido";
                return View("VsdrContent", vsdrModel);
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return View("VsdrContent", vsdrModel);
            }
        }
        public ActionResult RedirectToUrl(int id)
        {
            VsdrSession vsdrToReturn = ApplicationDbContext.VsdrSessions.Find(id);
            return Redirect(vsdrToReturn.resource_url);
        }
        
        #endregion

        #region VSDR Admin
        #endregion

    }
}