using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Engagement;
using SCORM1.Models.Lms;
using SCORM1.Models.Newspaper;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using SCORM1.Models.PageCustomization;
using PagedList;
using SCORM1.Models.Logs;

namespace SCORM1.Controllers
{
    public class AdminGeneralController : Controller
    {

        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        public List<CategoryPrize> SearchCategoryPrize { get; private set; }
        public List<Prize> searchPrize { get; private set; }

        public AdminGeneralController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
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
        public ActionResult Admin(AdminGeneralControllerViewModel model)
        {
            int CompanyUser = (int)GetActualUserId().CompanyId;

            try
            {
                var url = HttpRuntime.AppDomainAppVirtualPath;

                Edition EditionToShow = ApplicationDbContext.Editions.Where(x => x.Edit_StateEdition == EDITIONSTATE.Activo && x.CompanyId == CompanyUser).First();
                List<Article> ListArticlesToSend = ApplicationDbContext.Articles.Where(x => x.Section.Edition.Edit_Id == EditionToShow.Edit_Id).ToList();
                model = new AdminGeneralControllerViewModel { ActualRole = GetActualUserId().Role, EditionCurrentToActive = EditionToShow, ListArticles = ListArticlesToSend };
            }
            catch (Exception) { }
            List<Banner> banners = ApplicationDbContext.Banners.Where(x => x.companyId == CompanyUser).ToList();

            if (banners != null && banners.Count != 0)
            {
                model.Banners = banners;
            }
            else
            {
                model.Banners = ApplicationDbContext.Banners.Where(x => x.Bann_Id <= 4).ToList();
            }
            var titles = ApplicationDbContext.StylesLogos.Where(x => x.companyId == CompanyUser).FirstOrDefault();
            if (titles != null)
            {
                model.Title1 = titles.Title1;
                model.Title2 = titles.Title2;
                model.Title3 = titles.Title3;
            }
            else
            {
                model.Title1 = ApplicationDbContext.StylesLogos.Find(1).Title1;
                model.Title2 = ApplicationDbContext.StylesLogos.Find(1).Title2;
                model.Title3 = ApplicationDbContext.StylesLogos.Find(1).Title3;
            }
            List<Module> listmoduleVirtual = new List<Module>();
            List<Module> listmoduleEvaluative = new List<Module>();
            var GetModuleCompany = GetActualUserId().CompanyId;
            listmoduleVirtual = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompany && m.Modu_TypeOfModule == CURSO.Virtual && m.Modu_Statemodule == MODULESTATE.Activo).ToList();
            listmoduleEvaluative = ApplicationDbContext.Modules.Where(m => m.CompanyId == GetModuleCompany && m.Modu_TypeOfModule == CURSO.Evaluativo && m.Modu_Statemodule == MODULESTATE.Activo).ToList();
            model.Listmoduleevaluative = listmoduleEvaluative;
            model.Listmodulevirtual = listmoduleVirtual;
            model.Sesion = GetActualUserId().SesionUser;
            model.juego = GetActualUserId().Company.CompanyGame;
            return PartialView("_Admin", model);
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
        public ActionResult CategoryPrize()
        {
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(295);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de gestión de categoria de premios en la compañia con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            AdminGeneralCategoryPrizeViewModel model = new AdminGeneralCategoryPrizeViewModel { ActualRole = GetActualUserId().Role, ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetCategoryPrizeCompanyId).ToList(), Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchCategoryPrizes(AdminGeneralCategoryPrizeViewModel model)
        {
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.searchCategoryPrize) || string.IsNullOrEmpty(model.searchCategoryPrize))
            {
                var table = ApplicationDbContext.TableChanges.Find(13);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(29);
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
                        Log_StateLogs = LOGSTATE.Error,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de una categoría de un premio sin ingresar ningún nombre de una categoría para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                model = new AdminGeneralCategoryPrizeViewModel { ActualRole = GetActualUserId().Role, ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetCategoryPrizeCompanyId).ToList() };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("CategoryPrize", model);
            }
            else
            {
                var table = ApplicationDbContext.TableChanges.Find(13);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(296);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de una categoría de un premio ingresando un nombre de una categoría para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                List<CategoryPrize> SearchedCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(m => m.Capr_category.Contains(model.searchCategoryPrize)).ToList();
                model = new AdminGeneralCategoryPrizeViewModel { ActualRole = GetActualUserId().Role, ListCategoryPrize = SearchedCategoryPrize };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("CategoryPrize", model);
            }
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategoryPrize(AdminGeneralCategoryPrizeViewModel model)
        {
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {

                TempData["Add"] = "La Categoria se ha creado con éxito";
                CategoryPrize categoryPrize = new CategoryPrize { Capr_category = model.Capr_category, Company = GetActualUserId().Company };
                ApplicationDbContext.CategoryPrizes.Add(categoryPrize);
                ApplicationDbContext.SaveChanges();
                model.ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetCategoryPrizeCompanyId).ToList();
                var table = ApplicationDbContext.TableChanges.Find(13);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(297);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = categoryPrize.Capr_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de añadir una nueva categoría de un premio, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("CategoryPrize");
            }
            else
            {
                TempData["Add"] = "Ingrese Nombre de Categoria";
                model.ActualRole = GetActualUserId().Role;
                model.ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetCategoryPrizeCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                var table = ApplicationDbContext.TableChanges.Find(13);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(297);
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
                        Log_StateLogs = LOGSTATE.Error,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " no ha agregado un nombre para crear una nueva categoría, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("CategoryPrize", model);
            }
        }
        [Authorize]
        public ActionResult DeleteCategoryPrize(int id)
        {
            TempData["Delete"] = "Categoria eliminada";
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            CategoryPrize deletedCategoryPrize = ApplicationDbContext.CategoryPrizes.Find(id);
            var table = ApplicationDbContext.TableChanges.Find(13);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(299);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = deletedCategoryPrize.Capr_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de eliminar una nueva categoría de un premio llamada " + deletedCategoryPrize.Capr_category + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.CategoryPrizes.Remove(deletedCategoryPrize);
            ApplicationDbContext.SaveChanges();
            AdminGeneralCategoryPrizeViewModel model = new AdminGeneralCategoryPrizeViewModel { ActualRole = GetActualUserId().Role, ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetCategoryPrizeCompanyId).ToList() };
            return RedirectToAction("CategoryPrize");
        }

        [Authorize]
        public ActionResult UpdateCategoryPrize(int id)
        {

            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            CategoryPrize updatedcategoryPrize = ApplicationDbContext.CategoryPrizes.Find(id);
            TempData["UpdateCategoryPrize"] = "Categoria modificada con éxito";
            AdminGeneralCategoryPrizeViewModel model = new AdminGeneralCategoryPrizeViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetCategoryPrizeCompanyId).ToList(),
                Capr_Id = updatedcategoryPrize.Capr_Id,
                Capr_category = updatedcategoryPrize.Capr_category,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(13);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(298);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = updatedcategoryPrize.Capr_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de seleccionar una categoría de un premio para modificar, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("CategoryPrize", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCategoryPrize(AdminGeneralCategoryPrizeViewModel model)
        {
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            CategoryPrize updatedcategoryPrize = ApplicationDbContext.CategoryPrizes.Find(model.Capr_Id);
            if (ModelState.IsValid)
            {

                TempData["Edit"] = "La categoria se ha Modificado con éxito";
                updatedcategoryPrize.Capr_category = model.Capr_category;
                ApplicationDbContext.SaveChanges();
                model.ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetCategoryPrizeCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(13);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(298);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = updatedcategoryPrize.Capr_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de modificar una categoría de un premio, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("CategoryPrize", model);
            }

            else
            {
                var table = ApplicationDbContext.TableChanges.Find(13);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(298);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = updatedcategoryPrize.Capr_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " no ha ingresado un nombre para modificar una categoría de un premio seleccionado, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Edit"] = "Debe ingresar un nombre de Categoria";
                model.ActualRole = GetActualUserId().Role;
                model.ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetCategoryPrizeCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("categoryModules", model);
            }
        }

        [Authorize]
        public ActionResult Prize()

        {
            var GetGeneralPrizeCompanyId = GetActualUserId().CompanyId;
            AdminGeneralPrizeViewModel model = new AdminGeneralPrizeViewModel { ActualRole = GetActualUserId().Role, ListPrize = ApplicationDbContext.Prizes.Where(c => c.CompanyId == GetGeneralPrizeCompanyId).ToList(), CategoryPrize = GetCategoryPrizeIEnum(), Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(46);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(290);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de gestión de premios, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Prize", model);
        }

        [HttpPost]
        [Authorize]

        public ActionResult SearchPrize(AdminGeneralPrizeViewModel model)
        {
            var GetPrizecompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.searchPrize) || string.IsNullOrEmpty(model.searchPrize))
            {

                model = new AdminGeneralPrizeViewModel { ActualRole = GetActualUserId().Role, ListPrize = ApplicationDbContext.Prizes.Where(m => m.CompanyId == GetPrizecompany).ToList(), CategoryPrize = GetCategoryPrizeIEnum() };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                var table = ApplicationDbContext.TableChanges.Find(46);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(291);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un premio sin ingresar ningún nombre de premio para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("Prize", model);
            }
            else
            {
                var table = ApplicationDbContext.TableChanges.Find(46);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(291);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un premio ingresando un nombre de un premio para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                List<Prize> SearchedPrize = ApplicationDbContext.Prizes.Where(m => m.Priz_Name.Contains(model.searchPrize) && m.CompanyId == GetPrizecompany).ToList();
                model = new AdminGeneralPrizeViewModel { ActualRole = GetActualUserId().Role, ListPrize = SearchedPrize, CategoryPrize = GetCategoryPrizeIEnum() };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("Prize", model);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddPrize(AdminGeneralPrizeViewModel model)
        {
            var GetPrizeCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                var categoryprize = ApplicationDbContext.CategoryPrizes.Find(model.Priz_Capr_Id);
                if (categoryprize != null)
                {
                    Prize Prize = new Prize { Priz_Name = model.Priz_Name, Priz_Description = model.Priz_Description, Priz_RequiredPoints = model.Priz_RequiredPoints, Priz_Quantity = model.Priz_Quantity, Priz_Stateprize = (PRIZESTATE)model.Priz_Stateprize, Prize_Icon = (ICONPRIZE)model.Prize_Icon, Priz_Date = DateTime.Now, CategoryPrize = categoryprize, Company = GetActualUserId().Company };
                    ApplicationDbContext.Prizes.Add(Prize);
                    var prize_name = ApplicationDbContext.CategoryPrizes.Find(model.Priz_Capr_Id);
                    if (model.Priz_Name == null)
                    {
                        TempData["AddMessageError"] = "Se debe Ingresar un premio";
                        var table1 = ApplicationDbContext.TableChanges.Find(46);
                        var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code1 = ApplicationDbContext.CodeLogs.Find(292);
                        var idcompany1 = UserCurrent1.CompanyId;
                        if (idcompany1 != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany1);
                            string ip = IpUser();
                            var idchange1 = new IdChange
                            {
                                IdCh_IdChange = null
                            };
                            ApplicationDbContext.IdChanges.Add(idchange1);
                            ApplicationDbContext.SaveChanges();
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = UserCurrent1,
                                CoLo_Id = code1.CoLo_Id,
                                CodeLogs = code1,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.NoRealizado,
                                TableChange = table1,
                                TaCh_Id = table1.TaCh_Id,
                                IdChange = idchange1,
                                IdCh_Id = idchange1.IdCh_Id,
                                User_Id = UserCurrent1.Id,
                                Log_Description = "El usuario con id: " + UserCurrent1.Id + " no ha ingresado un nombre para crear un premio nuevo, en la compañía con id" + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        return RedirectToAction("Prize", model);
                    }
                    model.ListPrize = ApplicationDbContext.Prizes.Where(c => c.CompanyId == GetPrizeCompanyId).ToList();
                    ApplicationDbContext.CategoryPrizes.Find(model.Priz_Capr_Id);
                    ApplicationDbContext.SaveChanges();
                    var table = ApplicationDbContext.TableChanges.Find(46);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(292);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = Prize.Priz_Id.ToString()
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de agregar un premio nuevo, en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("Prize");
                }
                else
                {
                    TempData["AddMessageError"] = "Se debe Crear una categoria";
                    model.Sesion = GetActualUserId().SesionUser;
                    var table = ApplicationDbContext.TableChanges.Find(46);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(291);
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
                            Log_StateLogs = LOGSTATE.Error,
                            TableChange = table,
                            TaCh_Id = table.TaCh_Id,
                            IdChange = idchange,
                            IdCh_Id = idchange.IdCh_Id,
                            User_Id = UserCurrent.Id,
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " intento crear un premio pero no se tuvo éxito ya que, no hay una categoría de premio creada, en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    return RedirectToAction("Prize", model);
                }
            }
            else
            {
                var table = ApplicationDbContext.TableChanges.Find(46);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(291);
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
                        Log_StateLogs = LOGSTATE.Error,
                        TableChange = table,
                        TaCh_Id = table.TaCh_Id,
                        IdChange = idchange,
                        IdCh_Id = idchange.IdCh_Id,
                        User_Id = UserCurrent.Id,
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " no ha ingresado toda la información necesaria para crear un nuevo premio, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Add"] = "Los campos no pueden estar vacios";
                model.ActualRole = GetActualUserId().Role;
                model.CategoryPrize = GetCategoryPrizeIEnum();
                model.ListPrize = ApplicationDbContext.Prizes.Where(c => c.CompanyId == GetPrizeCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("Prize", model);
            }
        }
        [Authorize]
        public ActionResult DeletePrize(int id)
        {
            TempData["Delete"] = "Premio eliminada";
            var GetPrizeCompanyId = GetActualUserId().CompanyId;
            Prize deletedPrize = ApplicationDbContext.Prizes.Find(id);
            var table1 = ApplicationDbContext.TableChanges.Find(46);
            var UserCurrent1 = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code1 = ApplicationDbContext.CodeLogs.Find(294);
            var idcompany1 = UserCurrent1.CompanyId;
            if (idcompany1 != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany1);
                string ip = IpUser();
                var idchange1 = new IdChange
                {
                    IdCh_IdChange = deletedPrize.Priz_Id.ToString()
                };
                ApplicationDbContext.IdChanges.Add(idchange1);
                ApplicationDbContext.SaveChanges();
                Log logsesiontrue = new Log
                {
                    ApplicationUser = UserCurrent1,
                    CoLo_Id = code1.CoLo_Id,
                    CodeLogs = code1,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.NoRealizado,
                    TableChange = table1,
                    TaCh_Id = table1.TaCh_Id,
                    IdChange = idchange1,
                    IdCh_Id = idchange1.IdCh_Id,
                    User_Id = UserCurrent1.Id,
                    Log_Description = "El usuario con id: " + UserCurrent1.Id + " acaba de eliminar un premio llamado " + deletedPrize.Priz_Name + "y creado el " + deletedPrize.Priz_Date + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            ApplicationDbContext.Prizes.Remove(deletedPrize);
            ApplicationDbContext.SaveChanges();
            AdminGeneralPrizeViewModel model = new AdminGeneralPrizeViewModel { ActualRole = GetActualUserId().Role, ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetPrizeCompanyId).ToList() };
            return RedirectToAction("Prize");
        }

        [Authorize]
        public ActionResult UpdatePrize(int id)
        {

            var GetPrizeCompanyId = GetActualUserId().CompanyId;
            Prize updatedPrize = ApplicationDbContext.Prizes.Find(id);
            TempData["UpdatePrize"] = "Prize Modificada con exito";

            AdminGeneralPrizeViewModel model = new AdminGeneralPrizeViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListPrize = ApplicationDbContext.Prizes.Where(c => c.CompanyId == GetPrizeCompanyId).ToList(),
                Priz_Id = updatedPrize.Priz_Id,
                Priz_Name = updatedPrize.Priz_Name,
                Priz_Description = updatedPrize.Priz_Description,
                Priz_RequiredPoints = updatedPrize.Priz_RequiredPoints,
                Priz_Quantity = updatedPrize.Priz_Quantity,
                Priz_Stateprize = updatedPrize.Priz_Stateprize,
                Prize_Icon = updatedPrize.Prize_Icon,

                CategoryPrize = GetCategoryPrizeIEnum()
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(46);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(293);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = updatedPrize.Priz_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de seleccionar un premio para modificar, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Prize", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdatePrize(AdminGeneralPrizeViewModel model)
        {
            var GetPrizeCompanyId = GetActualUserId().CompanyId;
            Prize updatedPrize = ApplicationDbContext.Prizes.Find(model.Priz_Id);
            if (ModelState.IsValid)
            {

                TempData["Edit"] = "El premio se ha modificado con éxito";
                updatedPrize.Priz_Name = model.Priz_Name;
                updatedPrize.Priz_Description = model.Priz_Description;
                updatedPrize.Priz_RequiredPoints = model.Priz_RequiredPoints;
                updatedPrize.Priz_Quantity = model.Priz_Quantity;
                updatedPrize.Priz_Stateprize = model.Priz_Stateprize;
                updatedPrize.Prize_Icon = model.Prize_Icon;
                updatedPrize.Priz_Date = DateTime.Now;
                updatedPrize.CategoryPrize = ApplicationDbContext.CategoryPrizes.Find(model.Priz_Capr_Id);
                ApplicationDbContext.SaveChanges();
                model.ListCategoryPrize = ApplicationDbContext.CategoryPrizes.Where(c => c.CompanyId == GetPrizeCompanyId).ToList();
                model.Sesion = GetActualUserId().SesionUser;
                var table = ApplicationDbContext.TableChanges.Find(46);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(293);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = updatedPrize.Priz_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de modificar el premio con id " + updatedPrize.Priz_Id + ", en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Prize", model);
            }

            else
            {
                TempData["Edit"] = "Los campos no pueden ser vacios ";
                model.ActualRole = GetActualUserId().Role;
                model.ListPrize = ApplicationDbContext.Prizes.Where(c => c.CompanyId == GetPrizeCompanyId).ToList();
                model.CategoryPrize = GetCategoryPrizeIEnum();
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                var table = ApplicationDbContext.TableChanges.Find(46);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(293);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = updatedPrize.Capr_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " no ha ingresado información para modificar el premio seleccionado, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return View("Prize", model);
            }
        }

        [Authorize]
        public ActionResult ManagerPrize()
        {
            var GetModuleCompanyId = GetActualUserId().CompanyId;
            var GetAdminUserGeneral = GetActualUserId().Id;
            var GetAdminUserGeneralCompany = GetActualUserId().Company.CompanyId;
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            List<ApplicationUser> listUser = ApplicationDbContext.Users.ToList();
            List<Prize> listPrize = ApplicationDbContext.Prizes.Where(a => a.CompanyId == GetAdminUserGeneralCompany && a.Priz_Stateprize == PRIZESTATE.Inactivo).ToList();
            List<Exchange> Listexchange = ApplicationDbContext.Exchanges.ToList();
            List<Prize> listPrize1 = ApplicationDbContext.Prizes.Where(x => x.CompanyId == GetModuleCompanyId).ToList();
            AdminPointsGeneral model = new AdminPointsGeneral { ActualRole = GetActualUserId().Role, prizes = listPrize, Users = listUser, Exchanges = Listexchange, /*TotalPointUser = PointOfUserActuallity*/ };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(23);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(300);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de solicitudes de canje, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View(model);
        }

        [Authorize]
        public ActionResult UpdateManager(int id)
        {

            var GetManagerCompanyId = GetActualUserId().CompanyId;
            Exchange updatedManager = ApplicationDbContext.Exchanges.Find(id);
            TempData["UpdateManager"] = "Premio modificado con éxito";
            List<Exchange> Listexchange = ApplicationDbContext.Exchanges.ToList();
            AdminPointsGeneral model = new AdminPointsGeneral
            {
                ActualRole = GetActualUserId().Role,
                Exch_Id = id,
                Exchanges = Listexchange,
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            var table = ApplicationDbContext.TableChanges.Find(23);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(301);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = updatedManager.Exch_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de seleccionar una solicitud de canje con id ," + updatedManager.Exch_Id + "en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("ManagerPrize", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateManager(AdminPointsGeneral model)
        {
            var GetManagerCompanyId = GetActualUserId().CompanyId;
            List<Prize> listPrize = ApplicationDbContext.Prizes.Where(a => a.CompanyId == GetManagerCompanyId && a.Priz_Stateprize == PRIZESTATE.Inactivo).ToList();
            if (ModelState.IsValid)
            {
                Exchange updatedManager = ApplicationDbContext.Exchanges.Find(model.Exch_Id);
                TempData["Edit"] = "El premio se ha Modificado con éxito";
                updatedManager.StateExchange = model.StateExchange;
                //updatedManager.Exch_date = model.Exch_date;
                updatedManager.Exch_Finishdate = DateTime.Now;
                ApplicationDbContext.SaveChanges();
                if (updatedManager.StateExchange == STATEEXCHANGE.Autorizado)
                {
                    List<Point> listpoints = ApplicationDbContext.Points.Where(x => x.User_Id == updatedManager.User.Id).ToList();
                    var GetAdminUserGeneral = GetActualUserId().Id;
                    var GetAdminUserGeneralCompany = GetActualUserId().Company.CompanyId;
                    Prize prize = ApplicationDbContext.Prizes.Find(updatedManager.Prize.Priz_Id);
                    var GetModuleCompanyId = GetActualUserId().CompanyId;
                    List<Exchange> listexchange = ApplicationDbContext.Exchanges.Where(x => x.Prize.Company.CompanyId == GetModuleCompanyId && x.StateExchange == Enum.STATEEXCHANGE.Pendiente).ToList();
                    //List<Point> listpoints = ApplicationDbContext.Points.ToList();
                    List<Point> listPoint = ApplicationDbContext.Points.Where(x => x.Poin_Id == GetModuleCompanyId).ToList();
                    List<Prize> PrizeOfTheCompany = ApplicationDbContext.Prizes.Where(x => x.CompanyId == GetModuleCompanyId).ToList();
                    ApplicationUser UserActuallity = UserManager.FindById(updatedManager.User.Id);
                    int PointOfUserActuallity = UserActuallity.Point.Sum(x => x.Quantity_Points);
                    //Restar unidades de puntos 
                    var z = UserActuallity.Point.ToList();
                    int ValorProducto = prize.Priz_RequiredPoints;
                    var asunto1 = "Solicitud Canje ";
                    var mensaje1 = "No cuentas con los puntos requeridos para mayor información comunicate a nuestas de atención  0000000-OOOOOOOOOO o escríbenos a nuestro correo electrónico xxxx@gmail.com ";
                    if (z.Sum(x => x.Quantity_Points) >= ValorProducto)
                    {
                        var table = ApplicationDbContext.TableChanges.Find(23);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(301);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = updatedManager.Exch_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de aprobar la solicitudes de canje con id ," + updatedManager.Exch_Id + "en la compañía con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        TempData["Menssages"] = "Transacción se realizo con éxito ";
                        var asunto = "Programa ECO: ¡Tu participación en el sorteo ha sido aprobada!";
                        var mensaje = "Desde ahora te encuentras inscrito en el sorteo mensual de premios. Te deseamos mucha suerte. ¡Sigue participando y hagamos ECO juntos!";
                        var solicitud = new MailMessage();
                        solicitud.Body = mensaje + "<br/>" + "Si tienes alguna duda o solicitud por favor no dudes en contactarnos: univida.cap@saludvidaeps.com";
                        solicitud.Subject = asunto;
                        solicitud.To.Add(UserActuallity.Email);
                        solicitud.IsBodyHtml = true;
                        var smtp = new SmtpClient();
                        smtp.Send(solicitud);
                        int valorItem = 0;
                        foreach (var item in z)
                        {
                            var modificar = ApplicationDbContext.Points.Find(item.Poin_Id);
                            if (ValorProducto > 0)
                            {
                                if (ValorProducto <= item.Quantity_Points)
                                {

                                    //Restar unidades de premios a canjear ManagerPrize
                                    prize.Priz_Quantity = prize.Priz_Quantity - 1;
                                    if (prize.Priz_Quantity < 0)
                                    {
                                        prize.Priz_Quantity = 0;
                                        ApplicationDbContext.SaveChanges();
                                    }

                                    if (prize.Priz_Quantity == 0)
                                    {
                                        prize.Priz_Stateprize = PRIZESTATE.Inactivo;
                                        ApplicationDbContext.SaveChanges();

                                    }

                                    valorItem = item.Quantity_Points - ValorProducto;
                                    modificar.Quantity_Points = valorItem;
                                    ApplicationDbContext.SaveChanges();

                                    break;
                                }
                                else
                                {
                                    if (item.Quantity_Points - ValorProducto <= 0)
                                    {
                                        var x = inverso(ValorProducto, item.Quantity_Points);
                                        ValorProducto = x;
                                        modificar.Quantity_Points = 0;
                                        ApplicationDbContext.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        var table = ApplicationDbContext.TableChanges.Find(23);
                        var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var code = ApplicationDbContext.CodeLogs.Find(301);
                        var idcompany = UserCurrent.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = updatedManager.Exch_Id.ToString()
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento aprobar la solicitudes de canje pero el usuario que realizo la solicitud, no cuenta con los puntos necesarios para realizar el canje con id ," + updatedManager.Exch_Id + "en la compañía con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        TempData["Menssages"] = "El usuario no tiene los puntos suficientes para realizar este canje";
                        var solicitud2 = new MailMessage();
                        solicitud2.Subject = asunto1;
                        solicitud2.Body = UserActuallity.FirstName + "<br/>" + mensaje1 + "<br/>" + "Sigue participando no te desanimes";
                        solicitud2.To.Add(UserActuallity.Email);
                        solicitud2.IsBodyHtml = true;
                        var smtp2 = new SmtpClient();
                        smtp2.Send(solicitud2);
                    }
                }
                model.Sesion = GetActualUserId().SesionUser;
                return RedirectToAction("ManagerPrize", model);
            }
            else
            {
                TempData["Menssages"] = "Los campos no pueden ser vacios ";
                model.ActualRole = GetActualUserId().Role;
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("ManagerPrize", model);
            }
        }
        public int inverso(int mayor, int menor)
        {
            int z = mayor - menor;
            return z;
        }


        [Authorize]
        public ActionResult ReportUserIndividual(int? page)
        {
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId && x.Role == ROLES.Usuario).ToList().ToPagedList(page ?? 1, 6);
            AdminReportsUserIndividual model = new AdminReportsUserIndividual { ActualRole = GetActualUserId().Role, UserEnrollment = ListOfUser };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(284);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de reportes de usuario individual, en la compañía con id " + company.CompanyId,
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
        public ActionResult SeachUser(AdminReportsUserIndividual model, int? page)
        {


            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchUser) || string.IsNullOrEmpty(model.SearchUser))
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(285);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario sin ingresar ningún nombre de usuario para generar un reporte individual, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ReportUserIndividual");
            }
            else
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(285);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario en específico para generar un reporte individual, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser)) && x.Role == ROLES.Usuario).ToList().ToPagedList(page ?? 1, 6);
                model = new AdminReportsUserIndividual { ActualRole = GetActualUserId().Role, UserEnrollment = ListOfUser };
                model.Logo = GetUrlLogo();
                model.Sesion = GetActualUserId().SesionUser;
                return View("ReportUserIndividual", model);
            }
        }

        [Authorize]
        public ActionResult ReportUserCertification(int? page)
        {
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId && x.Role == ROLES.Usuario).ToList().ToPagedList(page ?? 1, 6);
            AdminReportsUserIndividual model = new AdminReportsUserIndividual { ActualRole = GetActualUserId().Role, UserEnrollment = ListOfUser };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(287);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de reportes de usuarios certificados, en la compañía con id " + company.CompanyId,
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
        public ActionResult SeachUserCertification(AdminReportsUserIndividual model, int? page)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchUser) || string.IsNullOrEmpty(model.SearchUser))
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(287);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario sin ingresar ningún nombre de usuario para generar un reporte individual de certificación, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("ReportUserCertification");
            }
            else
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(287);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de un usuario en específico para generar un reporte de certificación individual, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser)) && x.Role == ROLES.Usuario).ToList().ToPagedList(page ?? 1, 6);
                model = new AdminReportsUserIndividual { ActualRole = GetActualUserId().Role, UserEnrollment = ListOfUser };
                model.Logo = GetUrlLogo();
                model.Sesion = GetActualUserId().SesionUser;
                return View("ReportUserCertification", model);
            }
        }

        [Authorize]
        public ActionResult ReportUserLogs(int? page)
        {
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId).ToList().ToPagedList(page ?? 1, 6);
            AdminReportsUserLogs model = new AdminReportsUserLogs { ActualRole = GetActualUserId().Role, UserLogs = ListOfUser };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SeachUserLogs(AdminReportsUserLogs model, int? page)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchUser) || string.IsNullOrEmpty(model.SearchUser))
            {

                return RedirectToAction("ReportUserLogs");
            }
            else
            {

                IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser))).ToList().ToPagedList(page ?? 1, 6);
                model = new AdminReportsUserLogs { ActualRole = GetActualUserId().Role, UserLogs = ListOfUser };
                model.Sesion = GetActualUserId().SesionUser;
                return View("ReportUserLogs", model);
            }
        }
        [Authorize]
        public ActionResult FilterLogs(string id, int? page)
        {
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId).ToList().ToPagedList(page ?? 1, 6);
            var user = ApplicationDbContext.Users.Where(x => x.Id == id && x.CompanyId == GetModuleCompanyId).ToList();
            AdminReportsUserLogs model = new AdminReportsUserLogs { ActualRole = GetActualUserId().Role, UserLogs = ListOfUser };
            model.Logo = GetUrlLogo();
            model.Sesion = GetActualUserId().SesionUser;
            model.UserSelected = user;
            model.userId = id;
            TempData["Form"] = "Activar";
            return View("ReportUserLogs", model);
        }

        public ActionResult ExportToExcelLogs(AdminReportsUserLogs model)
        {
            var UserActuallity = UserManager.FindById(model.userId);
            DateTime finish = new DateTime();
            switch (model.Time)
            {
                case VIGENCIA.Dias:
                    finish = DateTime.Now.AddDays(-model.Number);
                    break;
                case VIGENCIA.Meses:
                    finish = DateTime.Now.AddMonths(-model.Number);
                    break;
                case VIGENCIA.Años:
                    finish = DateTime.Now.AddYears(-model.Number);
                    break;
                default:
                    break;
            }
            DateTime a = finish;

            var AM = (from k in ApplicationDbContext.Logs.Where(x => x.User_Id == UserActuallity.Id && x.Log_Date <= DateTime.Now && x.Log_Date >= a)
                      select new
                      {

                          Codigo = k.CoLo_Id,
                          Nombre = k.ApplicationUser.FirstName + " " + k.ApplicationUser.LastName,
                          Usuario = k.ApplicationUser.UserName,
                          Tipo_de_Usuario = k.ApplicationUser.Role,
                          Correo = k.ApplicationUser.Email,
                          Tabla_Afectada = k.TableChange.TaCh_TableName,
                          Id_Afectado = k.IdChange.IdCh_IdChange,
                          Acción_Realizada = k.CodeLogs.CoLo_Description,
                          Descripción = k.Log_Description,
                          Fecha_Evento = k.Log_Date,
                          Ip_Ingreso = k.Log_Ip,
                          Primer_Ingreso = k.ApplicationUser.firstAccess,
                          UltimoIngreso = k.ApplicationUser.lastAccess,
                          Compañia = k.ApplicationUser.Company.CompanyName
                      }).OrderByDescending(x => x.Fecha_Evento).ToList();
            if (AM.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = AM;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE LOGS USUARIO INDIVIDIAL.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE LOGS DE USUARIO </H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write("<h3>Datos Usuario</h3>");
                Response.Output.Write("UserName: " + UserActuallity.UserName + "<br>");
                Response.Output.Write("Cedula: " + UserActuallity.Document + "<br>");
                Response.Output.Write("Nombre: " + UserActuallity.FirstName + UserActuallity.LastName + "<br>");
                Response.Output.Write("Pais: " + UserActuallity.Country + "<br>");
                Response.Output.Write("Estado: " + UserActuallity.StateUser + "<br>");
                Response.Output.Write("Tipo de Usuario: " + UserActuallity.Role + "<br>");
                Response.Output.Write("<center>" + objStringWriter.ToString() + "</center>");
                Response.Flush();
                Response.End();
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }

        public ActionResult Report()

        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            AdminReports model = new AdminReports
            {
                Logo = GetUrlLogo(),
                user = user
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(274);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de gestión de reportes, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Report", model);
        }
        public ActionResult Reports()

        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            AdminReports model = new AdminReports
            {
                Logo = GetUrlLogo(),
                user = user
            };
            model.Sesion = GetActualUserId().SesionUser;
            var table = ApplicationDbContext.TableChanges.Find(72);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(274);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de gestión de reportes, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            return View("Reports", model);
        }
        public ActionResult ExportToExcel()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            AdminReports model = new AdminReports
            {
                Logo = GetUrlLogo()
            };
            var List = (
                            from u in ApplicationDbContext.Users
                            from co in ApplicationDbContext.Companies
                            from po in ApplicationDbContext.Position
                            where u.PositionId == po.Posi_id
                            from a in ApplicationDbContext.Areas
                            where u.AreaId == a.AreaId
                            from lo in ApplicationDbContext.Location
                            where u.LocationId == lo.Loca_Id
                            from c in ApplicationDbContext.City
                            where u.CityId == c.City_Id
                            from m in ApplicationDbContext.Enrollments
                            where u.Id == m.User_Id
                            from ad in ApplicationDbContext.AdvanceCourses/*.Where(X => X.User_Id == u.Id).ToList()*/
                            where /*u.Id == ad.User_Id &&*/ ad.Enro_Id == m.Enro_Id
                            where u.CompanyId == UserActuallity.CompanyId
                            where u.CompanyId == co.CompanyId
                            select new
                            {
                                UserName = u.UserName,
                                Documento = u.Document,
                                Nombre = u.FirstName + " " + u.LastName,
                                Cargo = po.Posi_Description,
                                Area = a.AreaName,
                                Ubicacion = lo.Loca_Description,
                                Pais = u.Country,
                                Ciudad = c.City_Name,
                                Estado = u.Enable,
                                Tipo_de_Usuario = u.Role,
                                Curso = m.Module.Modu_Name,
                                Esperado = "100",
                                Avance = ad.AdCo_ScoreObtanied,
                                Úlmimo_Ingreso = u.lastAccess
                            }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=PROGRESO DE USUARIOS POR CURSO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE PROGRESO DE USUARIOS POR CURSO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Progreso de usuario por curso");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel1()
        {
            AdminReports model = new AdminReports
            {
                Logo = GetUrlLogo()
            };
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var Users = (from u in ApplicationDbContext.Users
                         from co in ApplicationDbContext.Companies
                         from a in ApplicationDbContext.Areas
                         where u.AreaId == a.AreaId
                         from lo in ApplicationDbContext.Location
                         where u.LocationId == lo.Loca_Id
                         from c in ApplicationDbContext.City
                         where u.CityId == c.City_Id
                         from po in ApplicationDbContext.Position
                         where u.PositionId == po.Posi_id
                         where u.CompanyId == UserActuallity.CompanyId
                         where u.CompanyId == co.CompanyId
                         select new
                         {
                             UserName = u.UserName,
                             Documento = u.Document,
                             Nombre = u.FirstName + " " + u.LastName,
                             Cargo = po.Posi_Description,
                             Area = a.AreaName,
                             Ubicacion = lo.Loca_Description,
                             Pais = u.Country,
                             Ciudad = c.City_Name,
                             Estado = u.Enable,
                             Tipo_de_Usuario = u.Role,
                             Compañia = co.CompanyName,
                             Primer_Acceso = u.firstAccess,
                             Ultimo_Acceso = u.lastAccess,
                             Terminos_y_Condiciones = u.TermsandConditions,
                         }).ToList();
            if (Users.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = Users;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Accesos Usuarios.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE ACCESOS USUARIOS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Usuarios  Registrados");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }

            else

            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel2()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            var List = (
                from u in ApplicationDbContext.Users
                from co in ApplicationDbContext.Companies
                from po in ApplicationDbContext.Position
                where u.PositionId == po.Posi_id
                from a in ApplicationDbContext.Areas
                where u.AreaId == a.AreaId
                from lo in ApplicationDbContext.Location
                where u.LocationId == lo.Loca_Id
                from c in ApplicationDbContext.City
                where u.CityId == c.City_Id
                from cer in ApplicationDbContext.Certifications
                where cer.User_Id == u.Id
                where u.CompanyId == UserActuallity.CompanyId
                where u.CompanyId == co.CompanyId
                select new
                {
                    UserName = u.UserName,
                    Documento = u.Document,
                    Nombre = u.FirstName + " " + u.LastName,
                    Cargo = po.Posi_Description,
                    Area = a.AreaName,
                    Ubicacion = lo.Loca_Description,
                    Pais = u.Country,
                    Ciudad = c.City_Name,
                    Estado = u.Enable,
                    Tipo_de_Usuario = u.Role,
                    Nombre_Curso = cer.Enrollment.Module.Modu_Name,
                    Tipo_Curso = cer.Enrollment.Module.Modu_TypeOfModule,
                    Úlmimo_Ingreso = u.lastAccess,
                    Fecha_certificación = cer.Cert_Date
                }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE CERTIFICADO POR CURSO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE CERTIFICADO POR CURSO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Reporte Certificacion ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel3()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var List = (from u in ApplicationDbContext.Users
                        from co in ApplicationDbContext.Companies
                        from po in ApplicationDbContext.Position
                        where u.PositionId == po.Posi_id
                        from a in ApplicationDbContext.Areas
                        where u.AreaId == a.AreaId
                        from lo in ApplicationDbContext.Location
                        where u.LocationId == lo.Loca_Id
                        from c in ApplicationDbContext.City
                        where u.CityId == c.City_Id
                        from m in ApplicationDbContext.Enrollments
                        where u.Id == m.User_Id
                        from t in ApplicationDbContext.TopicsCourses
                        where t.Modu_Id == m.Modu_Id
                        where u.CompanyId == UserActuallity.CompanyId
                        where u.CompanyId == co.CompanyId
                        select new
                        {
                            UserName = u.UserName,
                            Documento = u.Document,
                            Nombre = u.FirstName + " " + u.LastName,
                            Cargo = po.Posi_Description,
                            Area = a.AreaName,
                            Ubicacion = lo.Loca_Description,
                            Pais = u.Country,
                            Ciudad = c.City_Name,
                            Estado = u.Enable,
                            Tipo_de_Usuario = u.Role,
                            Curso = m.Module.Modu_Name,
                            Tema = t.ToCo_Name,
                            Puntos_Curso = m.Module.Modu_Points,
                            Compañia = co.CompanyName,
                        }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=TEMAS POR USUARIO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE TEMAS POR USUARIO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Temas por usuario ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel4()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var List = (from u in ApplicationDbContext.Users
                        from po in ApplicationDbContext.Position
                        from co in ApplicationDbContext.Companies
                        where u.PositionId == po.Posi_id
                        from a in ApplicationDbContext.Areas
                        where u.AreaId == a.AreaId
                        from lo in ApplicationDbContext.Location
                        where u.LocationId == lo.Loca_Id
                        from c in ApplicationDbContext.City
                        where u.CityId == c.City_Id
                        from m in ApplicationDbContext.Enrollments
                        where u.Id == m.User_Id
                        from t in ApplicationDbContext.TopicsCourses
                        where t.Modu_Id == m.Modu_Id
                        where u.CompanyId == UserActuallity.CompanyId
                        where u.CompanyId == co.CompanyId
                        select new
                        {
                            UserName = u.UserName,
                            Documento = u.Document,
                            Nombre = u.FirstName + " " + u.LastName,
                            Cargo = po.Posi_Description,
                            Area = a.AreaName,
                            Ubicacion = lo.Loca_Description,
                            Pais = u.Country,
                            Ciudad = c.City_Name,
                            Estado = u.Enable,
                            Tipo_de_Usuario = u.Role,
                            Curso = m.Module.Modu_Name,
                            Tema = t.ToCo_Name,
                            Intentos_por_tema = t.ToCo_Attempt
                        }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=CONSOLIDADO POR CURSOS USUARIO .xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE  CURSOS POR USUARIO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Cursos por usuarios ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel5(int idCurso)
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.Modu_Id == idCurso).FirstOrDefault();
            var l2 = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).ToList();

            var List = (
                            from u in ApplicationDbContext.Users
                            from co in ApplicationDbContext.Companies
                            from po in ApplicationDbContext.Position
                            where u.PositionId == po.Posi_id
                            from a in ApplicationDbContext.Areas
                            where u.AreaId == a.AreaId
                            from lo in ApplicationDbContext.Location
                            where u.LocationId == lo.Loca_Id
                            from c in ApplicationDbContext.City
                            where u.CityId == c.City_Id
                            from m in ApplicationDbContext.Enrollments.Where(x => x.Modu_Id == l.Modu_Id)
                            where u.Id == m.User_Id
                            where u.CompanyId == UserActuallity.CompanyId
                            where u.CompanyId == co.CompanyId
                            select new
                            {
                                UserName = u.UserName,
                                Documento = u.Document,
                                Nombre = u.FirstName + " " + u.LastName,
                                Cargo = po.Posi_Description,
                                Area = a.AreaName,
                                Ubicacion = lo.Loca_Description,
                                Pais = u.Country,
                                Ciudad = c.City_Name,
                                Estado = u.Enable,
                                Tipo_de_Usuario = u.Role,
                                Curso = m.Module.Modu_Name,
                                Matriculados = m.Enro_StateEnrollment,
                                Tipo_Curso = m.Module.Modu_TypeOfModule
                            }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=USUARIOS MATRICULADOS POR CURSO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE  USUARIOS MATRICULADOS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Usuarios Matriculados ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel6()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var List = (
                            from u in ApplicationDbContext.Users
                            from co in ApplicationDbContext.Companies
                            from po in ApplicationDbContext.Position
                            where u.PositionId == po.Posi_id
                            from a in ApplicationDbContext.Areas
                            where u.AreaId == a.AreaId
                            from lo in ApplicationDbContext.Location
                            where u.LocationId == lo.Loca_Id
                            from c in ApplicationDbContext.City
                            where u.CityId == c.City_Id
                            from p in ApplicationDbContext.Points
                            where p.User_Id == u.Id && p.User_Id == p.User_Id
                            where u.CompanyId == UserActuallity.CompanyId
                            where u.CompanyId == co.CompanyId
                            select new
                            {
                                UserName = u.UserName,
                                Documento = u.Document,
                                Nombre = u.FirstName + " " + u.LastName,
                                Correo = u.Email,
                                Cargo = po.Posi_Description,
                                Area = a.AreaName,
                                Ubicacion = lo.Loca_Description,
                                Pais = u.Country,
                                Ciudad = c.City_Name,
                                Estado = u.Enable,
                                Tipo_de_Usuario = u.Role,
                                Tipo_punto = p.TypePoint.Poin_TypePoints,
                                Categoría = p.TypePoint.TyPo_Description,
                                Puntos_Obtenidos = p.Quantity_Points,
                            }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=PUNTOS POR USUARIO CURSO .xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE PUNTOS POR USUARIO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Puntos por usuario ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel7()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var List = (
                             from u in ApplicationDbContext.Users
                             from co in ApplicationDbContext.Companies
                             from po in ApplicationDbContext.Position
                             where u.PositionId == po.Posi_id
                             from a in ApplicationDbContext.Areas
                             where u.AreaId == a.AreaId
                             from lo in ApplicationDbContext.Location
                             where u.LocationId == lo.Loca_Id
                             from c in ApplicationDbContext.City
                             where u.CityId == c.City_Id
                             from com in ApplicationDbContext.PointsComments
                             where com.User_Id == u.Id
                             from comp in ApplicationDbContext.PointManagerCategory
                             where comp.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == co.CompanyId
                             select new
                             {
                                 UserName = u.UserName,
                                 Documento = u.Document,
                                 Nombre = u.FirstName + " " + u.LastName,
                                 Cargo = po.Posi_Description,
                                 Area = a.AreaName,
                                 Ubicacion = lo.Loca_Description,
                                 Pais = u.Country,
                                 Ciudad = c.City_Name,
                                 Estado = u.Enable,
                                 Tipo_de_Usuario = u.Role,
                                 Articulo = com.Comments.Article.Arti_Name,
                                 Fecha_Comentario = com.PoCo_Date,
                                 Estado_comentario = com.Comments.Comm_StateComment,
                                 Titulo_Comentario = com.Comments.comm_Title,
                                 Compañia = co.CompanyName,
                                 Puntos_Comentario = comp.PoMaCa_Periodical
                             }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE COMENTARIOS-PERIODICO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE COMENTARIOS-PERIODICO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Comentarios periodico ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel8()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var List = (
                             from u in ApplicationDbContext.Users
                             from co in ApplicationDbContext.Companies
                             from po in ApplicationDbContext.Position
                             where u.PositionId == po.Posi_id
                             from a in ApplicationDbContext.Areas
                             where u.AreaId == a.AreaId
                             from lo in ApplicationDbContext.Location
                             where u.LocationId == lo.Loca_Id
                             from c in ApplicationDbContext.City
                             where u.CityId == c.City_Id
                             from com in ApplicationDbContext.BetterPractices
                             where com.User_Id == u.Id
                             from peri in ApplicationDbContext.PointManagerCategory
                             where peri.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == co.CompanyId
                             select new
                             {
                                 UserName = u.UserName,
                                 Documento = u.Document,
                                 Nombre = u.FirstName + " " + u.LastName,
                                 Cargo = po.Posi_Description,
                                 Area = a.AreaName,
                                 Ubicación = lo.Loca_Description,
                                 Pais = u.Country,
                                 Ciudad = c.City_Name,
                                 Estado = u.Enable,
                                 Tipo_de_Usuario = u.Role,
                                 Tema = com.Module.Modu_Name,
                                 Tipo_Curso = com.Module.Modu_TypeOfModule,
                                 Titulo_Comentario = com.BePr_TiTle,
                                 Fecha_Creación_Comentario = com.BePr_InitDate,
                                 Fecha_Aprobación_comentario = com.BePr_FinishDate,
                                 Estado_Comentario = com.BePr_StateBetterpractice,
                                 Compañia = co.CompanyName,
                                 Puntos_Comentario = peri.PoMaCa_course
                             }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE COMENTARIOS-B.PRACTICAS.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE COMENTARIOS BUENAS PRACTICAS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Comentarios Buenas Practicas ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel9()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var List = (
                             from u in ApplicationDbContext.Users
                             from co in ApplicationDbContext.Companies
                             from po in ApplicationDbContext.Position
                             where u.PositionId == po.Posi_id
                             from a in ApplicationDbContext.Areas
                             where u.AreaId == a.AreaId
                             from lo in ApplicationDbContext.Location
                             where u.LocationId == lo.Loca_Id
                             from c in ApplicationDbContext.City
                             where u.CityId == c.City_Id
                             from com in ApplicationDbContext.Improvements
                             where com.User_Id == u.Id
                             from peri in ApplicationDbContext.PointManagerCategory
                             where peri.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == co.CompanyId
                             select new
                             {
                                 UserName = u.UserName,
                                 Documento = u.Document,
                                 Nombre = u.FirstName + " " + u.LastName,
                                 Cargo = po.Posi_Description,
                                 Area = a.AreaName,
                                 Ubicación = lo.Loca_Description,
                                 Pais = u.Country,
                                 Ciudad = c.City_Name,
                                 Estado = u.Enable,
                                 Tipo_de_Usuario = u.Role,
                                 Tema = com.Module.Modu_Name,
                                 Tipo_Curso = com.Module.Modu_TypeOfModule,
                                 Titulo_Comentario = com.Impr_Title,
                                 Fecha_Creación_Comentario = com.Impr_InitDate,
                                 Fecha_Aprobación_comentario = com.Impr_FinishDate,
                                 Estado_Comentario = com.Impr_StateImprovement,
                                 Compañia = co.CompanyName,
                                 Puntos_Comentario = peri.PoMaCa_Improvements,
                             }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE COMENTARIOS-MEJORAS.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE COMENTARIOS-MEJORAS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Comentarios Mejoras ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel20()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var Users = (from u in ApplicationDbContext.Users
                         from co in ApplicationDbContext.Companies
                         where u.CompanyId == UserActuallity.CompanyId
                         where u.CompanyId == co.CompanyId
                         select new
                         {
                             UserName = u.UserName,
                             Documento = u.Document,
                             Nombre = u.FirstName + " " + u.LastName,
                             Pais = u.Country,
                             Estado = u.Enable,
                             Tipo_de_Usuario = u.Role,
                             Compañia = co.CompanyName,
                         }).ToList();
            if (Users.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = Users;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Usuarios Regitrados.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE USUARIOS REGISTRADOS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Total Usuarios  Registrados");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel21()
        {
            AdminReports model = new AdminReports
            {
                Logo = GetUrlLogo()
            };
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var Users = (from u in ApplicationDbContext.Users
                         from co in ApplicationDbContext.Companies
                         from a in ApplicationDbContext.Areas
                         where u.AreaId == a.AreaId
                         from lo in ApplicationDbContext.Location
                         where u.LocationId == lo.Loca_Id
                         from c in ApplicationDbContext.City
                         where u.CityId == c.City_Id
                         from po in ApplicationDbContext.Position
                         where u.PositionId == po.Posi_id
                         where u.CompanyId == UserActuallity.CompanyId
                         where u.CompanyId == co.CompanyId
                         where u.firstAccess == null
                         select new
                         {
                             UserName = u.UserName,
                             Documento = u.Document,
                             Nombre = u.FirstName + " " + u.LastName,
                             Cargo = po.Posi_Description,
                             Area = a.AreaName,
                             Ubicacion = lo.Loca_Description,
                             Pais = u.Country,
                             Ciudad = c.City_Name,
                             Estado = u.Enable,
                             Tipo_de_Usuario = u.Role,
                             Compañia = co.CompanyName,
                             Acceso = "No registra accesos",
                         }).ToList();
            if (Users.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = Users;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Usuarios Regitrados.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE USUARIOS REGISTRADOS QUE NO HAN INGRESADO A LA PLATAFORMA </H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Usuarios  Registrados");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }

            else

            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel22(string id)
        {
            var UserActuallity = UserManager.FindById(id);
            var AM = (from k in ApplicationDbContext.AdvanceUsers.Where(x => x.User_Id == UserActuallity.Id)
                      select new
                      {
                          Tipodecurso = k.TopicsCourse.Module.Modu_TypeOfModule,
                          NombreCurso = k.TopicsCourse.Module.Modu_Name,
                          Contenido = k.TopicsCourse.Module.Modu_Description,
                          Tema = k.TopicsCourse.ToCo_Name,
                          Esperado = "100%",
                          Obtenido = "100%",
                      }).OrderBy(x => x.Contenido).ToList();
            AdminReports model = new AdminReports
            {
                Logo = GetUrlLogo()
            };

            if (AM.Count != 0)
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(286);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de descargar un reporte por progreso por usuario individual, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                var gv = new GridView();
                gv.DataSource = AM;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=PROGRESO DE USUARIOS POR CURSO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE POR USUARIO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write("<h3>Datos Usuario</h3>");
                Response.Output.Write("UserName: " + UserActuallity.UserName + "<br>");
                Response.Output.Write("Cedula: " + UserActuallity.Document + "<br>");
                Response.Output.Write("Nombre: " + UserActuallity.FirstName + UserActuallity.LastName + "<br>");
                Response.Output.Write("Cargo: " + UserActuallity.Position.Posi_Description + "<br>");
                Response.Output.Write("Area: " + UserActuallity.Area.AreaName + "<br>");
                Response.Output.Write("Ubicacion: " + UserActuallity.Location.Loca_Description + "<br>");
                Response.Output.Write("Ciudad: " + UserActuallity.City.City_Name + "<br>");
                Response.Output.Write("Pais: " + UserActuallity.Country + "<br>");
                Response.Output.Write("Estado: " + UserActuallity.StateUser + "<br>");
                Response.Output.Write("Tipo de Usuario: " + UserActuallity.Role + "<br>");
                Response.Output.Write("<center>" + objStringWriter.ToString() + "</center>");
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(286);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento  descargar un reporte de progreso por usuarios individual, pero hace falta información, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExcel23(string id)
        {
            var UserActuallity = UserManager.FindById(id);
            var AM = (from k in ApplicationDbContext.Certifications.Where(x => x.User_Id == UserActuallity.Id)
                      select new
                      {
                          Tipodecurso = k.Enrollment.Module.Modu_TypeOfModule,
                          NombreCurso = k.Enrollment.Module.Modu_Name,
                          Contenido = k.Enrollment.Module.Modu_Description,
                          Esperado = "100%",
                          Obtenido = "100%",
                          UltimoIngreso = k.ApplicationUser.lastAccess,
                          FechaCertificación = k.Cert_Date
                      }).OrderBy(x => x.Contenido).ToList();
            AdminReports model = new AdminReports
            {
                Logo = GetUrlLogo()
            };

            if (AM.Count != 0)
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(289);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de descargar un reporte de certificado por usuario individual, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                var gv = new GridView();
                gv.DataSource = AM;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=CERTIFICACIÓN USUARIO INDIVIDIAL.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE CERTIFICACION DE USUARIO </H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write("<h3>Datos Usuario</h3>");
                Response.Output.Write("UserName: " + UserActuallity.UserName + "<br>");
                Response.Output.Write("Cedula: " + UserActuallity.Document + "<br>");
                Response.Output.Write("Nombre: " + UserActuallity.FirstName + UserActuallity.LastName + "<br>");
                Response.Output.Write("Cargo: " + UserActuallity.Position.Posi_Description + "<br>");
                Response.Output.Write("Area: " + UserActuallity.Area.AreaName + "<br>");
                Response.Output.Write("Ubicacion: " + UserActuallity.Location.Loca_Description + "<br>");
                Response.Output.Write("Ciudad: " + UserActuallity.City.City_Name + "<br>");
                Response.Output.Write("Pais: " + UserActuallity.Country + "<br>");
                Response.Output.Write("Estado: " + UserActuallity.StateUser + "<br>");
                Response.Output.Write("Tipo de Usuario: " + UserActuallity.Role + "<br>");
                Response.Output.Write("<center>" + objStringWriter.ToString() + "</center>");
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(289);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento  descargar un reporte de certificado por usuario individual, pero hace falta información, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report");
        }
        public ActionResult ExportToExce24()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Users.Where(x => x.Id == UserActuallity.Id).ToList();
            var List = (
                             from u in ApplicationDbContext.Users
                             from co in ApplicationDbContext.Companies
                             from po in ApplicationDbContext.Position
                             where u.PositionId == po.Posi_id
                             from a in ApplicationDbContext.Areas
                             where u.AreaId == a.AreaId
                             from lo in ApplicationDbContext.Location
                             where u.LocationId == lo.Loca_Id
                             from c in ApplicationDbContext.City
                             where u.CityId == c.City_Id
                             from com in ApplicationDbContext.PointsComments
                             where com.User_Id == u.Id
                             from comp in ApplicationDbContext.PointManagerCategory
                             where comp.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == co.CompanyId
                             select new
                             {
                                 UserName = u.UserName,
                                 Documento = u.Document,
                                 Nombre = u.FirstName + " " + u.LastName,
                                 Cargo = po.Posi_Description,
                                 Area = a.AreaName,
                                 Ubicacion = lo.Loca_Description,
                                 Pais = u.Country,
                                 Ciudad = c.City_Name,
                                 Estado = u.Enable,
                                 Tipo_de_Usuario = u.Role,
                                 Articulo = com.Comments.Article.Arti_Name,
                                 Fecha_Comentario = com.PoCo_Date,
                                 Estado_comentario = com.Comments.Comm_StateComment,
                                 Titulo_Comentario = com.Comments.comm_Title,
                                 Compañia = co.CompanyName,
                                 Puntos_Comentario = comp.PoMaCa_Periodical
                             }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE COMENTARIOS-PERIODICO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE COMENTARIOS-PERIODICO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Comentarios periodico ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ExportToExcel25()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            var List = (
                from u in ApplicationDbContext.Users
                from co in ApplicationDbContext.Companies
                from po in ApplicationDbContext.Position
                where u.PositionId == po.Posi_id
                from a in ApplicationDbContext.Areas
                where u.AreaId == a.AreaId
                from lo in ApplicationDbContext.Location
                where u.LocationId == lo.Loca_Id
                from c in ApplicationDbContext.City
                where u.CityId == c.City_Id
                from cer in ApplicationDbContext.Certifications.Where(x => x.Enrollment.Modu_Id == l.Modu_Id)
                where cer.User_Id == u.Id
                where u.CompanyId == UserActuallity.CompanyId
                where u.CompanyId == co.CompanyId
                select new
                {
                    UserName = u.UserName,
                    Documento = u.Document,
                    Nombre = u.FirstName + " " + u.LastName,
                    Cargo = po.Posi_Description,
                    Area = a.AreaName,
                    Ubicacion = lo.Loca_Description,
                    Pais = u.Country,
                    Ciudad = c.City_Name,
                    Estado = u.Enable,
                    Tipo_de_Usuario = u.Role,
                    Nombre_Curso = cer.Enrollment.Module.Modu_Name,
                    Tipo_Curso = cer.Enrollment.Module.Modu_TypeOfModule,
                    Úlmimo_Ingreso = u.lastAccess,
                    Fecha_certificación = cer.Cert_Date
                }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE CERTIFICADO POR CURSO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE CERTIFICADO POR CURSO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Reporte Certificacion ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Reports");
        }
        public ActionResult ExportToExcel26()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            var List = (
                            from u in ApplicationDbContext.Users
                            from co in ApplicationDbContext.Companies
                            from po in ApplicationDbContext.Position
                            where u.PositionId == po.Posi_id
                            from a in ApplicationDbContext.Areas
                            where u.AreaId == a.AreaId
                            from lo in ApplicationDbContext.Location
                            where u.LocationId == lo.Loca_Id
                            from c in ApplicationDbContext.City
                            where u.CityId == c.City_Id
                            from m in ApplicationDbContext.Enrollments.Where(x => x.Modu_Id == l.Modu_Id)
                            where u.Id == m.User_Id
                            where u.CompanyId == UserActuallity.CompanyId
                            where u.CompanyId == co.CompanyId
                            select new
                            {
                                UserName = u.UserName,
                                Documento = u.Document,
                                Nombre = u.FirstName + " " + u.LastName,
                                Correo = u.Email,
                                Cargo = po.Posi_Description,
                                Area = a.AreaName,
                                Ubicacion = lo.Loca_Description,
                                Pais = u.Country,
                                Ciudad = c.City_Name,
                                Estado = u.Enable,
                                Tipo_de_Usuario = u.Role,
                                Curso = m.Module.Modu_Name,
                                Matriculados = m.Enro_StateEnrollment,
                                Tipo_Curso = m.Module.Modu_TypeOfModule,
                                Primer_Ingreso = u.firstAccess,
                                Ultimo_Ingreso = u.lastAccess

                            }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=USUARIOS MATRICULADOS POR CURSO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE  USUARIOS MATRICULADOS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Usuarios Matriculados ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Reports");
        }
        public ActionResult ExportToExcel27()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            AdminReports model = new AdminReports
            {
                Logo = GetUrlLogo()
            };
            var List = (
                            from u in ApplicationDbContext.Users
                            from co in ApplicationDbContext.Companies
                            from po in ApplicationDbContext.Position
                            where u.PositionId == po.Posi_id
                            from a in ApplicationDbContext.Areas
                            where u.AreaId == a.AreaId
                            from lo in ApplicationDbContext.Location
                            where u.LocationId == lo.Loca_Id
                            from c in ApplicationDbContext.City
                            where u.CityId == c.City_Id
                            from m in ApplicationDbContext.Enrollments.Where(x => x.Modu_Id == l.Modu_Id)
                            where u.Id == m.User_Id
                            from ad in ApplicationDbContext.AdvanceCourses/*.Where(X => X.User_Id == u.Id).ToList()*/
                            where /*u.Id == ad.User_Id &&*/ ad.Enro_Id == m.Enro_Id
                            where u.CompanyId == UserActuallity.CompanyId
                            where u.CompanyId == co.CompanyId
                            select new
                            {
                                UserName = u.UserName,
                                Documento = u.Document,
                                Nombre = u.FirstName + " " + u.LastName,
                                Cargo = po.Posi_Description,
                                Area = a.AreaName,
                                Ubicacion = lo.Loca_Description,
                                Pais = u.Country,
                                Ciudad = c.City_Name,
                                Estado = u.Enable,
                                Tipo_de_Usuario = u.Role,
                                Curso = m.Module.Modu_Name,
                                Esperado = "100",
                                Avance = ad.AdCo_ScoreObtanied,
                                Úlmimo_Ingreso = u.lastAccess
                            }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=PROGRESO DE USUARIOS POR CURSO.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE PROGRESO DE USUARIOS POR CURSO</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Progreso de usuario por curso");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Reports");
        }
        public ActionResult ExportToExcel28()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            var List = (
                             from u in ApplicationDbContext.Users
                             from co in ApplicationDbContext.Companies
                             from po in ApplicationDbContext.Position
                             where u.PositionId == po.Posi_id
                             from a in ApplicationDbContext.Areas
                             where u.AreaId == a.AreaId
                             from lo in ApplicationDbContext.Location
                             where u.LocationId == lo.Loca_Id
                             from c in ApplicationDbContext.City
                             where u.CityId == c.City_Id
                             from com in ApplicationDbContext.BetterPractices.Where(x => x.Modu_Id == l.Modu_Id)
                             where com.User_Id == u.Id
                             from peri in ApplicationDbContext.PointManagerCategory
                             where peri.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == co.CompanyId
                             select new
                             {
                                 UserName = u.UserName,
                                 Documento = u.Document,
                                 Nombre = u.FirstName + " " + u.LastName,
                                 Cargo = po.Posi_Description,
                                 Area = a.AreaName,
                                 Ubicación = lo.Loca_Description,
                                 Pais = u.Country,
                                 Ciudad = c.City_Name,
                                 Estado = u.Enable,
                                 Tipo_de_Usuario = u.Role,
                                 Tema = com.Module.Modu_Name,
                                 Tipo_Curso = com.Module.Modu_TypeOfModule,
                                 Titulo_Comentario = com.BePr_TiTle,
                                 Fecha_Creación_Comentario = com.BePr_InitDate,
                                 Fecha_Aprobación_comentario = com.BePr_FinishDate,
                                 Estado_Comentario = com.BePr_StateBetterpractice,
                                 Compañia = co.CompanyName,
                                 Puntos_Comentario = peri.PoMaCa_course
                             }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE COMENTARIOS-B.PRACTICAS.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE COMENTARIOS BUENAS PRACTICAS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Comentarios Buenas Practicas ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Reports");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Reports");
        }
        public ActionResult ExportToExcel29()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            var List = (
                             from u in ApplicationDbContext.Users
                             from co in ApplicationDbContext.Companies
                             from po in ApplicationDbContext.Position
                             where u.PositionId == po.Posi_id
                             from a in ApplicationDbContext.Areas
                             where u.AreaId == a.AreaId
                             from lo in ApplicationDbContext.Location
                             where u.LocationId == lo.Loca_Id
                             from c in ApplicationDbContext.City
                             where u.CityId == c.City_Id
                             from com in ApplicationDbContext.Improvements.Where(x => x.Modu_Id == l.Modu_Id)
                             where com.User_Id == u.Id
                             from peri in ApplicationDbContext.PointManagerCategory
                             where peri.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == UserActuallity.CompanyId
                             where u.CompanyId == co.CompanyId
                             select new
                             {
                                 UserName = u.UserName,
                                 Documento = u.Document,
                                 Nombre = u.FirstName + " " + u.LastName,
                                 Cargo = po.Posi_Description,
                                 Area = a.AreaName,
                                 Ubicación = lo.Loca_Description,
                                 Pais = u.Country,
                                 Ciudad = c.City_Name,
                                 Estado = u.Enable,
                                 Tipo_de_Usuario = u.Role,
                                 Tema = com.Module.Modu_Name,
                                 Tipo_Curso = com.Module.Modu_TypeOfModule,
                                 Titulo_Comentario = com.Impr_Title,
                                 Fecha_Creación_Comentario = com.Impr_InitDate,
                                 Fecha_Aprobación_comentario = com.Impr_FinishDate,
                                 Estado_Comentario = com.Impr_StateImprovement,
                                 Compañia = co.CompanyName,
                                 Puntos_Comentario = peri.PoMaCa_Improvements,
                             }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE COMENTARIOS-MEJORAS.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE COMENTARIOS-MEJORAS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Comentarios Mejoras ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Reports");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Reports");
        }
        public ActionResult ExportToExcel30()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            var List = (
                             from user in ApplicationDbContext.Users
                             from module in ApplicationDbContext.Modules
                             from topicCourse in ApplicationDbContext.TopicsCourses
                             from advance in ApplicationDbContext.AdvanceUsers

                             where user.Id == advance.User_Id
                             where advance.ToCo_id == topicCourse.ToCo_Id
                             where topicCourse.Modu_Id == module.Modu_Id

                             select new
                             {
                                 UserName = user.UserName,
                                 Documento = user.Document,
                                 Nombre = user.FirstName + " " + user.LastName,
                                 Email = user.Email,
                                 NombreModulo = module.Modu_Name,
                                 Tema = topicCourse.ToCo_Name,
                                 Puntaje = advance.AdUs_ScoreObtained,
                                 FechaPresentacion = advance.AdUs_PresentDate
                             }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE COMENTARIOS-MEJORAS.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>REPORTE COMENTARIOS-MEJORAS</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Comentarios Mejoras ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Reports");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Reports");
        }
        public ActionResult ExportToExcel31()
        {
            ApplicationUser UserActuallity = UserManager.FindById(GetActualUserId().Id);
            var l = ApplicationDbContext.Enrollments.Where(x => x.User_Id == UserActuallity.Id).FirstOrDefault();
            var List = (
                             from user in ApplicationDbContext.Users
                             from module in ApplicationDbContext.Modules
                             from topicCourse in ApplicationDbContext.TopicsCourses
                             from advance in ApplicationDbContext.AdvanceLoseUser

                             where user.Id == advance.User_Id
                             where advance.ToCo_id == topicCourse.ToCo_Id
                             where topicCourse.Modu_Id == module.Modu_Id

                             select new
                             {
                                 UserName = user.UserName,
                                 Documento = user.Document,
                                 Nombre = user.FirstName + " " + user.LastName,
                                 Email = user.Email,
                                 NombreModulo = module.Modu_Name,
                                 Tema = topicCourse.ToCo_Name,
                                 Puntaje = advance.AdLoUs_ScoreObtained,
                                 FechaPresentacion = advance.AdLoUs_PresentDate
                             }).ToList();
            if (List.Count != 0)
            {
                var gv = new GridView();
                gv.DataSource = List;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Calificación-usuarios-Individual.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write("<H3>Calificación usuarios Individual</H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write(" <center> <b> Comentarios Mejoras ");
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("Reports");
            }
            else
            {
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Reports");
        }

    }
}