using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SCORM1.Models;
using SCORM1.Models.SCORM1;
using SCORM1.Models.ViewModel;
using SCORM1.Models.Newspaper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Excel;
using System.IO;
using SCORM1.Enum;
using System.Net.Mail;
using PagedList;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.Logs;

namespace SCORM1.Controllers
{
    public class UserAndMassiveManagementController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private ApplicationSignInManager _signInManager;

        public UserAndMassiveManagementController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));

        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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
        // GET: UserAndMassiveManagement
        public ActionResult ManagementUser(int? page)
        {
            int companyId = (int)GetActualUserId().CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);

            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(),
                PositionTheCompany = GetPosition(),
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(251);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de gestión de usuarios, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario  sin ingresar ningún nombre para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
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

                model = new UserAndMassiveManagementViewModel
                {
                    UserOfCompany = ListOfUser,
                    AreasOfTheCompany = GetArea(),
                    CityOfTheCompany = GetCityOfTheCompany(),
                    LocationOfTheCompany = GetLocationOfTheCompany(),
                    PositionTheCompany = GetPosition(),
                    Logo = GetUrlLogo()
                };
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario ingresando un nombre para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("ManagementUser", model);
            }
        }

        public IEnumerable<SelectListItem> GetArea()
        {
            int CompanyId = GetActualUserId().Company.CompanyId;
            List<Area> AreaOfMyCompany = ApplicationDbContext.Areas.Where(x => x.Company.CompanyId == CompanyId).ToList();
            IEnumerable<SelectListItem> Areas = AreaOfMyCompany.Select(x =>
           new SelectListItem
           {
               Value = x.AreaId.ToString(),
               Text = x.AreaName
           });
            return new SelectList(Areas, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetPosition()
        {
            int CompanyId = GetActualUserId().Company.CompanyId;
            List<Position> PositionTheCompany = ApplicationDbContext.Position.Where(x => x.Company.CompanyId == CompanyId).ToList();
            IEnumerable<SelectListItem> Positions = PositionTheCompany.Select(x =>
           new SelectListItem
           {
               Value = x.Posi_id.ToString(),
               Text = x.Posi_Description
           });
            return new SelectList(Positions, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetCityOfTheCompany()
        {
            int CompanyId = GetActualUserId().Company.CompanyId;
            List<City> CityOfTheCompany = ApplicationDbContext.City.ToList();
            IEnumerable<SelectListItem> Citys = CityOfTheCompany.Select(x =>
           new SelectListItem
           {
               Value = x.City_Id.ToString(),
               Text = x.City_Name
           });
            return new SelectList(Citys, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetLocationOfTheCompany()
        {
            int CompanyId = GetActualUserId().Company.CompanyId;
            List<Location> LocationOfTheCompany = ApplicationDbContext.Location.Where(x => x.Company.CompanyId == CompanyId).ToList();
            IEnumerable<SelectListItem> Locations = LocationOfTheCompany.Select(x =>
           new SelectListItem
           {
               Value = x.Loca_Id.ToString(),
               Text = x.Loca_Description
           });
            return new SelectList(Locations, "Value", "Text");
        }
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
        private string GetUrlLogo()
        {
            var companyId = (int)GetActualUserId().CompanyId;
            string Logo = "";
            StylesLogos CompanyLogo = ApplicationDbContext.StylesLogos.Where(x => x.companyId == companyId).FirstOrDefault();
            if (CompanyLogo != null)
            {
                Logo = CompanyLogo.UrlLogo;
            }
            else
            {
                Logo = ApplicationDbContext.StylesLogos.Find(1).UrlLogo;
            }
            return Logo;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserAndMassiveManagementViewModel UserObtainedOfTheView)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser UserToCreate = new ApplicationUser
                {
                    UserName = UserObtainedOfTheView.UserName,
                    FirstName = UserObtainedOfTheView.FirstName,
                    LastName = UserObtainedOfTheView.LastName,
                    Email = UserObtainedOfTheView.Email,
                    Document = UserObtainedOfTheView.Document,
                    StateUser = UserObtainedOfTheView.StateUser,
                    Country = UserObtainedOfTheView.Country,
                    Role = (ROLES)UserObtainedOfTheView.Role,
                    AreaId = UserObtainedOfTheView.AreaId,
                    Company = GetActualUserId().Company,
                    CityId = UserObtainedOfTheView.CityId,
                    PositionId = UserObtainedOfTheView.PositionId,
                    LocationId = UserObtainedOfTheView.LocationId,
                    Enable = UserObtainedOfTheView.enable,
                    TermsandConditions = Terms_and_Conditions.No_apceptado,
                    Videos = VIDEOS.No_apceptado,
                    SesionUser = SESION.No,
                    TermsJuego = Terms_and_Conditions.No_apceptado,
                    ComunidadActiva = UserObtainedOfTheView.ComunidadActiva,
                    hasClientProfile = UserObtainedOfTheView.hasClientProfile
                };
                string next = VerifyUserFields(UserToCreate);

                if (next == "success")
                {
                    IdentityResult result = await UserManager.CreateAsync(UserToCreate, UserObtainedOfTheView.UserName);
                    if (result.Succeeded)
                    {
                        SendEmail(UserToCreate.FirstName + " " + UserToCreate.LastName, UserToCreate.Email, UserToCreate.UserName, UserToCreate.Company.CompanyName);
                        TempData["Menssage"] = "Usuario creado con éxito";
                        var table = ApplicationDbContext.TableChanges.Find(72);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(253);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = UserToCreate.Id
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear un usuario con id " + UserToCreate.Id + ", en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        return RedirectToAction("ManagementUser");
                    }
                    TempData["Warning"] = result.Errors.First();
                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    TempData["Menssage"] = " Error en la carga: descripcion. " + error + " favor verifica";
                    var table = ApplicationDbContext.TableChanges.Find(72);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(253);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento crear un usuario con id " + UserToCreate.Id + " pero genero en siguiente error" + error + ", en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("ManagementUser");
                }

            }
            UserObtainedOfTheView.Sesion = GetActualUserId().SesionUser;
            return RedirectToAction("ManagementUser", UserObtainedOfTheView);
        }
        private string VerifyUserFields(ApplicationUser result)
        {
            int CompanyId = (int)GetActualUserId().CompanyId;

            List<Area> Areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == CompanyId).ToList();
            List<City> Citys = ApplicationDbContext.City.ToList();
            List<Location> locations = ApplicationDbContext.Location.Where(x => x.CompanyId == CompanyId).ToList();
            List<Position> positions = ApplicationDbContext.Position.ToList();

            if (result.Email == null || result.Email.Length > 60)
            {
                return "no hay Email o excede el tamaño;";
            }
            if (result.FirstName == null || result.FirstName.Length > 100)
            {
                return "no hay nombres o excede el tamaño;";
            }
            if (result.LastName == null || result.LastName.Length > 100)
            {
                return "no hay apellidos o excede el tamaño;";
            }
            if (Areas.Count(x => x.AreaId == result.AreaId) <= 0)
            {
                return "no hay area registrada;";
            }
            if (Citys.Count(x => x.City_Id == result.CityId) <= 0)
            {
                return "no hay ciudad registrada;";
            }
            if (locations.Count(x => x.Loca_Id == result.LocationId) <= 0)
            {
                return "no hay ubicacion registrada;";
            }
            if (positions.Count(x => x.Posi_id == result.PositionId) <= 0)
            {
                return "no hay cargo registrado;";
            }
            return "success";
        }

        public ActionResult DeleteUser(string IdUserToDelete)
        {
            if (IdUserToDelete != GetActualUserId().Id)
            {
                ApplicationUser user = UserManager.FindById(IdUserToDelete);
                foreach (var point in user.PointsComment.ToList())
                {
                    PointsComment Comment = ApplicationDbContext.PointsComments.Find(point.Comm_Id);
                    ApplicationDbContext.PointsComments.Remove(Comment);
                    ApplicationDbContext.SaveChanges();
                }
                Boolean veruser = VeriUserDelete(user.Id);
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(255);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = user.Id
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de eliminar un usuario con id " + user.Id + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                if (veruser == true)
                {
                    UserManager.Delete(user);
                    TempData["Menssage"] = "Usuario elimado con éxito";
                    return RedirectToAction("ManagementUser");
                }
                else
                {
                    TempData["Menssage"] = "Problemas al elimar el usuario";
                    return RedirectToAction("ManagementUser");
                }



            }
            else
            {
                TempData["Menssage"] = "No puedes eliminar el usuario con el que esta logeado";
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(255);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = UserCurrent.Id
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminarse asi mismo, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }

        }

        public Boolean VeriUserDelete(string User_id)
        {
            ApplicationUser user = UserManager.FindById(User_id);
            var avcourse = ApplicationDbContext.AdvanceCourses.Where(x => x.User_Id == user.Id).ToList();
            if (avcourse.Count != 0)
            {
                foreach (var item in avcourse)
                {
                    ApplicationDbContext.AdvanceCourses.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var avuser = ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == user.Id).ToList();
            if (avuser.Count != 0)
            {
                foreach (var item in avuser)
                {
                    ApplicationDbContext.AdvanceUsers.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var betterPractice = ApplicationDbContext.BetterPractices.Where(x => x.User_Id == user.Id).ToList();
            if (betterPractice.Count != 0)
            {
                foreach (var item in betterPractice)
                {
                    ApplicationDbContext.BetterPractices.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var improvement = ApplicationDbContext.Improvements.Where(x => x.User_Id == user.Id).ToList();
            if (improvement.Count != 0)
            {
                foreach (var item in improvement)
                {
                    ApplicationDbContext.Improvements.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var attempts = ApplicationDbContext.Attempts.Where(x => x.UserId == user.Id).ToList();
            if (attempts.Count != 0)
            {
                foreach (var item in attempts)
                {
                    ApplicationDbContext.Attempts.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var newattempts = ApplicationDbContext.NewAttempts.Where(x => x.User_Id == user.Id).ToList();
            if (newattempts.Count != 0)
            {
                foreach (var item in newattempts)
                {
                    ApplicationDbContext.NewAttempts.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var certification = ApplicationDbContext.Certifications.Where(x => x.User_Id == user.Id).ToList();
            if (certification.Count != 0)
            {
                foreach (var item in certification)
                {
                    ApplicationDbContext.Certifications.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var enrollment = ApplicationDbContext.Enrollments.Where(x => x.User_Id == user.Id).ToList();
            if (enrollment.Count != 0)
            {
                foreach (var item in enrollment)
                {
                    ApplicationDbContext.Enrollments.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }

            var exchange = ApplicationDbContext.Exchanges.Where(x => x.ApplicationUser == user.Id).ToList();
            if (exchange.Count != 0)
            {
                foreach (var item in exchange)
                {
                    ApplicationDbContext.Exchanges.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var logs = ApplicationDbContext.Logs.Where(x => x.User_Id == user.Id).ToList();
            if (logs.Count != 0)
            {
                foreach (var item in logs)
                {
                    ApplicationDbContext.Logs.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var point = ApplicationDbContext.Points.Where(x => x.User_Id == user.Id).ToList();
            if (point.Count != 0)
            {
                foreach (var item in point)
                {
                    ApplicationDbContext.Points.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var exch = ApplicationDbContext.Exchanges.Where(x => x.User.Id == user.Id).ToList();
            if (exch.Count != 0)
            {
                foreach (var item in exch)
                {
                    ApplicationDbContext.Exchanges.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            var coment = ApplicationDbContext.Comments.Where(x => x.comm_Author.Id == user.Id).ToList();
            if (coment.Count != 0)
            {
                foreach (var item in coment)
                {
                    ApplicationDbContext.Comments.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            return true;
        }

        [HttpGet]
        public ActionResult UpdateUserCurrent(string IdUserToModified, int? page)
        {
            ApplicationUser user = UserManager.FindById(IdUserToModified);
            int companyId = (int)GetActualUserId().CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel UserToModified = new UserAndMassiveManagementViewModel
            {
                User_Id = IdUserToModified,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Document = user.Document,
                StateUser = user.StateUser,
                Country = user.Country,
                Role = (ADMIN_CREATE_ROLES)user.Role,
                AreaId = user.AreaId,
                CityId = user.CityId,
                PositionId = user.PositionId,
                LocationId = user.LocationId,
                UserOfCompany = ListOfUser,
                enable = user.Enable,
                AreasOfTheCompany = GetArea(),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(),
                PositionTheCompany = GetPosition(),
                Logo = GetUrlLogo()
            };
            TempData["UpdateUserCurrent"] = "Actualizar";
            UserToModified.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(254);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = user.Id
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar un usuario con id " + user.Id + " para modificar, en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("ManagementUser", UserToModified);
        }

        [HttpPost]
        public ActionResult UpdateUserCurrent(UserAndMassiveManagementViewModel UserToModified)
        {
            ApplicationUser user = UserManager.FindById(UserToModified.User_Id);

            user.UserName = UserToModified.UserName;
            user.FirstName = UserToModified.FirstName;
            user.LastName = UserToModified.LastName;
            user.Email = UserToModified.Email;
            user.Document = UserToModified.Document;
            user.Country = UserToModified.Country;
            user.Role = (ROLES)UserToModified.Role;
            user.AreaId = UserToModified.AreaId;
            user.CityId = UserToModified.CityId;
            user.PositionId = UserToModified.PositionId;
            user.LocationId = UserToModified.LocationId;
            user.Enable = UserToModified.enable;
            UserManager.RemovePassword(user.Id);
            UserManager.AddPassword(user.Id, UserToModified.UserName);
            UserManager.Update(user);
            TempData["Menssage"] = "Usuario modificado con éxito";
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(254);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = user.Id
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar un usuario con id " + user.Id + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("ManagementUser");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult MassiveRegister()
        {
            comoustdlollame model = new comoustdlollame
            {
                Logo = GetUrlLogo()

            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(268);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de carga masiva de usuarios, en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> MassiveRegister(HttpPostedFileBase excelUpload)
        {
            comoustdlollame model = new comoustdlollame
            {
                Logo = GetUrlLogo()

            };
            if (excelUpload != null && excelUpload.ContentLength > 0)
            {
                Stream stream = excelUpload.InputStream;
                IExcelDataReader reader = null;

                if (excelUpload.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (excelUpload.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    ModelState.AddModelError("File", "Este formato no es soportado");
                    model.Sesion = GetActualUserId().SesionUser;
                    var table = ApplicationDbContext.TableChanges.Find(72);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(269);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento subir un formato no compatible para la carga masiva de usuario, en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return View(model);
                }

                reader.IsFirstRowAsColumnNames = true;

                DataSet result = reader.AsDataSet();
                string next = VerifyUserFields(result);
                if (next == "success")
                {
                    var Country = GetActualUserId().Country;
                    var CompanyId = GetActualUserId().CompanyId;
                    foreach (DataTable table in result.Tables)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            ApplicationUser user = new ApplicationUser
                            {
                                UserName = table.Rows[i].ItemArray[0].ToString(),
                                Email = table.Rows[i].ItemArray[1].ToString(),
                                FirstName = table.Rows[i].ItemArray[2].ToString(),
                                LastName = table.Rows[i].ItemArray[3].ToString(),
                                Document = table.Rows[i].ItemArray[4].ToString(),
                                StateUser = STATEUSER.Usuario,
                                AreaId = Int32.Parse(table.Rows[i].ItemArray[5].ToString()),
                                CityId = Int32.Parse(table.Rows[i].ItemArray[6].ToString()),
                                PositionId = Int32.Parse(table.Rows[i].ItemArray[7].ToString()),
                                LocationId = Int32.Parse(table.Rows[i].ItemArray[8].ToString()),
                                Country = Country,
                                CompanyId = CompanyId,
                                Company = GetActualUserId().Company,
                                TermsandConditions = Terms_and_Conditions.No_apceptado,
                                Videos = VIDEOS.No_apceptado,
                                SesionUser = SESION.No,
                                TermsJuego = Terms_and_Conditions.No_apceptado,
                                ComunidadActiva=0,
                                hasClientProfile=false
                            };
                            IdentityResult results = await UserManager.CreateAsync(user, user.UserName);
                            AddErrors(results);
                            if (results.Succeeded == true)
                            {
                                SendEmail(user.FirstName + " " + user.LastName, user.Email, user.UserName, user.Company.CompanyName);
                                var table1 = ApplicationDbContext.TableChanges.Find(72);
                                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                                var code = ApplicationDbContext.CodeLogs.Find(253);
                                var idcompany = UserCurrent.CompanyId;
                                if (idcompany != null)
                                {
                                    var company = ApplicationDbContext.Companies.Find(idcompany);
                                    string ip = IpUser();
                                    var idchange = new IdChange
                                    {
                                        IdCh_IdChange = user.Id
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
                                        TableChange = table1,
                                        TaCh_Id = table1.TaCh_Id,
                                        IdChange = idchange,
                                        IdCh_Id = idchange.IdCh_Id,
                                        User_Id = UserCurrent.Id,
                                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear de forma masiva un usuario con id " + user.Id + ", en la compañía con id" + company.CompanyId,
                                        Company = company,
                                        Company_Id = company.CompanyId,
                                        Log_Ip = ip
                                    };
                                    ApplicationDbContext.Logs.Add(logsesiontrue);
                                    ApplicationDbContext.SaveChanges();
                                }
                            }
                        }
                        reader.Close();
                        comoustdlollame n = new comoustdlollame { data = result.Tables[0], Logo = GetUrlLogo() };
                        n.Sesion = GetActualUserId().SesionUser;
                        return View(n);
                    }
                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    string line = Convert.ToString(Int32.Parse(a[1]) + 2);
                    TempData["Menssage"] = " Error en la carga: descripcion. " + error + " favor verifica la linea:" + line;
                    model.Sesion = GetActualUserId().SesionUser;
                    var table = ApplicationDbContext.TableChanges.Find(72);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(253);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento crear un usuario de forma masiva pero genero el siguiente error " + error + ", en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("File", "Por favor selecciona un formato valido");
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(269);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento subir un formato no compatible para la carga masiva de usuario, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
            }
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        private string VerifyUserFields(DataSet result)
        {
            int CompanyId = (int)GetActualUserId().CompanyId;

            List<Area> Areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == CompanyId).ToList();
            List<City> Citys = ApplicationDbContext.City.ToList();
            List<Location> locations = ApplicationDbContext.Location.Where(x => x.CompanyId == CompanyId).ToList();
            List<Position> positions = ApplicationDbContext.Position.ToList();

            foreach (DataTable Table in result.Tables)
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    string username = Table.Rows[i].ItemArray[0].ToString();
                    string email = Table.Rows[i].ItemArray[1].ToString();
                    string firstname = Table.Rows[i].ItemArray[2].ToString();
                    string lastname = Table.Rows[i].ItemArray[3].ToString();
                    string document = Table.Rows[i].ItemArray[4].ToString();
                    int Area = Int32.Parse(Table.Rows[i].ItemArray[5].ToString());
                    int city = Int32.Parse(Table.Rows[i].ItemArray[6].ToString());
                    int position = Int32.Parse(Table.Rows[i].ItemArray[7].ToString());
                    int location = Int32.Parse(Table.Rows[i].ItemArray[8].ToString());
                    if (username == null || username.Length > 300)
                    {
                        return "No hay username o excede los 60 caracteres;" + i;
                    }
                    if (email == null || email.Length > 300)
                    {
                        return "nNo hay Email o excede los 60 caracteres;" + i;
                    }
                    if (firstname == null || firstname.Length > 300)
                    {
                        return "No hay nombre o excede los 60 caracteres;" + i;
                    }
                    if (lastname == null || lastname.Length > 300)
                    {
                        return "No hay apellidos o excede los 60 caracteres;" + i;
                    }
                    if (Areas.Count(x => x.AreaId == Area) <= 0)
                    {
                        return "No hay área registrada;" + i;
                    }
                    if (Citys.Count(x => x.City_Id == city) <= 0)
                    {
                        return "No hay ciudad registrada;" + i;
                    }
                    if (locations.Count(x => x.Loca_Id == location) <= 0)
                    {
                        return "No hay ubicación registrada;" + i;
                    }
                    if (positions.Count(x => x.Posi_id == position) <= 0)
                    {
                        return "No hay cargo registrado;" + i;
                    }

                }
            }
            return "success";
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult MassiveUpdate()
        {
            LogoUserUpdate model = new LogoUserUpdate
            {
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(268);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de carga masiva de usuarios, en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> MassiveUpdate(HttpPostedFileBase excelUpload)
        {
            LogoUserUpdate model = new LogoUserUpdate
            {
                Logo = GetUrlLogo()
            };
            if (excelUpload != null && excelUpload.ContentLength > 0)
            {
                Stream stream = excelUpload.InputStream;
                IExcelDataReader reader = null;

                if (excelUpload.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (excelUpload.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    ModelState.AddModelError("File", "Este formato no es soportado");
                    model.Sesion = GetActualUserId().SesionUser;
                    var table = ApplicationDbContext.TableChanges.Find(72);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(269);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento subir un formato no compatible para la carga masiva de usuario, en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return View(model);
                }

                reader.IsFirstRowAsColumnNames = true;

                DataSet result = reader.AsDataSet();
                string next = VerifyUserFieldsUpdate(result);
                List<UserUpdate> modeluserupdate = new List<UserUpdate>();
                List<UserNew> modelusernew = new List<UserNew>();
                if (next == "success")
                {
                    var Country = GetActualUserId().Country;
                    var CompanyId = GetActualUserId().CompanyId;
                    foreach (DataTable table in result.Tables)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            var userna = table.Rows[i].ItemArray[0].ToString();
                            var usuario = ApplicationDbContext.Users.Where(x => x.UserName == userna).ToList();
                            if (usuario.Count != 0)
                            {
                                var actual = usuario.FirstOrDefault();
                                if (table.Rows[i].ItemArray[1].ToString() != "")
                                {
                                    actual.Email = table.Rows[i].ItemArray[1].ToString();
                                }
                                if (table.Rows[i].ItemArray[2].ToString() != "")
                                {
                                    actual.FirstName = table.Rows[i].ItemArray[2].ToString();
                                }
                                if (table.Rows[i].ItemArray[3].ToString() != "")
                                {
                                    actual.LastName = table.Rows[i].ItemArray[3].ToString();
                                }
                                if (table.Rows[i].ItemArray[4].ToString() != "")
                                {
                                    actual.Document = table.Rows[i].ItemArray[4].ToString();
                                }
                                if (table.Rows[i].ItemArray[5].ToString() != "")
                                {
                                    actual.AreaId = Int32.Parse(table.Rows[i].ItemArray[5].ToString());
                                }
                                if (table.Rows[i].ItemArray[6].ToString() != "")
                                {
                                    actual.CityId = Int32.Parse(table.Rows[i].ItemArray[6].ToString());
                                }
                                if (table.Rows[i].ItemArray[7].ToString() != "")
                                {
                                    actual.PositionId = Int32.Parse(table.Rows[i].ItemArray[7].ToString());
                                }
                                if (table.Rows[i].ItemArray[8].ToString() != "")
                                {
                                    actual.LocationId = Int32.Parse(table.Rows[i].ItemArray[8].ToString());
                                }
                                ApplicationDbContext.SaveChanges();
                                modeluserupdate.Add(new UserUpdate
                                {
                                    username = actual.UserName,
                                    firstname = actual.FirstName,
                                    lastname = actual.LastName,
                                    email = actual.Email,
                                    document = actual.Document,
                                    codigo_area = Int32.Parse(actual.AreaId.ToString()),
                                    área = actual.Area.AreaName,
                                    codigo_cargo = Int32.Parse(actual.PositionId.ToString()),
                                    cargo = actual.Position.Posi_Description,
                                    codigo_ciudad = Int32.Parse(actual.CityId.ToString()),
                                    ciudad = actual.City.City_Name,
                                    codigo_ubicación = Int32.Parse(actual.LocationId.ToString()),
                                    ubicación = actual.Location.Loca_Description
                                });
                                TempData["Menssage1"] = " Listado de Usuarios Actualizados";
                            }
                            else
                            {
                                ApplicationUser user = new ApplicationUser
                                {
                                    UserName = table.Rows[i].ItemArray[0].ToString(),
                                    Email = table.Rows[i].ItemArray[1].ToString(),
                                    FirstName = table.Rows[i].ItemArray[2].ToString(),
                                    LastName = table.Rows[i].ItemArray[3].ToString(),
                                    Document = table.Rows[i].ItemArray[4].ToString(),
                                    StateUser = STATEUSER.Usuario,
                                    AreaId = Int32.Parse(table.Rows[i].ItemArray[5].ToString()),
                                    CityId = Int32.Parse(table.Rows[i].ItemArray[6].ToString()),
                                    PositionId = Int32.Parse(table.Rows[i].ItemArray[7].ToString()),
                                    LocationId = Int32.Parse(table.Rows[i].ItemArray[8].ToString()),
                                    Country = Country,
                                    CompanyId = CompanyId,
                                    Company = GetActualUserId().Company,
                                    TermsandConditions = Terms_and_Conditions.No_apceptado,
                                    SesionUser = SESION.No,
                                    ComunidadActiva = 0,
                                    hasClientProfile = false
                                };
                                IdentityResult results = await UserManager.CreateAsync(user, user.UserName);
                                AddErrors(results);
                                if (results.Succeeded == true)
                                {
                                    SendEmail(user.FirstName + " " + user.LastName, user.Email, user.UserName, user.Company.CompanyName);
                                    var table1 = ApplicationDbContext.TableChanges.Find(72);
                                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                                    var code = ApplicationDbContext.CodeLogs.Find(253);
                                    var idcompany = UserCurrent.CompanyId;
                                    if (idcompany != null)
                                    {
                                        var company = ApplicationDbContext.Companies.Find(idcompany);
                                        string ip = IpUser();
                                        var idchange = new IdChange
                                        {
                                            IdCh_IdChange = user.Id
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
                                            TableChange = table1,
                                            TaCh_Id = table1.TaCh_Id,
                                            IdChange = idchange,
                                            IdCh_Id = idchange.IdCh_Id,
                                            User_Id = UserCurrent.Id,
                                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear de forma masiva un usuario con id " + user.Id + ", en la compañía con id" + company.CompanyId,
                                            Company = company,
                                            Company_Id = company.CompanyId,
                                            Log_Ip = ip
                                        };
                                        ApplicationDbContext.Logs.Add(logsesiontrue);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                    modelusernew.Add(new UserNew
                                    {
                                        username = user.UserName,
                                        firstname = user.FirstName,
                                        lastname = user.LastName,
                                        email = user.Email,
                                        document = user.Document,
                                        codigo_area = Int32.Parse(user.AreaId.ToString()),
                                        área = user.Area.AreaName,
                                        codigo_cargo = Int32.Parse(user.PositionId.ToString()),
                                        cargo = user.Position.Posi_Description,
                                        codigo_ciudad = Int32.Parse(user.CityId.ToString()),
                                        ciudad = user.City.City_Name,
                                        codigo_ubicación = Int32.Parse(user.LocationId.ToString()),
                                        ubicación = user.Location.Loca_Description
                                    });
                                    TempData["Menssage2"] = " Listado de Usuarios Nuevos";
                                }
                            }

                        }
                        reader.Close();
                        LogoUserUpdate n = new LogoUserUpdate
                        {
                            data = result.Tables[0],
                            Logo = GetUrlLogo(),
                            usuarios_actualizados = modeluserupdate,
                            usuarios_nuevos = modelusernew
                        };
                        n.Sesion = GetActualUserId().SesionUser;
                        TempData["Menssage"] = " Resultados";
                        return View(n);
                    }
                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    string line = Convert.ToString(Int32.Parse(a[1]) + 2);
                    TempData["Menssage"] = " Error en la actualización de datos : descripcion. " + error + " favor verifica la linea:" + line;
                    model.Sesion = GetActualUserId().SesionUser;
                    var table = ApplicationDbContext.TableChanges.Find(72);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(253);
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento crear un usuario de forma masiva pero genero el siguiente error " + error + ", en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("File", "Por favor selecciona un formato valido");
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(269);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento subir un formato no compatible para la carga masiva de usuario, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
            }
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        private string VerifyUserFieldsUpdate(DataSet result)
        {
            int CompanyId = (int)GetActualUserId().CompanyId;
            List<Area> Areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == CompanyId).ToList();
            List<City> Citys = ApplicationDbContext.City.ToList();
            List<Location> locations = ApplicationDbContext.Location.Where(x => x.CompanyId == CompanyId).ToList();
            List<Position> positions = ApplicationDbContext.Position.ToList();
            foreach (DataTable Table in result.Tables)
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    string username = Table.Rows[i].ItemArray[0].ToString();
                    string email = Table.Rows[i].ItemArray[1].ToString();
                    string firstname = Table.Rows[i].ItemArray[2].ToString();
                    string lastname = Table.Rows[i].ItemArray[3].ToString();
                    string document = Table.Rows[i].ItemArray[4].ToString();
                    string Area = Table.Rows[i].ItemArray[5].ToString();
                    string city = Table.Rows[i].ItemArray[6].ToString();
                    string location = Table.Rows[i].ItemArray[8].ToString();
                    string position = Table.Rows[i].ItemArray[7].ToString();
                    if (username == "" || username.Length > 200)
                    {
                        return "No hay username o excede los 60 caracteres;" + i;
                    }
                    if (email == "")
                    {

                    }
                    else
                    {
                        if (email.Length > 200)
                        {
                            return "nNo hay Email o excede los 60 caracteres;" + i;
                        }
                    }
                    if (firstname == "")
                    {

                    }
                    else
                    {
                        if (firstname.Length > 200)
                        {
                            return "El nombre excede los 60 caracteres;" + i;
                        }
                    }
                    if (email == "")
                    {

                    }
                    else
                    {
                        if (lastname.Length > 200)
                        {
                            return "El apellido excede los 60 caracteres;" + i;
                        }
                    }
                    if (Area == "")
                    {

                    }
                    else
                    {
                        if (Areas.Count(x => x.AreaId == Int32.Parse(Area)) <= 0)
                        {
                            return "No hay área registrada;" + i;
                        }
                    }
                    if (city == "")
                    {

                    }
                    else
                    {
                        if (Citys.Count(x => x.City_Id == Int32.Parse(city)) <= 0)
                        {
                            return "No hay ciudad registrada;" + i;
                        }
                    }
                    if (location == "")
                    {

                    }
                    else
                    {
                        if (locations.Count(x => x.Loca_Id == Int32.Parse(location)) <= 0)
                        {
                            return "No hay ubicación registrada;" + i;
                        }
                    }

                    if (position == "")
                    {

                    }
                    else
                    {

                        if (positions.Count(x => x.Posi_id == Int32.Parse(position)) <= 0)
                        {
                            return "No hay cargo registrado;" + i;
                        }
                    }


                }
            }
            return "success";
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult EmailMassive()
        {
            int companyid = GetActualUserId().Company.CompanyId;
            var user = ApplicationDbContext.Users.Where(x => x.CompanyId == companyid).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    SendEmail(item.FirstName + " " + item.LastName, item.Email, item.UserName, item.Company.CompanyName);
                }
            }
            TempData["Menssage"] = " Se ha enviado un correo de bienvenida a todos los usuarios registrados con sus datos de acceso a la plataforma.";
            return RedirectToAction("ManagementUser");
        }

        private void SendEmail(string NameUser, string Email, string Usuario, string Company)
        {
            try
            {
                MailMessage solicitud = new MailMessage();
                solicitud.Subject = "Bienvenida a la Comunidad Social de Conocimiento Bureau Veritas";
                solicitud.Body =
                    "<img src='http://aprendeyavanza2.com.co/bureauveritastrainingcommunity/content/images/image6.jpg' width='50%' />" + "<br/>" +
                    "Nos alegra saludarte, " + NameUser +
                   "<br/>" + "Te damos la bienvenida a nuestra comunidad social de conocimiento: " + "<strong>" + "Bureau Veritas Training Community." + "</strong>" + "<br/>" +
                   "<br/>" + "Nuestra comunidad es un espacio donde vas a tener la oportunidad de " + "<strong>" + "aprender y consultar conocimiento" + "<br/>" +
                   " a partir de múltiples recursos," + " </strong> " + "los cuales están diseñados para que desarrolles tus" + "<br/>" +
                   " competencias profesionales y te destaques en el mercado laboral. Aquí podrás compartir y" + "<br/>" +
                   " aprender de las experiencias de otros miembros de la comunidad, además obtendrás puntos por" + "<br/>" +
                   " tus interacciones y redimirlos por premios." + " <br/>" +
                   "<strong>" + "Ponemos a tu disposición:" + "</strong>" + "<br/><br/>" +
                   "<img src='http://aprendeyavanza2.com.co/bureauveritastrainingcommunity/content/images/imagencorreo1.png' width='50%' />" + "<br/>" +
                   "<br/></br>" + "Nuestra comunidad estará disponible para ti, las 24 horas del día y los 7 días de la semana." +
                   "<br/></br>" + "¡Te esperamos!" +
                   "<h1><strong>" + "Empieza ahora" + "</strong></h1>" +
                   "<br/>" + "Ahora es tiempo de ingresar y comenzar con tu experiencia de aprendizaje" +
                   "<br/><br/>" + "Estos son tus datos de acceso:" + "<br/><br/>" +
                   "Usuario: " + Usuario +
                   "<br/>" + "Contraseña: " + Usuario + "<br/>" +
                   "<br/>" +
                   "<a href='http://aprendeyavanza2.com.co/bureauveritastrainingcommunity'><button style='background: #CC023B; color: #fff; padding: 10px; font-size: 20px; border-radius: 13px;'>Ingresa Aqu&igrave;</button></a>";

                solicitud.To.Add(Email);
                solicitud.IsBodyHtml = true;
                var smtp2 = new SmtpClient();

                smtp2.Send(solicitud);
            }
            catch (Exception)
            {


            }

        }


        [HttpPost]
        [Authorize]
        public ActionResult AddArea(UserAndMassiveManagementViewModel model)
        {
            if (model.AreaName != null)
            {
                Area areaToCreate = new Area { AreaName = model.AreaName, CompanyId = (int)GetActualUserId().CompanyId };
                ApplicationDbContext.Areas.Add(areaToCreate);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(5);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(256);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = areaToCreate.AreaId.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear un área con id " + areaToCreate.AreaId + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }
            else
            {
                TempData["Menssage"] = "el area no puede estar vacia";
                return RedirectToAction("ManagementUser");
            }
        }


        [Authorize]
        public ActionResult DeleteArea(int id)
        {
            int listUserWithThisArea = ApplicationDbContext.Users.Where(x => x.AreaId == id).ToList().Count();
            if (listUserWithThisArea <= 0)
            {
                Area areaTodelete = ApplicationDbContext.Areas.Find(id);
                var table = ApplicationDbContext.TableChanges.Find(5);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(258);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = areaTodelete.AreaId.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Elimino un área con id " + areaTodelete.AreaId + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.Areas.Remove(areaTodelete);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "El area ha sido eliminada";
                return RedirectToAction("ManagementUser");
            }
            else
            {
                TempData["Menssage"] = "el área no puede ser elimanada porque hay usuarios asignados";
                var table = ApplicationDbContext.TableChanges.Find(5);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(258);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar un área con id " + id + " pero esta área esta asignada a un usuario, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }
        }

        [HttpGet]
        public ActionResult UpdateArea(int id, int? page)
        {
            int companyId = (int)GetActualUserId().CompanyId;
            Area area = ApplicationDbContext.Areas.Find(id);
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                AreaId = area.AreaId,
                AreaName = area.AreaName,
                Logo = GetUrlLogo(),
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(),
                PositionTheCompany = GetPosition(),
            };
            TempData["Updatearea"] = "Actualizar";
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(5);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(257);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = area.AreaId.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar un área para modificar con id " + area.AreaId + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("ManagementUser", model);
        }

        [HttpPost]
        public ActionResult UpdateArea(UserAndMassiveManagementViewModel model)
        {
            Area area = ApplicationDbContext.Areas.Find(model.AreaId);
            area.AreaName = model.AreaName;
            ApplicationDbContext.SaveChanges();
            TempData["Menssage"] = "Área modificada con éxito";
            var table = ApplicationDbContext.TableChanges.Find(5);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(257);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = area.AreaId.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar un área con id " + area.AreaId + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("ManagementUser");
        }


        [HttpPost]
        [Authorize]
        public ActionResult AddLocation(UserAndMassiveManagementViewModel model)
        {
            if (model.LocationName != null)
            {
                Location LocationToCreate = new Location { Loca_Description = model.LocationName, CompanyId = (int)GetActualUserId().CompanyId };
                ApplicationDbContext.Location.Add(LocationToCreate);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(29);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(262);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = LocationToCreate.Loca_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear una ubicación con id " + LocationToCreate.Loca_Id + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }
            else
            {
                TempData["Menssage"] = "el Locacion no puede estar vacia";
                return RedirectToAction("ManagementUser");
            }
        }

        [HttpGet]
        public ActionResult UpdateLocation(int id, int? page)
        {
            int companyId = (int)GetActualUserId().CompanyId;
            Location location = ApplicationDbContext.Location.Find(id);
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                LocationId = location.Loca_Id,
                LocationName = location.Loca_Description,
                Logo = GetUrlLogo(),
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(),
                PositionTheCompany = GetPosition(),
            };
            TempData["UpdateLocations"] = "Actualizar";
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(29);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(263);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = location.Loca_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar una ubicación para modificar con id " + location.Loca_Id + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("ManagementUser", model);
        }

        [HttpPost]
        public ActionResult Updatelocation(UserAndMassiveManagementViewModel model)
        {
            Location location = ApplicationDbContext.Location.Find(model.LocationId);
            location.Loca_Description = model.LocationName;
            ApplicationDbContext.SaveChanges();
            TempData["Menssage"] = "Ubicación modificada con éxito";
            var table = ApplicationDbContext.TableChanges.Find(29);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(263);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = location.Loca_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar una ubicación con id " + location.Loca_Id + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("ManagementUser");
        }

        [Authorize]
        public ActionResult DeleteLocation(int id)
        {
            int listUserWithThisLocation = ApplicationDbContext.Users.Where(x => x.LocationId == id).ToList().Count();
            if (listUserWithThisLocation <= 0)
            {
                Location locationTodelete = ApplicationDbContext.Location.Find(id);
                var table = ApplicationDbContext.TableChanges.Find(29);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(264);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = locationTodelete.Loca_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Elimino una ubicación con id " + locationTodelete.Loca_Id + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.Location.Remove(locationTodelete);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "La ubicación ha sido eliminada";
                return RedirectToAction("ManagementUser");
            }
            else
            {
                TempData["Menssage"] = "La ubicación no puede ser elimanada porque hay usuarios asignados";
                var table = ApplicationDbContext.TableChanges.Find(29);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(264);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar una ubicación con id " + id + " pero esta área esta asignada a un usuario, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddCitys(UserAndMassiveManagementViewModel model)
        {
            if (model.CityName != null)
            {
                City cityToCreate = new City { City_Name = model.CityName };
                ApplicationDbContext.City.Add(cityToCreate);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(16);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(265);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = cityToCreate.City_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear una ciudad con id " + cityToCreate.City_Id + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }
            else
            {
                TempData["Menssage"] = "el campo ciudad puede estar vacia";
                return RedirectToAction("ManagementUser");
            }
        }

        [HttpGet]
        public ActionResult UpdateCity(int id, int? page)
        {
            int companyId = (int)GetActualUserId().CompanyId;
            City city = ApplicationDbContext.City.Find(id);
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                CityId = city.City_Id,
                CityName = city.City_Name,
                Logo = GetUrlLogo(),
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(),
                PositionTheCompany = GetPosition(),
            };
            TempData["Updatecity"] = "Actualizar";
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(16);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(266);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = city.City_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar una ciudad para modificar con id " + city.City_Id + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("ManagementUser", model);
        }
        [HttpPost]
        public ActionResult UpdateCity(UserAndMassiveManagementViewModel model)
        {
            City city = ApplicationDbContext.City.Find(model.CityId);
            city.City_Name = model.CityName;
            ApplicationDbContext.SaveChanges();
            TempData["Menssage"] = "Ciudad modificada con éxito";
            var table = ApplicationDbContext.TableChanges.Find(16);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(266);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = city.City_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar una ciudad con id " + city.City_Id + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("ManagementUser");
        }
        [Authorize]
        public ActionResult DeleteCitys(int id)
        {
            int listUserWithThisCity = ApplicationDbContext.Users.Where(x => x.CityId == id).ToList().Count();
            if (listUserWithThisCity <= 0)
            {
                City cityTodelete = ApplicationDbContext.City.Find(id);
                var table = ApplicationDbContext.TableChanges.Find(16);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(267);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = cityTodelete.City_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Elimino una ciudad con id " + cityTodelete.City_Id + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.City.Remove(cityTodelete);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "Ciudad elimanada con éxito";
                return RedirectToAction("ManagementUser");
            }
            else
            {
                TempData["Menssage"] = "La ciudad no puede ser elimanada porque hay usuarios asignados";
                var table = ApplicationDbContext.TableChanges.Find(16);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(267);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar una ciudad con id " + id + " pero esta ciudad esta asignada a un usuario, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPosition(UserAndMassiveManagementViewModel model)
        {
            if (model.PositionName != null)
            {
                Position positionToCreate = new Position { Posi_Description = model.PositionName, CompanyId = (int)GetActualUserId().CompanyId };
                ApplicationDbContext.Position.Add(positionToCreate);
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(45);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(259);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = positionToCreate.Posi_id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de crear un cargo con id " + positionToCreate.Posi_id + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }
            else
            {
                TempData["Menssage"] = "el campo Cargo no puede estar vacia";
                return RedirectToAction("ManagementUser");
            }
        }
        [HttpGet]
        public ActionResult UpdatePosition(int idPosition, int? page)
        {
            int companyId = (int)GetActualUserId().CompanyId;
            Position position = ApplicationDbContext.Position.Find(idPosition);
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                PositionId = position.Posi_id,
                PositionName = position.Posi_Description,
                Logo = GetUrlLogo(),
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(),
                PositionTheCompany = GetPosition(),
            };
            TempData["Updateposition"] = "Actualizar";
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(45);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(260);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = position.Posi_id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de seleccionar un cargo para modificar con id " + position.Posi_id + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("ManagementUser", model);
        }
        [HttpPost]
        public ActionResult UpdatePosition(UserAndMassiveManagementViewModel model)
        {
            Position position = ApplicationDbContext.Position.Find(model.PositionId);
            position.Posi_Description = model.PositionName;
            ApplicationDbContext.SaveChanges();
            TempData["Menssage"] = "Cargo modificado con éxito";
            var table = ApplicationDbContext.TableChanges.Find(45);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(260);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = position.Posi_id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de modificar un cargo con id " + position.Posi_id + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("ManagementUser");
        }

        [Authorize]
        public ActionResult DeletePosition(int idPosition)
        {
            int listUserWithThisCity = ApplicationDbContext.Users.Where(x => x.PositionId == idPosition).ToList().Count();
            if (listUserWithThisCity <= 0)
            {
                Position positionTodelete = ApplicationDbContext.Position.Find(idPosition);
                var table = ApplicationDbContext.TableChanges.Find(45);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(261);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = positionTodelete.Posi_id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Elimino un cargo con id " + positionTodelete.Posi_id + ", en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.Position.Remove(positionTodelete);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "Cargo elimanado con éxito";
                return RedirectToAction("ManagementUser");
            }
            else
            {
                TempData["Menssage"] = "El cargo no puede ser elimanado porque hay usuarios asignados";
                var table = ApplicationDbContext.TableChanges.Find(45);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(261);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = idPosition.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento eliminar un cargo con id " + idPosition + " pero esta ciudad esta asignada a un usuario, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ManagementUser");
            }
        }

    }
}