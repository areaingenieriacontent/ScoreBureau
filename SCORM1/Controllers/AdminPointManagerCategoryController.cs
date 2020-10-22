using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Engagement;
using SCORM1.Models.Logs;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class AdminPointManagerCategoryController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
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
        public List<CategoryPrize> SearchCategoryPrize { get; private set; }
        public List<Prize> searchPrize { get; private set; }

        public AdminPointManagerCategoryController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
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
        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
        private IEnumerable<SelectListItem> GetCategoryPrizeIEnum()
        {
            var categoryprizecompaniid = GetActualUserId().CompanyId;
            var dbcategoryprize = ApplicationDbContext.CategoryPrizes.Where(x => x.CompanyId == categoryprizecompaniid);
            var categoryprize = dbcategoryprize
                        .Select(categoryprizes =>
                                new SelectListItem
                                {
                                    Value = categoryprizes.Capr_Id.ToString(),
                                    Text = categoryprizes.Capr_category
                                });

            return new SelectList(categoryprize, "Value", "Text");
        }

        [Authorize]
        public ActionResult PointManagerCategory()
        {
            var GetPointManagerCategoryCompanyId = GetActualUserId().CompanyId;
            AdminPointManagerCategory model = new AdminPointManagerCategory { ActualRole = GetActualUserId().Role, PointManagerCategory = ApplicationDbContext.PointManagerCategory.Where(c => c.CompanyId == GetPointManagerCategoryCompanyId).ToList(), Logo=GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(42);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(270);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la vista de gestión de puntos, en la compañía con id " + company.CompanyId,
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddPointManagerCategory(AdminPointManagerCategory model)
        {
            var GetPointManagerCategoryCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                TempData["Add"] = "Se han agregeado los puntos de la categoria  ";
                PointManagerCategory pointManagerCategory = new PointManagerCategory { PoMaCa_course = model.PoMaCa_course, PoMaCa_Periodical = model.PoMaCa_Periodical, PoMaCa_Improvements = model.PoMaCa_Improvements, Company = GetActualUserId().Company };
                ApplicationDbContext.PointManagerCategory.Add(pointManagerCategory);
                ApplicationDbContext.SaveChanges();
                model.PointManagerCategory = ApplicationDbContext.PointManagerCategory.Where(c => c.CompanyId == GetPointManagerCategoryCompanyId).ToList();
                var table = ApplicationDbContext.TableChanges.Find(42);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(271);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = pointManagerCategory.PoMaCa_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de agregar puntos de buenas practicas,mejoras y comentarios del periodico con id " + pointManagerCategory.PoMaCa_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("PointManagerCategory");
            }
            else
            {
                TempData["Add"] = "Cantidad de puntos";
                model.ActualRole = GetActualUserId().Role;
                model.PointManagerCategory = ApplicationDbContext.PointManagerCategory.Where(c => c.CompanyId == GetPointManagerCategoryCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                var table = ApplicationDbContext.TableChanges.Find(42);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(271);
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
                        Log_StateLogs = LOGSTATE.NoRealizado,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento agregar puntos de buenas practicas,mejoras y comentarios del periodico, pero no ha llenado los campos, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("PointManagerCategory", model);
            }
        }
        [Authorize]
        public ActionResult DeletePointManagerCategory(int id)
        {
            TempData["Delete"] = "Puntos eliminados";
            var GetPointManagerCategoryCompanyId = GetActualUserId().CompanyId;
            PointManagerCategory deletedPointManagerCategory = ApplicationDbContext.PointManagerCategory.Find(id);
            var table = ApplicationDbContext.TableChanges.Find(42);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(273);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = deletedPointManagerCategory.PoMaCa_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de eliminar puntos de buenas practicas,mejoras y comentarios del periodico con id " + deletedPointManagerCategory.PoMaCa_Id + ", en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.PointManagerCategory.Remove(deletedPointManagerCategory);
            ApplicationDbContext.SaveChanges();
            AdminPointManagerCategory model = new AdminPointManagerCategory { ActualRole = GetActualUserId().Role, PointManagerCategory = ApplicationDbContext.PointManagerCategory.Where(c => c.CompanyId == GetPointManagerCategoryCompanyId).ToList() };
            return RedirectToAction("PointManagerCategory");
        }

        [Authorize]
        public ActionResult UpdatePointManagerCategory(int id)
        { 
            var GetPointManagerCategoryCompanyId = GetActualUserId().CompanyId;
            PointManagerCategory updatedPointManagerCategory = ApplicationDbContext.PointManagerCategory.Find(id);
            TempData["UpdatePointManagerCategory"] = "Puntos modificados  con exito";
            AdminPointManagerCategory model = new AdminPointManagerCategory
            {
                ActualRole = GetActualUserId().Role,
                PointManagerCategory = ApplicationDbContext.PointManagerCategory.Where(c => c.CompanyId == GetPointManagerCategoryCompanyId).ToList(),
                PoMaCa_Id = updatedPointManagerCategory.PoMaCa_Id,
                PoMaCa_Periodical = updatedPointManagerCategory.PoMaCa_Periodical,
                PoMaCa_course = updatedPointManagerCategory.PoMaCa_course,
                PoMaCa_Improvements = updatedPointManagerCategory.PoMaCa_Improvements
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(42);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(272);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = updatedPointManagerCategory.PoMaCa_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Selecciono puntos de buenas practicas,mejoras y comentarios del periodico con id " + updatedPointManagerCategory.PoMaCa_Id + "para modificar, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("PointManagerCategory", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdatePointManagerCategory(AdminPointManagerCategory model)
        {
            var GetPointManagerCategoryCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                PointManagerCategory updatePointManagerCategory = ApplicationDbContext.PointManagerCategory.Find(model.PoMaCa_Id);
                TempData["Edit"] = "Puntos modificados con exito";
                updatePointManagerCategory.PoMaCa_course = model.PoMaCa_course;
                updatePointManagerCategory.PoMaCa_Periodical = model.PoMaCa_Periodical;
                updatePointManagerCategory.PoMaCa_Improvements = model.PoMaCa_Improvements; 
                ApplicationDbContext.SaveChanges();
                model.PointManagerCategory = ApplicationDbContext.PointManagerCategory.Where(c => c.CompanyId == GetPointManagerCategoryCompanyId).ToList();
                var table = ApplicationDbContext.TableChanges.Find(42);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(272);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = updatePointManagerCategory.PoMaCa_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Modifico puntos de buenas practicas,mejoras y comentarios del periodico con id " + updatePointManagerCategory.PoMaCa_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("PointManagerCategory", model);
            }
            else
            {
                TempData["Edit"] = "Debe ingresar la cantidad de puntos";
                model.ActualRole = GetActualUserId().Role;
                model.PointManagerCategory = ApplicationDbContext.PointManagerCategory.Where(c => c.CompanyId == GetPointManagerCategoryCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("PointManagerCategory", model);
            }
        }


        [Authorize]
        public ActionResult Points(int? page)
        {
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId && x.Role == ROLES.Usuario).ToList().ToPagedList(page ?? 1, 6);
            pointuser model = new pointuser { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(22);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(205);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Ingreso a la vista de gestión de puntos adicionales, en la compañía con id " + company.CompanyId,
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
        [Authorize]
        public ActionResult SeachUserp(pointuser model, int? page)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchUser) || string.IsNullOrEmpty(model.SearchUser))
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(206);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario, sin ingresar ningún nombre para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Points");
            }
            else
            {
                IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && x.Role == ROLES.Usuario && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser)||x.UserName.Contains(model.SearchUser))).ToList().ToPagedList(page ?? 1, 6);
                model = new pointuser { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser, Logo = GetUrlLogo() };
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(206);
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
                return View("Points", model);
            }
        }


        [Authorize]
        public ActionResult AddPoin(string id, int? page)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId).ToList().ToPagedList(page ?? 1, 6);
            pointuser model = new pointuser { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser, user = user, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            TempData["Info"] = "Datos";
            return View("Points", model);

        }

        [HttpPost]
        [Authorize]
        public ActionResult Pointadd(pointuser model)
        {
            var GetPointManagerCategoryCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                var user = ApplicationDbContext.Users.Find(model.user.Id);
                var userpoint = ApplicationDbContext.Points.Where(x => x.ApplicationUser.Id == model.user.Id && x.TypePoint.Poin_TypePoints == TYPEPOINTS.EXTRA).ToList();
                if (userpoint.Count != 0)
                {
                    var a = userpoint.FirstOrDefault();
                    a.Quantity_Points = a.Quantity_Points + model.puntos;
                    ApplicationDbContext.SaveChanges();
                }
                else
                {
                    var cate = ApplicationDbContext.TypePoints.FirstOrDefault(x => x.Poin_TypePoints == TYPEPOINTS.EXTRA);
                    var point = new Point
                    {
                        ApplicationUser = user,
                        TypePoint = cate,
                        TyPo_Id=cate.TyPo_Id,
                        Quantity_Points = model.puntos,
                        User_Id = user.Id,
                        Poin_Date = DateTime.Now
                    };
                    ApplicationDbContext.Points.Add(point);
                    ApplicationDbContext.SaveChanges();
                }
                var table = ApplicationDbContext.TableChanges.Find(41);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(319);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = user.Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Asigno puntos adicioanles al usuario con id " + user.Id + " por: " + model.Description + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Info"] = "Puntos Asignados";
                TempData["Info2"] = "Puntos Asignados";
                return RedirectToAction("Points");
            }
            else
            {
                TempData["Info"] = "Los campos no pueden ser vacios";
                return RedirectToAction("Points");
            }
        }



    }

}
