
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
using System.Threading.Tasks;

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
            List<VsdrSession> vsdrList = new List<VsdrSession>();
            List<VsdrSession> tempList = new List<VsdrSession>();
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
                foreach (var extention in allowedExtensions)
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

        public ActionResult CreateVSDR()
        {
            CreateVsdrSession vsdrModel = new CreateVsdrSession();
            //Get list from data base;
            List<VsdrSession> vsdrList = new List<VsdrSession>();
            vsdrList = ApplicationDbContext.VsdrSessions.ToList();

            //To-Do, filter list by enrollment and open
            vsdrModel.listOfVsdr = vsdrList;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateVSDR2(CreateVsdrSession entrada)
        {
            if (ModelState.IsValid)
            {
                var anterior = ApplicationDbContext.VsdrSessions.Where(x => x.name == entrada.actualVsdr1.name).Count();
                if (anterior != 0)
                {
                    throw new Exception("El nombre de VSDR ya esta siendo utilizado ");
                }

                VsdrSession vsdrSession = new VsdrSession
                {
                    name = entrada.actualVsdr1.name,
                    resource_url = entrada.actualVsdr1.resource_url,
                    start_date = entrada.actualVsdr1.start_date,
                    end_date = entrada.actualVsdr1.end_date,
                    case_content = entrada.actualVsdr1.case_content,
                    available = entrada.actualVsdr1.available,
                    open = entrada.actualVsdr1.open,
                };
                ApplicationDbContext.VsdrSessions.Add(vsdrSession);
                ApplicationDbContext.SaveChanges();

            }
            entrada.Sesion = GetActualUserId().SesionUser;
            return RedirectToAction("CreateVSDR", "VSDR");
        }
        public ActionResult MatriculaVSDR(int? page, int id_vsdr)
        {
            int companyId = (int)GetActualUserId().CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && x.ComunidadActiva != null).ToList().ToPagedList(page ?? 1, 15);
            var actualVsdrEnrrollment = ApplicationDbContext.VsdrSessions.Where(x => x.id == id_vsdr).FirstOrDefault();

            CreateVsdrSession model = new CreateVsdrSession
            {
                UserOfCompany = ListOfUser,
                Id_VSDR = id_vsdr,

            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }      
        [Authorize]
        public ActionResult VSDR2(string id, int id_vsdr)
        {
            var user = id; 
            var anterior = ApplicationDbContext.VsdrEnrollments.Where(x => x.user_id == user && x.vsdr_id == id_vsdr).FirstOrDefault();
            if (anterior != null)
            {
                throw new Exception("Ya esta Matriculado");
            }
            var actualVsdrEnrrollment = ApplicationDbContext.VsdrSessions.Where(x => x.id == id_vsdr).FirstOrDefault();


            VsdrEnrollment vsdrEnrollment = new VsdrEnrollment
            {
                user_id = user,
                vsdr_id = id_vsdr,
                vsdr_enro_init_date = actualVsdrEnrrollment.start_date,
                vsdr_enro_finish_date = actualVsdrEnrrollment.end_date,
                qualification = 0,
            };


            ApplicationDbContext.VsdrEnrollments.Add(vsdrEnrollment);
            ApplicationDbContext.SaveChanges();
            int a = 1;
            return RedirectToAction("MatriculaVSDR", new { a, id_vsdr });
        }
        public ActionResult DeleteVSDR(string id, int id_vsdr)
        {
            var user = id;
            var entidad = ApplicationDbContext.VsdrEnrollments.Where(x => x.user_id == user && x.vsdr_id == id_vsdr).FirstOrDefault();
            if (entidad == null)
            {
                throw new Exception("No esta matriculado");
            }         
            ApplicationDbContext.VsdrEnrollments.Remove(entidad);
            ApplicationDbContext.SaveChanges();
            int a = 1;
            return RedirectToAction("MatriculaVSDR", new { a, id_vsdr });
        }
        [Authorize]
        public JsonResult DeleteSession(int id)
        {
            bool result = false;
            var entidad = ApplicationDbContext.VsdrSessions
                .Where(x =>x.id == id)
                .FirstOrDefault();
            var entidadMatriculas = ApplicationDbContext.VsdrEnrollments
                .Where(x => x.vsdr_id == id)
                .ToList();
            if (entidad != null)
            {
                try
                {
                    ApplicationDbContext.VsdrEnrollments.RemoveRange(entidadMatriculas);
                    ApplicationDbContext.VsdrSessions.Remove(entidad);                    
                    ApplicationDbContext.SaveChanges();
                    result = true;

                }
                catch (Exception ex)
                {
                    throw new Exception("El elemento no se puede ser eliminado",ex.InnerException);
                }
            }
            else 
            {
                throw new Exception("No se encuentra VSDR");
            }
           
                    
            return Json(result, JsonRequestBehavior.AllowGet);
        }   
        [Authorize]
        public ActionResult UpdateSession(int id) 
        {
            CreateVsdrSession model = new CreateVsdrSession();
            model.actualVsdr1 = new VsdrSession();
            var entidad = ApplicationDbContext.VsdrSessions.Find(id);

                if (entidad != null)
                {
                    model.actualVsdr1.id = entidad.id;
                    model.actualVsdr1.name = entidad.name;
                    model.actualVsdr1.open = entidad.open;
                    model.actualVsdr1.start_date = entidad.start_date;
                    model.actualVsdr1.end_date = entidad.end_date;
                    model.actualVsdr1.available = entidad.available;
                    model.actualVsdr1.case_content = entidad.case_content;
                model.actualVsdr1.resource_url = entidad.resource_url;
                   model.Sesion = GetActualUserId().SesionUser;
                }
                else
                {
                    throw new Exception("Error al encontrar VSDR");
                }
            return View(model);
        }
        [Authorize]
        public ActionResult UpdateSession2(CreateVsdrSession entrada) 
        {
            var anterior = ApplicationDbContext.VsdrSessions
                  .Where(x => x.id != entrada.actualVsdr1.id && x.name == entrada.actualVsdr1.name)
                  .FirstOrDefault();

            if (anterior != null)
            {
                throw new Exception("La configuracion ya existe");
            }
            var entidad = ApplicationDbContext.VsdrSessions
                .Find(entrada.actualVsdr1.id);

            if (entidad != null)
            {

                entidad.name = entrada.actualVsdr1.name;
                entidad.case_content = entrada.actualVsdr1.case_content;
                entidad.start_date = entrada.actualVsdr1.start_date;
                entidad.end_date = entrada.actualVsdr1.end_date;
                entidad.available = entrada.actualVsdr1.available;
                entidad.open = entrada.actualVsdr1.available;
                entidad.resource_url = entrada.actualVsdr1.resource_url;                
                try
                {
                    entrada.Sesion = GetActualUserId().SesionUser;
                    ApplicationDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("El elemento no se puede modificar", ex.InnerException);
                }

            }
            else
            {
                throw new Exception("Elemento no encontrado");
            }

            return RedirectToAction("CreateVSDR");
        }
        public string IpUser()
        {
            string ip = Request.UserHostAddress;
            string szXForwardedFor = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string Ipserver;
            if (szXForwardedFor != null)
            {
                Ipserver = szXForwardedFor;
            }
            else
            {
                Ipserver = ip;
            }
            return Ipserver;
        }

        public ActionResult SearchUserManager(UserAndMassiveManagementViewModel model, int? page)
        {
            var companyId = GetActualUserId().CompanyId;
            if (string.IsNullOrEmpty(model.User_Id) || string.IsNullOrWhiteSpace(model.User_Id))
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(252);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = null
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();
                  
                }
                return RedirectToAction("MatriculaVSDR", new { page });
            }
            else
            {
                IPagedList<ApplicationUser> ListOfUser;
                var conteo = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && (x.FirstName.Contains(model.User_Id) || x.LastName.Contains(model.User_Id) || x.UserName.Contains(model.User_Id))).ToList();
                if (conteo.Count > 0)
                {
                    ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && (x.FirstName.Contains(model.User_Id) || x.LastName.Contains(model.User_Id) || x.UserName.Contains(model.User_Id))).ToList().ToPagedList(page ?? 1, conteo.Count);
                }
                else
                {
                    ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && (x.FirstName.Contains(model.User_Id) || x.LastName.Contains(model.User_Id) || x.UserName.Contains(model.User_Id))).ToList().ToPagedList(page ?? 1, 6);
                }

                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(252);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = null
                    };
                    ApplicationDbContext.IdChanges.Add(idchange);
                    ApplicationDbContext.SaveChanges();             
                    
                }
                return View("MatriculaVSDR", model);
            }
        }




    }
}