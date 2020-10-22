using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Engagement;
using SCORM1.Models.Logs;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.SCORM1;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class AdminPointController : Controller
    {

        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        public AdminPointController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        /*
         * Metodo que se encarga de llenar una lista con el nombre de las categorias que se pueden usar para implementar
         * en el creación de los puntos adicionales.
         * para poderlo utilizar en una vista de formulario como lista desplegable
         */
        public IEnumerable<SelectListItem> GetCategory()
        {
            int CompanyId = GetActualUserId().Company.CompanyId;
            List<TypePoint> AreaOfMyCompany = ApplicationDbContext.TypePoints.ToList();
            IEnumerable<SelectListItem> Categorias = AreaOfMyCompany.Select(x =>
           new SelectListItem
           {
               Value = x.TyPo_Id.ToString(),
               Text = x.TyPo_Description
           });
            return new SelectList(Categorias, "Value", "Text");
        }
        /*
          * Metodo que se encarga de llenar una lista con el nombre de las áreas por compañia.
          * para poderlo utilizar en una vista de formulario como lista desplegable
          */
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
        /*
        * Metodo que se encarga de llenar una lista con el nombre de los cargos por compañia.
        * para poderlo utilizar en una vista de formulario como lista desplegable
        */
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
        /*
        * Metodo que se encarga de llenar una lista con el nombre de las ciudades por compañia.
        * para poderlo utilizar en una vista de formulario como lista desplegable
        */
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
        /*
        * Metodo que se encarga de llenar una lista con el nombre de las ubicaciones por compañia.
        * para poderlo utilizar en una vista de formulario como lista desplegable
        */
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
        /*
        * Metodo que dependiendo el usuario que se loguee realiza una instancia de ese usuario
        * para que puedan acceder a su información
        */
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
        //Este metodo toma la ip de la cual se está ingresando y la retorna en una variable String.
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
        /*
        * Este metodo se utiliza para Obtener el logo de la compañia del usuario actual,
        * para que se vea en todas las vistas.
        */
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
        // GET: AdminPoint
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProfileAd()
        {
            return PartialView("_ProfileAd");
        }
        /*
         * Metodo utilizado para ver la vista de los terminos y condiciones 
         * que tiene que aceptar el usuario
         */
        [Authorize]
        public ActionResult TermsandConditions()
        {
            AdminProfileViewModel model = new AdminProfileViewModel { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };
            int CompanyUser = (int)GetActualUserId().CompanyId;
            var terms = ApplicationDbContext.StylesLogos.Where(x => x.companyId == CompanyUser).FirstOrDefault();
            if (terms != null)
            {
                model.FileTerms = terms.colorsTittle;
            }
            else
            {
                model.FileTerms = ApplicationDbContext.StylesLogos.Find(1).colorsTittle;
            }
            model.Sesion = GetActualUserId().SesionUser;
            return PartialView("_TermsandConditions", model);
        }
        /*
         * Metodo utilizado para validar si el usuario acepto los terminos 
         * y condiciones de una compañia
         * Esté metodo recibe un modelo con la información necesaria 
         * para validar los terminos y condiciones
         */
        [Authorize]
        public ActionResult Validateterms(AdminProfileViewModel model)
        {
            if (model.termsandconditions == true)
            {
                var id = GetActualUserId().Id;
                ApplicationUser user = ApplicationDbContext.Users.Find(id);
                user.TermsandConditions = Terms_and_Conditions.Aceptado;
                ApplicationDbContext.SaveChanges();
                var table = ApplicationDbContext.TableChanges.Find(72);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(302);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = id
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de aceptar los terminos y condiciones, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Info"] = "Para ingresar al contenido debe aceptar los términos y condiciones.";
                return RedirectToAction("Index", "Home");
            }
        }
        /*
         * Metodo utlizado para ingresar a la 
         * gestión de categorias de puntos disponibles 
         * por compañia
         */ 
        [Authorize]
        public ActionResult Category()
        {
            //Inicio código logs
            var table = ApplicationDbContext.TableChanges.Find(65);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(324);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de ingresar a la vista de gestión de categoria de puntos en la compañia con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            //Fin código logs
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            AdminPointCategory model = new AdminPointCategory { ActualRole = GetActualUserId().Role, listcategory = ApplicationDbContext.TypePoints.ToList(), Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
        /*
         * Metodo utilizado para buscar una 
         * categoria de puntos por un nombre
         * Esté metodo recibe un modelo con todos 
         * los datos necesarios par4a realizar una busqueda
         */ 
        [HttpPost]
        [Authorize]
        public ActionResult SearchCategory(AdminPointCategory model)
        {
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchCate) || string.IsNullOrEmpty(model.SearchCate))
            {
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(65);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(325);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de una categoría de un puntos sin ingresar ningún nombre de una categoría para buscar, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
                return RedirectToAction("Category");
            }
            else
            {
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(65);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(325);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de realizar una búsqueda de una categoría de puntos ingresando un nombre de una categoría para buscar, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
                List<TypePoint> SearchedCategoryPrize = ApplicationDbContext.TypePoints.Where(m => m.TyPo_Description.Contains(model.SearchCate)).ToList();
                model = new AdminPointCategory { ActualRole = GetActualUserId().Role, listcategory = SearchedCategoryPrize };
                model.Sesion = GetActualUserId().SesionUser;
                model.Logo = GetUrlLogo();
                return View("Category", model);
            }
        }
        /*
         * Metodo utilizado para crear una nueva 
         * categoria de puntos por compañia
         * Esté metodo recibe un modelo con todos los datos necesarios
         * para crear una nueva categoria de puntos 
         */ 
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(AdminPointCategory model)
        {
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {

                TempData["Add"] = "La Categoria se ha creado con éxito";
                TypePoint categoryPrize = new TypePoint { TyPo_Description = model.TyPo_Description, Poin_TypePoints = model.Poin_TypePoints };
                ApplicationDbContext.TypePoints.Add(categoryPrize);
                ApplicationDbContext.SaveChanges();
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(65);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(326);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = categoryPrize.TyPo_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de añadir una nueva categoría de puntos, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
                return RedirectToAction("Category");
            }
            else
            {
                TempData["Add"] = "Ingrese Nombre de Categoria";
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(65);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(326);
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
                //Fin código logs
                return RedirectToAction("Category");
            }
        }
        /*
         * Metodo utilizado para eliminar una categoria
         * de puntos
         * Esté metodo recibe el id de la categoria para eliminar
         */ 
        [Authorize]
        public ActionResult DeleteCategory(int id)
        {
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            var deletedCategoryPrize = ApplicationDbContext.TypePoints.Find(id);
            //Inicio código logs
            var table = ApplicationDbContext.TableChanges.Find(65);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(328);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = deletedCategoryPrize.TyPo_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de eliminar una nueva categoría de puntos llamada " + deletedCategoryPrize.TyPo_Description + ", en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            //Fin código logs
            var point = ApplicationDbContext.Points.Where(x => x.TyPo_Id == deletedCategoryPrize.TyPo_Id).ToList();
            if (point.Count == 0)
            {
                TempData["Delete"] = "Categoria eliminada";
                ApplicationDbContext.TypePoints.Remove(deletedCategoryPrize);
                ApplicationDbContext.SaveChanges();
            }
            else
            {
                TempData["Delete"] = "No se puede eliminar la categoria por que tiene puntos asociados.";
            }
            return RedirectToAction("Category");
        }
        /*
         * Metodo utilizado para activar un formulario
         * para modificar los datos de una categoria de puntos
         * esté metodo recibe el id de la categoria selecionada 
         * para modificar
         */          
        [Authorize]
        public ActionResult UpdateCategory(int id)
        {
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            var updatedcategoryPrize = ApplicationDbContext.TypePoints.Find(id);
            TempData["UpdateCategoryPrize"] = "Categoria modificada con éxito";
            AdminPointCategory model = new AdminPointCategory
            {
                ActualRole = GetActualUserId().Role,
                listcategory = ApplicationDbContext.TypePoints.ToList(),
                TyPo_Id = updatedcategoryPrize.TyPo_Id,
                TyPo_Description = updatedcategoryPrize.TyPo_Description,
                Poin_TypePoints = updatedcategoryPrize.Poin_TypePoints,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            //Inicio código logs
            var table = ApplicationDbContext.TableChanges.Find(65);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(327);
            var idcompany = UserCurrent.CompanyId;
            if (idcompany != null)
            {
                var company = ApplicationDbContext.Companies.Find(idcompany);
                string ip = IpUser();
                var idchange = new IdChange
                {
                    IdCh_IdChange = updatedcategoryPrize.TyPo_Id.ToString()
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de seleccionar una categoría de puntos para modificar, en la compañía con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            //fin código logs
            return View("Category", model);
        }
        /*
         * Metodo utilizado para modificar los datos de 
         * una categoria de puntos seleccionada
         * Esté metodo recibe un modelo con todos los datos 
         * necesarios para modificar una categoria de puntos 
         * especifica
         */ 
        [HttpPost]
        [Authorize]
        public ActionResult UpdateCategory(AdminPointCategory model)
        {
            var GetCategoryPrizeCompanyId = GetActualUserId().CompanyId;
            var updatedcategoryPrize = ApplicationDbContext.TypePoints.Find(model.TyPo_Id);
            if (ModelState.IsValid)
            {

                TempData["Edit"] = "La categoria se ha Modificado con éxito";
                updatedcategoryPrize.TyPo_Description = model.TyPo_Description;
                updatedcategoryPrize.Poin_TypePoints = model.Poin_TypePoints;
                ApplicationDbContext.SaveChanges();
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(65);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(327);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = updatedcategoryPrize.TyPo_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " acaba de modificar una categoría de un puntos, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
                return RedirectToAction("Category");
            }

            else
            {
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(65);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(327);
                var idcompany = UserCurrent.CompanyId;
                if (idcompany != null)
                {
                    var company = ApplicationDbContext.Companies.Find(idcompany);
                    string ip = IpUser();
                    var idchange = new IdChange
                    {
                        IdCh_IdChange = updatedcategoryPrize.TyPo_Id.ToString()
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " no ha ingresado un nombre para modificar una categoría de puntos seleccionado, en la compañía con id " + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
                TempData["Edit"] = "Debe ingresar un nombre de Categoria";
                return RedirectToAction("category");
            }
        }
        /*
         * Metodo utilizado para ingresar a la gestión de 
         * puntos individuales por compañia
         */ 
        [Authorize]
        public ActionResult Points(int? page)
        {
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId && x.Role == ROLES.Usuario).ToList().ToPagedList(page ?? 1, 6);
            pointuser model = new pointuser { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            model.Categorias = GetCategory();
            //Inicio código logs
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
            //Fin código logs
            return View(model);
        }
        /*
         * Metodo utilizado para realizar la busqueda de 
         * un usuario por su nombre y así poderle asignar los puntos
         * Esté metdo recibe un modelo con todos los datos necesarios 
         * para realizar la busqueda
         */ 
        [HttpPost]
        [Authorize]
        public ActionResult SeachUserp(pointuser model, int? page)
        {
            var GetModuleCompany = GetActualUserId().CompanyId;
            if (string.IsNullOrWhiteSpace(model.SearchUser) || string.IsNullOrEmpty(model.SearchUser))
            {
                //Inicio código logs
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
                //Fin código logs
                return RedirectToAction("Points");
            }
            else
            {
                IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompany && x.Role == ROLES.Usuario && (x.FirstName.Contains(model.SearchUser) || x.LastName.Contains(model.SearchUser) || x.UserName.Contains(model.SearchUser))).ToList().ToPagedList(page ?? 1, 6);
                model = new pointuser { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser, Logo = GetUrlLogo() };
                model.Sesion = GetActualUserId().SesionUser;
                //Inicio código logs
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
                //Fin código logs
                return View("Points", model);
            }
        }
        /*
         * Metodo utilizado para activar un formulario
         * que sirve para registrar los puntos que se le 
         * van a dar al usuario y escoger la categoria 
         * al que se le va a asignar
         * Esté metodo recibe el id del usuario al cual 
         * se le va a asignar los puntos adicionales
         */ 
        [Authorize]
        public ActionResult AddPoin(string id, int? page)
        {
            var user = ApplicationDbContext.Users.Find(id);
            var GetModuleCompanyId = GetActualUserId().Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == GetModuleCompanyId && x.Role == ROLES.Usuario).ToList().ToPagedList(page ?? 1, 6);
            pointuser model = new pointuser { ActualRole = GetActualUserId().Role, UserEnrolllment = ListOfUser, user = user, Logo = GetUrlLogo() };
            model.Sesion = GetActualUserId().SesionUser;
            model.Categorias = GetCategory();
            TempData["Info"] = "Datos";
            return View("Points", model);
        }
        /*
         * Metodo utilizado para asignar 
         * los puntos individuales al usuario seleccionado
         * Esté metodo recibe un modelo con todos los datos
         * necesarios para registrar los nuevos puntos al
         * usuario
         */ 
        [HttpPost]
        [Authorize]
        public ActionResult Pointadd(pointuser model)
        {
            var GetPointManagerCategoryCompanyId = GetActualUserId().CompanyId;
            if (ModelState.IsValid)
            {
                var user = ApplicationDbContext.Users.Find(model.user.Id);
                var userpoint = ApplicationDbContext.Points.Where(x => x.ApplicationUser.Id == model.user.Id && x.TyPo_Id == model.Cate_Id).ToList();
                if (userpoint.Count != 0)
                {
                    var a = userpoint.FirstOrDefault();
                    a.Quantity_Points = a.Quantity_Points + model.puntos;
                    ApplicationDbContext.SaveChanges();
                }
                else
                {
                    var cate = ApplicationDbContext.TypePoints.FirstOrDefault(x => x.TyPo_Id == model.Cate_Id);
                    var point = new Point
                    {
                        ApplicationUser = user,
                        TypePoint = cate,
                        TyPo_Id = cate.TyPo_Id,
                        Quantity_Points = model.puntos,
                        User_Id = user.Id,
                        Poin_Date = DateTime.Now
                    };
                    ApplicationDbContext.Points.Add(point);
                    ApplicationDbContext.SaveChanges();
                }
                //Inicio código logs
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
                //Fin código logs
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
        /*
         * Metodo utilizado para abrir la gestión de 
         * carga masiva de puntos adicionales a los usuarios
         *  de una compañia
         */
        [Authorize]
        public ActionResult MassivePoints()
        {
            AdminPointMassive model = new AdminPointMassive
            {
                ActualRole = GetActualUserId().Role,
                Areas = GetArea(),
                Cargos = GetPosition(),
                Ciudades = GetCityOfTheCompany(),
                Ubicación = GetLocationOfTheCompany(),
                Categorias = GetCategory()
            };
            model.Sesion = GetActualUserId().SesionUser;
            model.Logo = GetUrlLogo();
            //Inicio código logs
            var table = ApplicationDbContext.TableChanges.Find(65);
            var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var code = ApplicationDbContext.CodeLogs.Find(329);
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
                    Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de ingresar a la carga masiva de puntos, en la compañía con id" + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
            }
            //Fin código logs
            return View(model);
        }
        /*
         * Metodo utilizado para asignar a los usuarios
         * de una compañia puntos adicionales por un área 
         * especifica
         * Esté metodo recibe un modelo con todos los datos necesario 
         * para el registro de puntos adicionales
         */ 
        [Authorize]
        public ActionResult PointAreas(AdminPointMassive model)
        {
            var user = ApplicationDbContext.Users.Where(x => x.AreaId == model.Area_Id && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var point = item.Point.Where(x => x.TyPo_Id == model.Cate_Id).ToList();
                    if (point.Count != 0)
                    {
                        var p = point.FirstOrDefault();
                        p.Quantity_Points = p.Quantity_Points + model.puntos;
                        ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        var cat = ApplicationDbContext.TypePoints.Find(model.Cate_Id);
                        var po = new Point
                        {
                            ApplicationUser = item,
                            Poin_Date = DateTime.Now,
                            Quantity_Points = model.puntos,
                            TypePoint = cat,
                            TyPo_Id = cat.TyPo_Id,
                            User_Id = item.Id
                        };
                        ApplicationDbContext.Points.Add(po);
                        ApplicationDbContext.SaveChanges();
                    }
                    //Inicio código logs
                    var table = ApplicationDbContext.TableChanges.Find(41);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(320);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = item.Id
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de agregar puntos por el área con id " + model.Area_Id + " al usuario con id " + item.Id + " por el siguiente motivo: " + model.description + ", en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    //Fin código logs
                }
                TempData["Info"] = "Puntos asignados con éxito.";
            }
            else
            {
                TempData["Info"] = "No hay usuarios asociados con esta área";
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(41);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(320);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento agregar puntos por el área con id " + model.Area_Id + "pero no hay usuarios asociados a está área, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
            }
            return RedirectToAction("MassivePoints");
        }
        /*
         * Metodo utilizado para asignar a los usuarios
         * de una compañia puntos adicionales por un cargo 
         * especifico
         * Esté metodo recibe un modelo con todos los datos necesario 
         * para el registro de puntos adicionales
         */
        [Authorize]
        public ActionResult PointPosition(AdminPointMassive model)
        {
            var user = ApplicationDbContext.Users.Where(x => x.PositionId == model.Posi_Id && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var point = item.Point.Where(x => x.TyPo_Id == model.Cate_Id).ToList();
                    if (point.Count != 0)
                    {
                        var p = point.FirstOrDefault();
                        p.Quantity_Points = p.Quantity_Points + model.puntos;
                        ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        var cat = ApplicationDbContext.TypePoints.Find(model.Cate_Id);
                        var po = new Point
                        {
                            ApplicationUser = item,
                            Poin_Date = DateTime.Now,
                            Quantity_Points = model.puntos,
                            TypePoint = cat,
                            TyPo_Id = cat.TyPo_Id,
                            User_Id = item.Id
                        };
                        ApplicationDbContext.Points.Add(po);
                        ApplicationDbContext.SaveChanges();
                    }
                    //Inicio código logs
                    var table = ApplicationDbContext.TableChanges.Find(41);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(321);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = item.Id
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de agregar puntos por el cargo con id " + model.Posi_Id + " al usuario con id " + item.Id + " por el siguiente motivo: " + model.description + ", en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    //Fin código logs
                }
                TempData["Info"] = "Puntos asignados con éxito.";
            }
            else
            {
                TempData["Info"] = "No hay usuarios asociados con este cargo";
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(41);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(321);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento agregar puntos por el cargo con id " + model.Area_Id + "pero no hay usuarios asociados a este cargo, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
            }
            return RedirectToAction("MassivePoints");
        }
        /*
         * Metodo utilizado para asignar a los usuarios
         * de una compañia puntos adicionales por una ciudad 
         * especifica
         * Esté metodo recibe un modelo con todos los datos necesario 
         * para el registro de puntos adicionales
         */
        [Authorize]
        public ActionResult PointCity(AdminPointMassive model)
        {
            var user = ApplicationDbContext.Users.Where(x => x.CityId == model.City_Id && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var point = item.Point.Where(x => x.TyPo_Id == model.Cate_Id).ToList();
                    if (point.Count != 0)
                    {
                        var p = point.FirstOrDefault();
                        p.Quantity_Points = p.Quantity_Points + model.puntos;
                        ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        var cat = ApplicationDbContext.TypePoints.Find(model.Cate_Id);
                        var po = new Point
                        {
                            ApplicationUser = item,
                            Poin_Date = DateTime.Now,
                            Quantity_Points = model.puntos,
                            TypePoint = cat,
                            TyPo_Id = cat.TyPo_Id,
                            User_Id = item.Id
                        };
                        ApplicationDbContext.Points.Add(po);
                        ApplicationDbContext.SaveChanges();
                    }
                    //Inicio código logs
                    var table = ApplicationDbContext.TableChanges.Find(41);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(322);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = item.Id
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de agregar puntos por la ciudad con id " + model.City_Id + " al usuario con id " + item.Id + " por el siguiente motivo: " + model.description + ", en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    //Fin código logs
                }
                TempData["Info"] = "Puntos asignados con éxito.";
            }
            else
            {
                TempData["Info"] = "No hay usuarios asociados con está ciudad";
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(41);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(322);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento agregar puntos por la ciudad con id " + model.City_Id + "pero no hay usuarios asociados a está ciudad, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
            }
            return RedirectToAction("MassivePoints");
        }
        /*
         * Metodo utilizado para asignar a los usuarios
         * de una compañia puntos adicionales por una ubicación 
         * especifica
         * Esté metodo recibe un modelo con todos los datos necesario 
         * para el registro de puntos adicionales
         */
        [Authorize]
        public ActionResult PointLocation(AdminPointMassive model)
        {
            var user = ApplicationDbContext.Users.Where(x => x.LocationId == model.Loca_Id && x.Role == ROLES.Usuario).ToList();
            if (user.Count != 0)
            {
                foreach (var item in user)
                {
                    var point = item.Point.Where(x => x.TyPo_Id == model.Cate_Id).ToList();
                    if (point.Count != 0)
                    {
                        var p = point.FirstOrDefault();
                        p.Quantity_Points = p.Quantity_Points + model.puntos;
                        ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        var cat = ApplicationDbContext.TypePoints.Find(model.Cate_Id);
                        var po = new Point
                        {
                            ApplicationUser = item,
                            Poin_Date = DateTime.Now,
                            Quantity_Points = model.puntos,
                            TypePoint = cat,
                            TyPo_Id = cat.TyPo_Id,
                            User_Id = item.Id
                        };
                        ApplicationDbContext.Points.Add(po);
                        ApplicationDbContext.SaveChanges();
                    }
                    //Inicio código logs
                    var table = ApplicationDbContext.TableChanges.Find(41);
                    var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                    var code = ApplicationDbContext.CodeLogs.Find(323);
                    var idcompany = UserCurrent.CompanyId;
                    if (idcompany != null)
                    {
                        var company = ApplicationDbContext.Companies.Find(idcompany);
                        string ip = IpUser();
                        var idchange = new IdChange
                        {
                            IdCh_IdChange = item.Id
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
                            Log_Description = "El usuario con id: " + UserCurrent.Id + " Acaba de agregar puntos por la ubicación con id " + model.Loca_Id + " al usuario con id " + item.Id + " por el siguiente motivo: " + model.description + ", en la compañía con id" + company.CompanyId,
                            Company = company,
                            Company_Id = company.CompanyId,
                            Log_Ip = ip
                        };
                        ApplicationDbContext.Logs.Add(logsesiontrue);
                        ApplicationDbContext.SaveChanges();
                    }
                    //Fin código logs
                }
                TempData["Info"] = "Puntos asignados con éxito.";
            }
            else
            {
                TempData["Info"] = "No hay usuarios asociados con está ubicación";
                //Inicio código logs
                var table = ApplicationDbContext.TableChanges.Find(41);
                var UserCurrent = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var code = ApplicationDbContext.CodeLogs.Find(323);
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
                        Log_Description = "El usuario con id: " + UserCurrent.Id + " Intento agregar puntos por la ubicación con id " + model.Loca_Id + "pero no hay usuarios asociados a está ubicación, en la compañía con id" + company.CompanyId,
                        Company = company,
                        Company_Id = company.CompanyId,
                        Log_Ip = ip
                    };
                    ApplicationDbContext.Logs.Add(logsesiontrue);
                    ApplicationDbContext.SaveChanges();
                }
                //Fin código logs
            }
            return RedirectToAction("MassivePoints");
        }


    }
}