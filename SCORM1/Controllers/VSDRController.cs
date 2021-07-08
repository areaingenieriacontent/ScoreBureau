
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
using SCORM1.Enum;
using SCORM1.Models.Lms;

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
            var actualUser = ApplicationDbContext.Users.Find(GetActualUserId().Id);

            List<VsdrSession> vsdrList = new List<VsdrSession>();
            List<VsdrSession> tempList = new List<VsdrSession>();
            List<VsdrEnrollment> matriculaVSDR = new List<VsdrEnrollment>();
            if (actualUser.Role == ROLES.AdministradoGeneral)
            {
                vsdrList = ApplicationDbContext.VsdrSessions.ToList();
            }
            if (actualUser.Role == ROLES.Usuario)
            {
                matriculaVSDR = ApplicationDbContext.VsdrEnrollments.Where(x => x.user_id == actualUser.Id)
                    .ToList();
                vsdrList = ApplicationDbContext.VsdrSessions.ToList();


                for (int i = 0; i < vsdrList.Count(); i++)
                {
                    if (vsdrList.Count() > 0)
                    {
                        for (int x = 0; x > matriculaVSDR.Count(); x++)
                        {
                            if (vsdrList[i].id == matriculaVSDR[x].vsdr_id )
                            {
                                vsdrList.Add(vsdrList[i]);
                            }
                        }
                    }

                }

            }


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
        public ActionResult VsdrUserListTeacher()
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
            var vsdrEnrollments = ApplicationDbContext.VsdrEnrollments.Where(x => x.vsdr_id == id && x.user_id == actualUser.Id).FirstOrDefault();
            List<VsdrUserFile> vsdrUserIssuedFiles = ApplicationDbContext.VsdrUserFiles.Where(x => x.user_id == actualUser.Id && x.vsdr_id == id).ToList();
            List<VsdrTeacherComment> vsdrTeacherComments = ApplicationDbContext.VsdrTeacherComments.Where(x => x.user_id == actualUser.Id && x.vsdr_id == id).ToList();
            VsdrUserFile file = new VsdrUserFile();
            if (vsdrToReturn.end_date.Subtract(DateTime.Now).TotalMinutes < 15)
            {
                vsdrModel.meetingAvailable = true;
            }
            //add loaded data to view model
            vsdrModel.listOfComments = vsdrTeacherComments;
            vsdrModel.VsdrEnrollment = vsdrEnrollments;
            vsdrModel.listOfIssuedFiles = vsdrUserIssuedFiles;
            vsdrModel.vsdrFileToAdd = file;
            vsdrModel.actualVsdr = vsdrToReturn;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }

        [Authorize]
        public ActionResult VsdrContentTeacher(int id)
        {

            var actualUser = ApplicationDbContext.Users
                .Find(GetActualUserId().Id);
            VsdrUserVM vsdrModel = new VsdrUserVM();
            VsdrSession vsdrToReturn = ApplicationDbContext.VsdrSessions.Find(id);
            List<VsdrUserFile> vsdrUserIssuedFiles = ApplicationDbContext.VsdrUserFiles
                .Where(x => x.vsdr_id == id)
                .ToList();
            var idvsdr = vsdrUserIssuedFiles.Where(m => m.vsdr_id == id)
                .OrderBy(m => m.registered_date)
                .FirstOrDefault();
            List<ApplicationUser> applicationUsers = ApplicationDbContext.Users                
               
                .ToList();
            var DbUsers = ApplicationDbContext.VsdrEnrollments.Where(m => m.vsdr_id == id)
                .ToList();
            List<ApplicationUser> ListUser = new List<ApplicationUser>();

            if (applicationUsers.Count != 0)
            {
                for (int i = 0; i < applicationUsers.Count; i++)
                {
                    for (int x = 0; x < DbUsers.Count ; x++)
                    {
                        if (applicationUsers[i].Id == DbUsers[x].user_id) 
                        {
                            ListUser.Add(applicationUsers[i]);
                        }

                    }
                }
            }
           
            List<VsdrTeacherComment> vsdrTeacherComments = ApplicationDbContext.VsdrTeacherComments
                .Where(x => x.teacher_id == actualUser.Id )
                .ToList();
            VsdrUserFile file = new VsdrUserFile();
            if (vsdrToReturn.end_date.Subtract(DateTime.Now).TotalMinutes < 15)
            {
                vsdrModel.meetingAvailable = true;
            }
            //add loaded data to view model

            vsdrModel.ListEnrollment = DbUsers;
            vsdrModel.listOfUsers = ListUser;
            vsdrModel.listOfIssuedFiles = vsdrUserIssuedFiles;
            vsdrModel.listOfComments = vsdrTeacherComments;
            vsdrModel.teacherName = actualUser.FirstName;
            vsdrModel.teacherLastName = actualUser.LastName;
            vsdrModel.vsdrFileToAdd = file;
            vsdrModel.actualVsdr = vsdrToReturn;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }
        public string numero { get; set; }
        [Authorize]
        public JsonResult ComentTeacher1(string id)
        {
            bool result = false;
            if (id != null) 
            {
                numero = id;
                VsdrUserVM vsdrModel = new VsdrUserVM();
                vsdrModel.userId = numero;
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        [Authorize]
        public JsonResult ComentTeacher(string id, int? id2, string dato) 
        {

            bool result = false;
            var actualUser = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            VsdrUserVM vsdrModel = new VsdrUserVM();
            
            if (id != null)
            {
                VsdrTeacherComment vsdrTeacherComment = new VsdrTeacherComment
                {
                    user_id = id,
                    vsdr_id = (int)id2,
                    teacher_id = actualUser.Id,
                    commentDate = DateTime.Now,
                    content = dato,
                    
                };
                result = true;
                ApplicationDbContext.VsdrTeacherComments.Add(vsdrTeacherComment);
                ApplicationDbContext.SaveChanges();
                
            }
            else 
            {
                throw new Exception("Error al crear el comentario");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
      
        [Route("EvaluarTeacher/{id:string}/{id2:string}/{dato:float}")]
        [Authorize]
        public JsonResult EvaluarTeacher(string id, int? id2, float? dato) 
        {
            VsdrUserVM model = new VsdrUserVM();
            model.VsdrEnrollment = new VsdrEnrollment();
            var entidad = ApplicationDbContext.VsdrEnrollments.Where(x=> x.user_id == id && x.vsdr_id == id2).FirstOrDefault();

            bool result = false;
            if (entidad != null)
            {
                entidad.qualification = (float)dato;
                entidad.vsdr_enro_init_date = entidad.vsdr_enro_init_date;
                entidad.vsdr_enro_finish_date = entidad.vsdr_enro_finish_date;               
                model.Sesion = GetActualUserId().SesionUser;
                ApplicationDbContext.SaveChanges();
                result = true;
            }
            else 
            {
                throw new Exception("Error al encontrar VSDR");
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
                        List<VsdrUserFile> fileList = ApplicationDbContext.VsdrUserFiles.Where(x => x.user_id == actualUser.Id && x.vsdr_id == vsdrModel.actualVsdr.id).ToList();
                        List<VsdrTeacherComment> teacherComments = ApplicationDbContext.VsdrTeacherComments.Where(x => x.user_id == actualUser.Id  && x.vsdr_id == vsdrModel.actualVsdr.id).ToList();
                        var vsdrEnrollments = ApplicationDbContext.VsdrEnrollments.Where(x => x.vsdr_id == vsdrModel.actualVsdr.id && x.user_id == actualUser.Id).FirstOrDefault();
                        vsdrModel.VsdrEnrollment = vsdrEnrollments;
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
            var actualVsdrEnrrollment = ApplicationDbContext.VsdrEnrollments.Where(x => x.vsdr_id == id_vsdr).ToList();
            

            CreateVsdrSession model = new CreateVsdrSession
            {
                listEnrollment = actualVsdrEnrrollment,
                UserOfCompany = ListOfUser,
                Id_VSDR = id_vsdr,
               
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }      
        [Authorize]
        public ActionResult VSDR2(string id, int id_vsdr, int ? page)
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
            return RedirectToAction("MatriculaVSDR", new { page, id_vsdr });
        }
        public ActionResult DeleteVSDR(string id, int id_vsdr)
        {
            var user = id;
            var entidad = ApplicationDbContext.VsdrEnrollments.Where(x => x.user_id == user && x.vsdr_id == id_vsdr).FirstOrDefault();
            try
            {
                if (entidad == null)
                {
                    throw new Exception("No esta matriculado");
                }
                ApplicationDbContext.VsdrEnrollments.Remove(entidad);
                ApplicationDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("El elemento no se puede ser eliminado", ex.InnerException);
            }
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

        public ActionResult SearchUserManager(CreateVsdrSession model, int? page)
        {
            var companyId = GetActualUserId().CompanyId;
            if (string.IsNullOrEmpty(model.User) || string.IsNullOrWhiteSpace(model.User))
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(252);
                var idcompany = UserCurrent.CompanyId;
                IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && x.ComunidadActiva != null).ToList().ToPagedList(page ?? 1, 15);
                var actualVsdrEnrrollment = ApplicationDbContext.VsdrEnrollments.Where(x => x.vsdr_id == model.Id_VSDR).ToList();
                model.listEnrollment = actualVsdrEnrrollment;
                model.UserOfCompany = ListOfUser;
                model.Id_VSDR = model.Id_VSDR;
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
                
                int a = 1;
                return RedirectToAction("MatriculaVSDR", new { a, model.Id_VSDR });
            }
            else
            {
                IPagedList<ApplicationUser> ListOfUser;
                var conteo = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && (x.FirstName.Contains(model.User) || x.LastName.Contains(model.User) || x.UserName.Contains(model.User))).ToList();
                if (conteo.Count > 0)
                {
                    ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && (x.FirstName.Contains(model.User) || x.LastName.Contains(model.User) || x.UserName.Contains(model.User))).ToList().ToPagedList(page ?? 1, conteo.Count);
                }
                else
                {
                    ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && (x.FirstName.Contains(model.User) || x.LastName.Contains(model.User) || x.UserName.Contains(model.User))).ToList().ToPagedList(page ?? 1, 6);
                }
                model = new CreateVsdrSession
                {
                    UserOfCompany = ListOfUser,
                    Id_VSDR = model.Id_VSDR,
                };

                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(252);
                var idcompany = UserCurrent.CompanyId;                
                var actualVsdrEnrrollment = ApplicationDbContext.VsdrEnrollments.Where(x => x.vsdr_id == model.Id_VSDR).ToList();
                model.listEnrollment = actualVsdrEnrrollment;
                model.UserOfCompany = ListOfUser;
                model.Id_VSDR = model.Id_VSDR;
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

        [Authorize]
        public ActionResult EnrollmentMasiveVSDR(int id_vsdr) 
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            VsdrSession getSession = ApplicationDbContext.VsdrSessions.Find(id_vsdr);
            var Users = ApplicationDbContext.Users.Where(x => x.CompanyId == GetModuleCompany && x.Role == ROLES.Usuario).ToList();
            List<AllUserVSDR> Listalluser = new List<AllUserVSDR>();
            foreach (var item in Users)
            {
                Listalluser.Add(new AllUserVSDR
                {
                    User_Id = item.Id,
                    VSDR_Id = id_vsdr                    
                });
            }
            List<AreasVSDR> ListAreas = new List<AreasVSDR>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new AreasVSDR
            {
                VSDR_Id = id_vsdr,
                Listareas = areas
            });
            List<PositionsVSDR> ListPositions = new List<PositionsVSDR>();
            var positions = ApplicationDbContext.Position.ToList();

            ListPositions.Add(new PositionsVSDR
            {
                VSDR_Id = id_vsdr,
                Listpositions = positions
            });
            List<CitiesVSDR> ListCities = new List<CitiesVSDR>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new CitiesVSDR
            {
                VSDR_Id = id_vsdr,
                Listcities = city
            });
            List<LocationsVSDR> ListLocations = new List<LocationsVSDR>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new LocationsVSDR
            {
                VSDR_Id = id_vsdr,
                Listlocations = location
            });
            CreateVsdrSession model = new CreateVsdrSession
            {
                ActualRole = GetActualUserId().Role,
                actualVsdr1 = getSession,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(34);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(219);
            var idcompany = UserCurrent.CompanyId;
           
            return View("EnrollmentMasiveVSDR", model);
        }

        [Authorize]
        public ActionResult AllUsers()
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var Users = ApplicationDbContext.Users.Where(x => x.CompanyId == GetModuleCompany && x.Role == ROLES.Usuario).ToList();
            CreateVsdrSession model = new CreateVsdrSession
            {
                ActualRole = GetActualUserId().Role,
                listUser123 = Users
            };
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_AllUser", model);
        }

        
        //public ActionResult EnrollmentAllUser(CreateVsdrSession model)
        //{
        //    var module = ApplicationDbContext.VsdrEnrollments.Find(model.Id_VSDR);
        //    foreach (var item in model.ListAllUser)
        //    {
        //        var user = ApplicationDbContext.Users.Find(item.User_Id);
        //        var getEnrollment = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
        //        if (getEnrollment.Count == 0)
        //        {
        //            DateTime finish = new DateTime();
        //            switch (module.Modu_Period)
        //            {
        //                case VIGENCIA.Dias:
        //                    finish = DateTime.Now.AddDays(module.Modu_Validity);
        //                    break;
        //                case VIGENCIA.Meses:
        //                    finish = DateTime.Now.AddMonths(module.Modu_Validity);
        //                    break;
        //                case VIGENCIA.Años:
        //                    finish = DateTime.Now.AddYears(module.Modu_Validity);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            DateTime b = finish;
        //            TempData["Info"] = "Matricula Registrada";
        //            Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = user, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };

        //            // QuienSabeMas
        //            if (module.QSMActive == 1)
        //            {
        //                QuienSabeMasPuntaje quienSabeMasPuntaje = new QuienSabeMasPuntaje()
        //                {
        //                    User_Id = GetActualUserId().Id,
        //                    User_Id_QSM = GetActualUserId().Document,
        //                    Mudole_Id = module.Modu_Id,
        //                    FechaPresentacion = DateTime.Now,
        //                    Puntaje = 0,
        //                    PorcentajeAprobacion = 0
        //                };
        //                ApplicationDbContext.QuienSabeMasPuntajes.Add(quienSabeMasPuntaje);
        //            }

        //            ApplicationDbContext.VsdrEnrollments.Add(model);
        //            ApplicationDbContext.SaveChanges();
        //            var table = ApplicationDbContext.TableChanges.Find(22);
        //            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //            var code = ApplicationDbContext.CodeLogs.Find(221);
        //            var idcompany = UserCurrent.CompanyId;
        //            if (idcompany != null)
        //            {
        //                var company = ApplicationDbContext.Companies.Find(idcompany);
        //                string ip = IpUser();
        //                var idchange = new IdChange
        //                {
        //                    IdCh_IdChange = enrollment.Enro_Id.ToString()
        //                };
        //                ApplicationDbContext.IdChanges.Add(idchange);
        //                ApplicationDbContext.SaveChanges();
        //                Log logsesiontrue = new Log
        //                {
        //                    ApplicationUser = UserCurrent,
        //                    CoLo_Id = code.CoLo_Id,
        //                    CodeLogs = code,
        //                    Log_Date = DateTime.Now,
        //                    Log_StateLogs = LOGSTATE.Realizado,
        //                    TableChange = table,
        //                    TaCh_Id = table.TaCh_Id,
        //                    IdChange = idchange,
        //                    IdCh_Id = idchange.IdCh_Id,
        //                    User_Id = UserCurrent.Id,
        //                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de matricular el curso con id " + enrollment.Modu_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
        //                    Company = company,
        //                    Company_Id = company.CompanyId,
        //                    Log_Ip = ip
        //                };
        //                ApplicationDbContext.Logs.Add(logsesiontrue);
        //                ApplicationDbContext.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
        //            var table = ApplicationDbContext.TableChanges.Find(22);
        //            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //            var code = ApplicationDbContext.CodeLogs.Find(221);
        //            var idcompany = UserCurrent.CompanyId;
        //            if (idcompany != null)
        //            {
        //                var company = ApplicationDbContext.Companies.Find(idcompany);
        //                string ip = IpUser();
        //                var idchange = new IdChange
        //                {
        //                    IdCh_IdChange = null
        //                };
        //                ApplicationDbContext.IdChanges.Add(idchange);
        //                ApplicationDbContext.SaveChanges();
        //                Log logsesiontrue = new Log
        //                {
        //                    ApplicationUser = UserCurrent,
        //                    CoLo_Id = code.CoLo_Id,
        //                    CodeLogs = code,
        //                    Log_Date = DateTime.Now,
        //                    Log_StateLogs = LOGSTATE.Realizado,
        //                    TableChange = table,
        //                    TaCh_Id = table.TaCh_Id,
        //                    IdChange = idchange,
        //                    IdCh_Id = idchange.IdCh_Id,
        //                    User_Id = UserCurrent.Id,
        //                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular el curso con id " + item.Modu_Id + " al usuario con id " + item.User_Id + "pero este usuario ya tiene este curso matriculado, en la compañía con id" + company.CompanyId,
        //                    Company = company,
        //                    Company_Id = company.CompanyId,
        //                    Log_Ip = ip
        //                };
        //                ApplicationDbContext.Logs.Add(logsesiontrue);
        //                ApplicationDbContext.SaveChanges();
        //            }
        //        }
        //    }
        //    return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        //}

        //[Authorize]
        //public ActionResult CancelEnrollmentAllUser(CreateVsdrSession Enrollments)
        //{
        //    var GetModuleCompany = GetActualUserId().CompanyId;
        //    var A = Enrollments.ListAllUser.Take(1);
        //    int c = A.Select(x => x.VSDR_Id).Single();
        //    List<user> ListUserCancel = new List<user>();
        //    List<user1> ListUserCancel1 = new List<user1>();
        //    List<user2> ListUserCancel2 = new List<user2>();
        //    VsdrSession getModule = ApplicationDbContext.VsdrSessions.Find(c);
        //    var Users = ApplicationDbContext.Users.Where(x => x.CompanyId == GetModuleCompany && x.Role == ROLES.Usuario).ToList();
        //    List<AllUserVSDR> Listalluser = new List<AllUserVSDR>();
        //    foreach (var item in Users)
        //    {
        //        Listalluser.Add(new AllUserVSDR
        //        {
        //            User_Id = item.Id,
        //            VSDR_Id = c
        //        });
        //    }
        //    foreach (var item in Enrollments.ListAllUser)
        //    {
        //        int VSDR_Id = item.VSDR_Id;
        //        string User_Id = item.User_Id;
        //        var user = ApplicationDbContext.Users.Find(User_Id);
        //        var module = ApplicationDbContext.Modules.Find(VSDR_Id);

        //        var getVSDR = user.Enrollment.Where(x => x.ApplicationUser.Id == user.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
        //        if (getVSDR != null)
        //        {
        //            var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == user.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
        //            var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
        //            if (advanceuser.Count != 0 || attempts.Count != 0)
        //            {
        //                ListUserCancel.Add(new user
        //                {
        //                    UserName = user.UserName,
        //                    Name = user.FirstName + user.LastName,
        //                    Email = user.Email
        //                });
        //                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
        //                var table = ApplicationDbContext.TableChanges.Find(22);
        //                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //                var code = ApplicationDbContext.CodeLogs.Find(226);
        //                var idcompany = UserCurrent.CompanyId;
        //                if (idcompany != null)
        //                {
        //                    var company = ApplicationDbContext.Companies.Find(idcompany);
        //                    string ip = IpUser();
        //                    var idchange = new IdChange
        //                    {
        //                        IdCh_IdChange = getVSDR.Enro_Id.ToString()
        //                    };
        //                    ApplicationDbContext.IdChanges.Add(idchange);
        //                    ApplicationDbContext.SaveChanges();
        //                    Log logsesiontrue = new Log
        //                    {
        //                        ApplicationUser = UserCurrent,
        //                        CoLo_Id = code.CoLo_Id,
        //                        CodeLogs = code,
        //                        Log_Date = DateTime.Now,
        //                        Log_StateLogs = LOGSTATE.Realizado,
        //                        TableChange = table,
        //                        TaCh_Id = table.TaCh_Id,
        //                        IdChange = idchange,
        //                        IdCh_Id = idchange.IdCh_Id,
        //                        User_Id = UserCurrent.Id,
        //                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + " pero este usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
        //                        Company = company,
        //                        Company_Id = company.CompanyId,
        //                        Log_Ip = ip
        //                    };
        //                    ApplicationDbContext.Logs.Add(logsesiontrue);
        //                    ApplicationDbContext.SaveChanges();
        //                }
        //            }
        //            else
        //            {
        //                ListUserCancel1.Add(new user1
        //                {
        //                    UserName = user.UserName,
        //                    Name = user.FirstName + user.LastName,
        //                    Email = user.Email
        //                });
        //                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
        //                TempData["Info1"] = "Usuarios desmatriculados.";
        //                var table = ApplicationDbContext.TableChanges.Find(22);
        //                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
        //                var code = ApplicationDbContext.CodeLogs.Find(226);
        //                var idcompany = UserCurrent.CompanyId;
        //                if (idcompany != null)
        //                {
        //                    var company = ApplicationDbContext.Companies.Find(idcompany);
        //                    string ip = IpUser();
        //                    var idchange = new IdChange
        //                    {
        //                        IdCh_IdChange = getVSDR.Enro_Id.ToString()
        //                    };
        //                    ApplicationDbContext.IdChanges.Add(idchange);
        //                    ApplicationDbContext.SaveChanges();
        //                    Log logsesiontrue = new Log
        //                    {
        //                        ApplicationUser = UserCurrent,
        //                        CoLo_Id = code.CoLo_Id,
        //                        CodeLogs = code,
        //                        Log_Date = DateTime.Now,
        //                        Log_StateLogs = LOGSTATE.Realizado,
        //                        TableChange = table,
        //                        TaCh_Id = table.TaCh_Id,
        //                        IdChange = idchange,
        //                        IdCh_Id = idchange.IdCh_Id,
        //                        User_Id = UserCurrent.Id,
        //                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
        //                        Company = company,
        //                        Company_Id = company.CompanyId,
        //                        Log_Ip = ip
        //                    };
        //                    ApplicationDbContext.Logs.Add(logsesiontrue);
        //                    ApplicationDbContext.SaveChanges();
        //                }
        //                ApplicationDbContext.Enrollments.Remove(getVSDR);
        //                ApplicationDbContext.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            ListUserCancel2.Add(new user2
        //            {
        //                UserName = user.UserName,
        //                Name = user.FirstName + user.LastName,
        //                Email = user.Email
        //            });
        //            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
        //            TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
        //            TempData["Info3"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
        //        }
        //    }
        //    List<Areas> ListAreas = new List<Areas>();
        //    var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

        //    ListAreas.Add(new Areas
        //    {
        //        Modu_Id = c,
        //        Listareas = areas
        //    });
        //    List<Positions> ListPositions = new List<Positions>();
        //    var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

        //    ListPositions.Add(new Positions
        //    {
        //        Modu_Id = c,
        //        Listpositions = positions
        //    });
        //    List<Cities> ListCities = new List<Cities>();
        //    var city = ApplicationDbContext.City.ToList();
        //    ListCities.Add(new Cities
        //    {
        //        Modu_Id = c,
        //        Listcities = city
        //    });
        //    List<Locations> ListLocations = new List<Locations>();
        //    var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
        //    ListLocations.Add(new Locations
        //    {
        //        Modu_Id = c,
        //        Listlocations = location
        //    });
        //    AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
        //    {
        //        ActualRole = GetActualUserId().Role,
        //        Module = getModule,
        //        ListAllUser = Listalluser,
        //        ListAreas = ListAreas,
        //        ListPositions = ListPositions,
        //        ListCitices = ListCities,
        //        ListLocations = ListLocations,
        //        ListUserCancel = ListUserCancel,
        //        ListUserCancel1 = ListUserCancel1,
        //        ListUserCancel2 = ListUserCancel2
        //    };
        //    model.Logo = GetUrlLogo();
        //    model.Sesion = GetActualUserId().SesionUser;
        //    return View("CancelEnrollments", model);
        //}


        [Authorize]
        public ActionResult EnrollmentAreas(CreateVsdrSession entrada)
        {

            var vsdrsession = ApplicationDbContext.VsdrSessions.Find(entrada.Id_VSDR);
            var areas = entrada.ListAreas.Take(1);
            var idarea = areas.Select(x => x.Area_Id).Single();
            var user = ApplicationDbContext.Users.Where(x => x.AreaId == idarea && x.Role == ROLES.Usuario).ToList();
            if (user.Count() != 0)
            {
                foreach (var item in user)
                {
                    var useractual = ApplicationDbContext.Users.Find(item.Id);

                    VsdrEnrollment vsdrEnrollment = new VsdrEnrollment
                    {
                        user_id = useractual.Id,
                        vsdr_id = vsdrsession.id,
                        vsdr_enro_init_date = vsdrsession.start_date,
                        vsdr_enro_finish_date = vsdrsession.start_date,
                        qualification = 0,
                    };
                    ApplicationDbContext.VsdrEnrollments.Add(vsdrEnrollment);
                    ApplicationDbContext.SaveChanges();
                }
            }
            else 
            {
                TempData["Info"] = "No hay usuarios asociados con esta area";
            };

            return View();
            //var vsdrsession = ApplicationDbContext.VsdrSessions.Find(Enrollments.Id_VSDR);
            //var A = Enrollments.ListAreas.Take(1);
            //int d = A.Select(x => x.Area_Id).Single();
            //var user = ApplicationDbContext.Users.Where(x => x.AreaId == d && x.Role == ROLES.Usuario).ToList();
            //if (user.Count != 0)
            //{
            //    foreach (var item in user)
            //    {
            //        var useractual = ApplicationDbContext.Users.Find(item.Id);
            //        var getEnrollment = ApplicationDbContext.VsdrEnrollments.Where(x => x.user_id == useractual.Id && x.vsdr_id == vsdrsession.id).ToList();
            //        if (getEnrollment.Count == 0)
            //        {
            //            DateTime finish = new DateTime();                        
            //            DateTime b = finish;
            //            TempData["Info"] = "Matricula Registrada";
            //            VsdrEnrollment enrollment = new VsdrEnrollment 
            //            {
            //                vsdr_id = vsdrsession.id, 
            //                User = useractual,
            //                vsdr_enro_init_date = DateTime.Now, 
            //                vsdr_enro_finish_date = b,
            //                qualification = 0 }
            //            ;
            //            ApplicationDbContext.VsdrEnrollments.Add(enrollment);
            //            ApplicationDbContext.SaveChanges();
            //            var table = ApplicationDbContext.TableChanges.Find(22);
            //            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            //            var code = ApplicationDbContext.CodeLogs.Find(222);
            //            var idcompany = UserCurrent.CompanyId;
            //            if (idcompany != null)
            //            {
            //                var company = ApplicationDbContext.Companies.Find(idcompany);
            //                string ip = IpUser();
                         
            //            }
            //        }
            //        else
            //        {
            //            TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
            //            var table = ApplicationDbContext.TableChanges.Find(22);
            //            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            //            var code = ApplicationDbContext.CodeLogs.Find(222);
            //            var idcompany = UserCurrent.CompanyId;
            //            if (idcompany != null)
            //            {
            //                var company = ApplicationDbContext.Companies.Find(idcompany);
            //                string ip = IpUser();
            //                var idchange = new IdChange
            //                {
            //                    IdCh_IdChange = null
            //                };
            //                ApplicationDbContext.IdChanges.Add(idchange);
            //                ApplicationDbContext.SaveChanges();
            //                Log logsesiontrue = new Log
            //                {
            //                    ApplicationUser = UserCurrent,
            //                    CoLo_Id = code.CoLo_Id,
            //                    CodeLogs = code,
            //                    Log_Date = DateTime.Now,
            //                    Log_StateLogs = LOGSTATE.Realizado,
            //                    TableChange = table,
            //                    TaCh_Id = table.TaCh_Id,
            //                    IdChange = idchange,
            //                    IdCh_Id = idchange.IdCh_Id,
            //                    User_Id = UserCurrent.Id,
            //                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular por área el curso con id " + vsdrsession.vsdr_id + " al usuario con id " + item.Id + "pero este usuario ya tienen matriculado este curso, en la compañía con id" + company.CompanyId,
            //                    Company = company,
            //                    Company_Id = company.CompanyId,
            //                    Log_Ip = ip
            //                };
            //                ApplicationDbContext.Logs.Add(logsesiontrue);
            //                ApplicationDbContext.SaveChanges();
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    TempData["Info"] = "No hay usuarios asociados con esta area";
            //}
            //return RedirectToAction("EnrollmentMasiveVSDR", new { id_vsdr = vsdrsession.vsdr_id });
        }

        [Authorize]
        public ActionResult CancelEnrollmentAreas(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListAreas.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            int d = A.Select(x => x.Area_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            var user = ApplicationDbContext.Users.Where(x => x.AreaId == d && x.Role == ROLES.Usuario).ToList();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in user)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    int Modu_Id = c;
                    string User_Id = item.Id;
                    var useractual = ApplicationDbContext.Users.Find(User_Id);
                    var module = ApplicationDbContext.Modules.Find(Modu_Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                    if (getEnrollment != null)
                    {
                        var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == useractual.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                        var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                        if (advanceuser.Count != 0 || attempts.Count != 0)
                        {
                            ListUserCancel.Add(new user
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(227);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular por área el curso con id " + Modu_Id + " al usuario con id " + item.Id + " pero el usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            ListUserCancel1.Add(new user1
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            TempData["Info1"] = "Usuarios desmatriculados.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(227);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular por área el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            ApplicationDbContext.Enrollments.Remove(getEnrollment);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ListUserCancel2.Add(new user2
                        {
                            UserName = useractual.UserName,
                            Name = useractual.FirstName + useractual.LastName,
                            Email = useractual.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    }
                }
            }
            else
            {
                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                TempData["Info3"] = "No hay usuarios asociados con esta area";
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }


        [Authorize]
        public ActionResult EnrollmentPositions(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);
            var A = Enrollments.ListPositions.Take(1);
            int d = A.Select(x => x.Posi_Id).Single();
            var user = ApplicationDbContext.Users.Where(x => x.PositionId == d && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var useractual = ApplicationDbContext.Users.Find(item.Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
                    if (getEnrollment.Count == 0)
                    {
                        DateTime finish = new DateTime();
                        switch (module.Modu_Period)
                        {
                            case VIGENCIA.Dias:
                                finish = DateTime.Now.AddDays(module.Modu_Validity);
                                break;
                            case VIGENCIA.Meses:
                                finish = DateTime.Now.AddMonths(module.Modu_Validity);
                                break;
                            case VIGENCIA.Años:
                                finish = DateTime.Now.AddYears(module.Modu_Validity);
                                break;
                            default:
                                break;
                        }
                        DateTime b = finish;
                        TempData["Info"] = "Matricula Registrada";
                        Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = useractual, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
                        ApplicationDbContext.Enrollments.Add(enrollment);
                        ApplicationDbContext.SaveChanges();
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(223);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = enrollment.Enro_Id.ToString()
                            };
                            ApplicationDbContext.IdChanges.Add(idchange);
                            ApplicationDbContext.SaveChanges();
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = UserCurrent,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = UserCurrent.Id,
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de matricular por cargo el curso con id " + enrollment.Enro_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(223);
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
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = UserCurrent,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = UserCurrent.Id,
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular por cargo el curso con id " + module.Modu_Id + " al usuario con id " + item.Id + "pero este usuario ya tienen matriculado este curso, en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                TempData["Info"] = "No hay usuarios asociados con este cargo";
            }
            return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        }

        [Authorize]
        public ActionResult CancelEnrollmentPositions(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListPositions.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            int d = A.Select(x => x.Posi_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            var user = ApplicationDbContext.Users.Where(x => x.PositionId == d && x.Role == ROLES.Usuario).ToList();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in user)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    int Modu_Id = c;
                    string User_Id = item.Id;
                    var useractual = ApplicationDbContext.Users.Find(User_Id);
                    var module = ApplicationDbContext.Modules.Find(Modu_Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                    if (getEnrollment != null)
                    {
                        var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == useractual.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                        var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                        if (advanceuser.Count != 0 || attempts.Count != 0)
                        {
                            ListUserCancel.Add(new user
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(228);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular por cargo el curso con id " + Modu_Id + " al usuario con id " + item.Id + " pero el usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            ListUserCancel1.Add(new user1
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            TempData["Info1"] = "Usuarios desmatriculados.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(228);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular por cargo el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            ApplicationDbContext.Enrollments.Remove(getEnrollment);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ListUserCancel2.Add(new user2
                        {
                            UserName = useractual.UserName,
                            Name = useractual.FirstName + useractual.LastName,
                            Email = useractual.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    }
                }
            }
            else
            {
                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                TempData["Info3"] = "No hay usuarios asociados con este cargo";
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }

        [Authorize]
        public ActionResult EnrollmentCities(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListCitices.Take(1);
            int d = A.Select(x => x.City_Id).Single();
            var user = ApplicationDbContext.Users.Where(x => x.CityId == d && x.Role == ROLES.Usuario && x.CompanyId == GetModuleCompany).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var useractual = ApplicationDbContext.Users.Find(item.Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
                    if (getEnrollment.Count == 0)
                    {
                        DateTime finish = new DateTime();
                        switch (module.Modu_Period)
                        {
                            case VIGENCIA.Dias:
                                finish = DateTime.Now.AddDays(module.Modu_Validity);
                                break;
                            case VIGENCIA.Meses:
                                finish = DateTime.Now.AddMonths(module.Modu_Validity);
                                break;
                            case VIGENCIA.Años:
                                finish = DateTime.Now.AddYears(module.Modu_Validity);
                                break;
                            default:
                                break;
                        }
                        DateTime b = finish;
                        TempData["Info"] = "Matricula Registrada";
                        Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = useractual, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
                        ApplicationDbContext.Enrollments.Add(enrollment);
                        ApplicationDbContext.SaveChanges();
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(224);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = enrollment.Enro_Id.ToString()
                            };
                            ApplicationDbContext.IdChanges.Add(idchange);
                            ApplicationDbContext.SaveChanges();
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = UserCurrent,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = UserCurrent.Id,
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de matricular por ciudad el curso con id " + enrollment.Enro_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(224);
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
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = UserCurrent,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = UserCurrent.Id,
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular por ciudad el curso con id " + module.Modu_Id + " al usuario con id " + item.Id + "pero este usuario ya tienen matriculado este curso, en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                TempData["Info"] = "No hay usuarios asociados con esta ciudad";
            }
            return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        }

        [Authorize]
        public ActionResult CancelEnrollmentCities(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListCitices.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            int d = A.Select(x => x.City_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            var user = ApplicationDbContext.Users.Where(x => x.CityId == d && x.Role == ROLES.Usuario && x.Company.CompanyId == GetModuleCompany).ToList();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in user)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    int Modu_Id = c;
                    string User_Id = item.Id;
                    var useractual = ApplicationDbContext.Users.Find(User_Id);
                    var module = ApplicationDbContext.Modules.Find(Modu_Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                    if (getEnrollment != null)
                    {
                        var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == useractual.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                        var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                        if (advanceuser.Count != 0 || attempts.Count != 0)
                        {
                            ListUserCancel.Add(new user
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(229);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular por ciudad el curso con id " + Modu_Id + " al usuario con id " + item.Id + " pero el usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            ListUserCancel1.Add(new user1
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            TempData["Info1"] = "Usuarios desmatriculados.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(229);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular por ciudad el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            ApplicationDbContext.Enrollments.Remove(getEnrollment);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ListUserCancel2.Add(new user2
                        {
                            UserName = useractual.UserName,
                            Name = useractual.FirstName + useractual.LastName,
                            Email = useractual.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    }
                }
            }
            else
            {
                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                TempData["Info3"] = "No hay usuarios asociados con este cargo";
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }

        [Authorize]
        public ActionResult EnrollmentLocations(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var module = ApplicationDbContext.Modules.Find(Enrollments.Module.Modu_Id);
            var A = Enrollments.ListLocations.Take(1);
            int d = A.Select(x => x.Loca_Id).Single();
            var user = ApplicationDbContext.Users.Where(x => x.LocationId == d && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var useractual = ApplicationDbContext.Users.Find(item.Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).ToList();
                    if (getEnrollment.Count == 0)
                    {
                        DateTime finish = new DateTime();
                        switch (module.Modu_Period)
                        {
                            case VIGENCIA.Dias:
                                finish = DateTime.Now.AddDays(module.Modu_Validity);
                                break;
                            case VIGENCIA.Meses:
                                finish = DateTime.Now.AddMonths(module.Modu_Validity);
                                break;
                            case VIGENCIA.Años:
                                finish = DateTime.Now.AddYears(module.Modu_Validity);
                                break;
                            default:
                                break;
                        }
                        DateTime b = finish;
                        TempData["Info"] = "Matricula Registrada";
                        Enrollment enrollment = new Enrollment { Module = module, ApplicationUser = useractual, Company = GetActualUserId().Company, Enro_InitDateModule = DateTime.Now, Enro_FinishDateModule = b, Enro_StateEnrollment = ENROLLMENTSTATE.Activo };
                        ApplicationDbContext.Enrollments.Add(enrollment);
                        ApplicationDbContext.SaveChanges();
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(225);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = enrollment.Enro_Id.ToString()
                            };
                            ApplicationDbContext.IdChanges.Add(idchange);
                            ApplicationDbContext.SaveChanges();
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = UserCurrent,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = UserCurrent.Id,
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de matricular por ubicación el curso con id " + enrollment.Enro_Id + " al usuario con id " + enrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        TempData["Info"] = "Algunos usuarios ya tiene matriculada esta materia";
                        var table = ApplicationDbContext.TableChanges.Find(22);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(225);
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
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = UserCurrent,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = UserCurrent.Id,
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento matricular por ubicación el curso con id " + module.Modu_Id + " al usuario con id " + item.Id + "pero este usuario ya tienen matriculado este curso, en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                TempData["Info"] = "No hay usuarios asociados con esta ubicación";
            }
            return RedirectToAction("Enrollments", new { Modu_Id = module.Modu_Id });
        }


        [Authorize]
        public ActionResult CancelEnrollmentLocations(AdminTrainingMassiveEnrollmentViewMode Enrollments)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            var A = Enrollments.ListLocations.Take(1);
            int c = A.Select(x => x.Modu_Id).Single();
            int d = A.Select(x => x.Loca_Id).Single();
            List<user> ListUserCancel = new List<user>();
            List<user1> ListUserCancel1 = new List<user1>();
            List<user2> ListUserCancel2 = new List<user2>();
            var user = ApplicationDbContext.Users.Where(x => x.LocationId == d && x.Role == ROLES.Usuario).ToList();
            Module getModule = ApplicationDbContext.Modules.Find(c);
            List<AllUser> Listalluser = new List<AllUser>();
            foreach (var item in user)
            {
                Listalluser.Add(new AllUser
                {
                    User_Id = item.Id,
                    Modu_Id = c
                });
            }
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    int Modu_Id = c;
                    string User_Id = item.Id;
                    var useractual = ApplicationDbContext.Users.Find(User_Id);
                    var module = ApplicationDbContext.Modules.Find(Modu_Id);
                    var getEnrollment = useractual.Enrollment.Where(x => x.ApplicationUser.Id == useractual.Id && x.Module.Modu_Id == module.Modu_Id).FirstOrDefault();
                    if (getEnrollment != null)
                    {
                        var advanceuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == useractual.Id && x.TopicsCourse.Module.Modu_Id == getEnrollment.Module.Modu_Id).ToList();
                        var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == User_Id && x.BankQuestion.TopicsCourse.Modu_Id == getEnrollment.Modu_Id).ToList();
                        if (advanceuser.Count != 0 || attempts.Count != 0)
                        {
                            ListUserCancel.Add(new user
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(230);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento des matricular por ubicación el curso con id " + Modu_Id + " al usuario con id " + item.Id + " pero el usuario ya tiene avances en este curso, en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            ListUserCancel1.Add(new user1
                            {
                                UserName = useractual.UserName,
                                Name = useractual.FirstName + useractual.LastName,
                                Email = useractual.Email
                            });
                            TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                            TempData["Info1"] = "Usuarios desmatriculados.";
                            var table = ApplicationDbContext.TableChanges.Find(22);
                            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                            var code = ApplicationDbContext.CodeLogs.Find(230);
                            var idcompany = UserCurrent.CompanyId;
                            if (idcompany != null)
                            {
                                var company = ApplicationDbContext.Companies.Find(idcompany);
                                string ip = IpUser();
                                var idchange = new IdChange
                                {
                                    IdCh_IdChange = getEnrollment.Enro_Id.ToString()
                                };
                                ApplicationDbContext.IdChanges.Add(idchange);
                                ApplicationDbContext.SaveChanges();
                                Log logsesiontrue = new Log
                                {
                                    ApplicationUser = UserCurrent,
                                    CoLo_Id = code.CoLo_Id,
                                    CodeLogs = code,
                                    Log_Date = DateTime.Now,
                                    Log_StateLogs = LOGSTATE.Realizado,
                                    TableChange = table,
                                    TaCh_Id = table.TaCh_Id,
                                    IdChange = idchange,
                                    IdCh_Id = idchange.IdCh_Id,
                                    User_Id = UserCurrent.Id,
                                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de des matricular por ubicación el curso con id " + getEnrollment.Enro_Id + " al usuario con id " + getEnrollment.User_Id + ", en la compañía con id" + company.CompanyId,
                                    Company = company,
                                    Company_Id = company.CompanyId,
                                    Log_Ip = ip
                                };
                                ApplicationDbContext.Logs.Add(logsesiontrue);
                                ApplicationDbContext.SaveChanges();
                            }
                            ApplicationDbContext.Enrollments.Remove(getEnrollment);
                            ApplicationDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        ListUserCancel2.Add(new user2
                        {
                            UserName = useractual.UserName,
                            Name = useractual.FirstName + useractual.LastName,
                            Email = useractual.Email
                        });
                        TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                        TempData["Info2"] = "Los siguientes usuarios no tienen matriculado este curso.";
                    }
                }
            }
            else
            {
                TempData["Info"] = "No se puede cancelar la matricula debido a que,los siguientes usuarios ya tienen avances en este curso.";
                TempData["Info3"] = "No hay usuarios asociados con este cargo";
            }
            List<Areas> ListAreas = new List<Areas>();
            var areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListAreas.Add(new Areas
            {
                Modu_Id = c,
                Listareas = areas
            });
            List<Positions> ListPositions = new List<Positions>();
            var positions = ApplicationDbContext.Position.Where(x => x.CompanyId == GetModuleCompany).ToList();

            ListPositions.Add(new Positions
            {
                Modu_Id = c,
                Listpositions = positions
            });
            List<Cities> ListCities = new List<Cities>();
            var city = ApplicationDbContext.City.ToList();
            ListCities.Add(new Cities
            {
                Modu_Id = c,
                Listcities = city
            });
            List<Locations> ListLocations = new List<Locations>();
            var location = ApplicationDbContext.Location.Where(x => x.CompanyId == GetModuleCompany).ToList();
            ListLocations.Add(new Locations
            {
                Modu_Id = c,
                Listlocations = location
            });
            AdminTrainingMassiveEnrollmentViewMode model = new AdminTrainingMassiveEnrollmentViewMode
            {
                ActualRole = GetActualUserId().Role,
                Module = getModule,
                ListAllUser = Listalluser,
                ListAreas = ListAreas,
                ListPositions = ListPositions,
                ListCitices = ListCities,
                ListLocations = ListLocations,
                ListUserCancel = ListUserCancel,
                ListUserCancel1 = ListUserCancel1,
                ListUserCancel2 = ListUserCancel2
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View("CancelEnrollments", model);
        }

    }
}