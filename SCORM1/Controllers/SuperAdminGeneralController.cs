using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCORM1.Models.SCORM1;
using SCORM1.Enum;
using System.Net.Mail;
using PagedList;
using SCORM1.Models.Newspaper;
using System.Threading.Tasks;
using SCORM1.Models.PageCustomization;
using System.Net;

namespace SCORM1.Controllers
{
    public class SuperAdminGeneralController : Controller
    {

        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public SuperAdminGeneralController()
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

        // envia la lista de compañias 
        public ActionResult CreationOfCompanies()
        {
            List<Company> Companies = ApplicationDbContext.Companies.ToList();

            SuperAdminGeneralControllerViewModel model = new SuperAdminGeneralControllerViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListCompanies = Companies,
                Logo= GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
        private string GetUrlLogo()
        {
      
            string Logo = "";
            StylesLogos CompanyLogo = ApplicationDbContext.StylesLogos.Where(x => x.companyId == null).FirstOrDefault();
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

        //crea las compañias 
        public ActionResult CreateCompany(SuperAdminGeneralControllerViewModel model)
        {

            if (ModelState.IsValid)
            {
                Company NewComapany = new Company
                {
                    CompanyName = model.CompanyName,
                    CompanyNit = model.CompanyNit,
                    CompanySector = model.CompanySector,
                    CompanyType = model.CompanyType,
                    CompanyGame = model.CompanyGame
                };
                ApplicationDbContext.Companies.Add(NewComapany);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "Se ha creado la compañía con éxito";
                return RedirectToAction("CreationOfCompanies");
            }
            TempData["Menssage"] = "hubo problema al crear la compañia por favor intente de nuevo";
            return RedirectToAction("CreationOfCompanies");
        }

        //aqui se actualizan las compañias 
        //Get
        [HttpGet]
        public ActionResult UpdateCompanyCurrent(int id)
        {
            List<Company> Companies = ApplicationDbContext.Companies.ToList();
            Company CompanyToUpdate = ApplicationDbContext.Companies.Find(id);
            SuperAdminGeneralControllerViewModel model = new SuperAdminGeneralControllerViewModel
            {
                ActualRole = GetActualUserId().Role,
                ListCompanies = Companies,
                Logo = GetUrlLogo(),
                CompanyToUpdate = CompanyToUpdate,            
            };
            TempData["UpdateCompany"] = "Se va a actualizar";
            model.Sesion = GetActualUserId().SesionUser;
            return View("CreationOfCompanies",model);
        }

        [HttpPost]
        public ActionResult UpdateCompanyCurrent(SuperAdminGeneralControllerViewModel model)
        {
            if (ModelState.IsValid)
            {
                Company companyToUpdate = ApplicationDbContext.Companies.Find(model.CompanyToUpdate.CompanyId);
                companyToUpdate.CompanyName = model.CompanyToUpdate.CompanyName;
                companyToUpdate.CompanyNit = model.CompanyToUpdate.CompanyNit;
                companyToUpdate.CompanySector = model.CompanyToUpdate.CompanySector;
                companyToUpdate.CompanyType = model.CompanyToUpdate.CompanyType;
                companyToUpdate.CompanyGame = model.CompanyToUpdate.CompanyGame;
                ApplicationDbContext.SaveChanges();

                TempData["Menssage"] = "Se Actualizo la compañia con éxito";
                return RedirectToAction("CreationOfCompanies");
            }
            return RedirectToAction("CreationOfCompanies");
        }

        //este enviara a la vista de creacion de los usuarios administradores  

        public ActionResult ShowTheUserAdministratorsOfTheCompany(int idCompany, int? page)
        {
            IPagedList<ApplicationUser> ListOfUserAdmin = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.Role == ROLES.AdministradoGeneral && x.CompanyId == idCompany).ToList().ToPagedList(page ?? 1, 6);
            SuperAdminGeneralManagementUsersAdminOfTheCompanies model = new SuperAdminGeneralManagementUsersAdminOfTheCompanies
            {
                ActualRole = GetActualUserId().Role,
                UserOfCompany = ListOfUserAdmin,
                companyId = idCompany,
                Logo= GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(SuperAdminGeneralManagementUsersAdminOfTheCompanies UserObtainedOfTheView)
        {
            var getcompany = ApplicationDbContext.Companies.Find(UserObtainedOfTheView.companyId);
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
                    Role = ROLES.AdministradoGeneral,
                    CompanyId = UserObtainedOfTheView.companyId,
                    Enable = UserObtainedOfTheView.enable,
                    Company = getcompany,
                    TermsandConditions = Terms_and_Conditions.No_apceptado,
                    Videos = VIDEOS.No_apceptado,
                    SesionUser =SESION.No,
                    TermsJuego=Terms_and_Conditions.No_apceptado

                };
                string next = VerifyUserFields(UserToCreate);

                if (next == "success")
                {
                    IdentityResult result = await UserManager.CreateAsync(UserToCreate, UserObtainedOfTheView.UserName);
                    if (result.Succeeded)
                    {
                        SendEmail(UserToCreate.FirstName + " " + UserToCreate.LastName, UserToCreate.Email, UserToCreate.UserName, UserToCreate.Company.CompanyName);
                        return RedirectToAction("ShowTheUserAdministratorsOfTheCompany", new { idCompany = UserObtainedOfTheView.companyId });
                    }
                    TempData["Warning"] = result.Errors.First();

                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    TempData["Menssage"] = " Error en la carga: descripcion. " + error + " favor verifica";
                    return View("ShowTheUserAdministratorsOfTheCompany", new { idCompany = UserObtainedOfTheView.companyId });
                }

            }
            return RedirectToAction("ShowTheUserAdministratorsOfTheCompany", new { idCompany = UserObtainedOfTheView.companyId });
        }
        private string VerifyUserFields(ApplicationUser result)
        {
            if (result.Email == null || result.Email.Length > 60)
            {
                return "no hay Email o excede el tamaño;";
            }
            if (result.FirstName == null || result.FirstName.Length > 15)
            {
                return "no hay nombres o excede el tamaño;";
            }
            if (result.LastName == null || result.LastName.Length > 15)
            {
                return "no hay apellidos o excede el tamaño;";
            }
            return "success";
        }

        public ActionResult DeleteUser(string IdUserToDelete)
        {
            if (IdUserToDelete != GetActualUserId().Id)
            {
                ApplicationUser user = UserManager.FindById(IdUserToDelete);
                int company = (int)user.CompanyId;
                foreach (var point in user.PointsComment.ToList())
                {
                    PointsComment Comment = ApplicationDbContext.PointsComments.Find(point.Comm_Id);
                    ApplicationDbContext.PointsComments.Remove(Comment);
                    ApplicationDbContext.SaveChanges();
                }

                UserManager.Delete(user);
                TempData["Menssage"] = "Usuario elimado con éxito";
                return RedirectToAction("ShowTheUserAdministratorsOfTheCompany", new { idCompany = company });
            }
            else
            {
                ApplicationUser user = UserManager.FindById(IdUserToDelete);

                TempData["Menssage"] = "No puedes eliminar el usuario con el que esta logeado";
                return View("ShowTheUserAdministratorsOfTheCompany", new { id = user.CompanyId });
            }

        }

        [HttpGet]
        public ActionResult UpdateUserCurrent(string IdUserToModified, int? page)
        {
            ApplicationUser user = UserManager.FindById(IdUserToModified);
            IPagedList<ApplicationUser> ListOfUserAdmin = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.Role == ROLES.AdministradoGeneral && x.CompanyId == user.CompanyId).ToList().ToPagedList(page ?? 1, 6);
            SuperAdminGeneralManagementUsersAdminOfTheCompanies UserToModified = new SuperAdminGeneralManagementUsersAdminOfTheCompanies
            {
                UserId = IdUserToModified,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Document = user.Document,
                StateUser = user.StateUser,
                Country = user.Country,
                UserOfCompany = ListOfUserAdmin,
                Logo = GetUrlLogo(),
                enable = user.Enable,
                
            };
            TempData["UpdateUserCurrent"] = "Actualizar";
            int company =(int) user.CompanyId;
            UserToModified.Sesion = GetActualUserId().SesionUser;
            return View("ShowTheUserAdministratorsOfTheCompany",UserToModified);
        }

        [HttpPost]
        public ActionResult UpdateUserCurrent(SuperAdminGeneralManagementUsersAdminOfTheCompanies UserToModified)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindById(UserToModified.UserId);

                user.UserName = UserToModified.UserName;
                user.FirstName = UserToModified.FirstName;
                user.LastName = UserToModified.LastName;
                user.Email = UserToModified.Email;
                user.Document = UserToModified.Document;
                user.Country = UserToModified.Country;
                user.Enable = UserToModified.enable;
                UserManager.RemovePassword(user.Id);
                UserManager.AddPassword(user.Id, UserToModified.UserName);
                UserManager.Update(user);
                return RedirectToAction("ShowTheUserAdministratorsOfTheCompany", new { idCompany = user.CompanyId });
            }
            return RedirectToAction("ShowTheUserAdministratorsOfTheCompany", new { id = UserToModified.companyId});

        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private void SendEmail(string NameUser, string Email, string Usuario, string Company)
        {
            WebClient client = new WebClient();
            string downloadString = client.DownloadString("http://aprendeyavanza2.com.co/bureauveritastrainingcommunity/content/images/htmlpage1.html");

            MailMessage solicitud = new MailMessage();
            solicitud.Subject = "Bienvenida a la Comunidad Social de Conocimiento Bureau Veritas";
            solicitud.Body = downloadString;
            solicitud.To.Add(Email);
            solicitud.IsBodyHtml = true;
            var smtp2 = new SmtpClient();
            smtp2.Send(solicitud);
        }
    }
}