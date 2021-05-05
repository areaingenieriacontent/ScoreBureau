using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SCORM1.Models;
using SCORM1.Enum;
using SCORM1.Models.Logs;
using SCORM1.Models.ViewModel;
using System.Net.Mail;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models.PageCustomization;

namespace SCORM1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        public AccountController()
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
        public AccountController(ApplicationSignInManager signInManager)
        {
            SignInManager = signInManager;
            this.ApplicationDbContext = new ApplicationDbContext();
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

        private string GetUrlLogo()
        {

            string Logo = "";
            StylesLogos CompanyLogo = ApplicationDbContext.StylesLogos.Find(2);
            if (CompanyLogo != null)
            {
                Logo = CompanyLogo.UrlLogo;
            }
            else
            {

                Logo = ApplicationDbContext.StylesLogos.Find(2).UrlLogo;
            }
            return Logo;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoginViewModel model = new LoginViewModel
            {
                UrlLogo = GetUrlLogo()
            };
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                model.Sesion = SESION.Si;
                model.Logo = GetUrlLogo();
                return RedirectToAction("Index", "Home");
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    ApplicationUser UserCurrent = UserManager.FindByName(model.Email);
                    if (UserCurrent.SesionUser == SESION.No)
                    {
                        var table = ApplicationDbContext.TableChanges.Find(72);
                        var code = ApplicationDbContext.CodeLogs.Find(151);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + "acaba de iniciar sesión en la compañia con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        UserCurrent.SesionUser = SESION.Si;
                        ApplicationDbContext.SaveChanges();
                        if (UserCurrent.firstAccess == null)
                        {
                            UserCurrent.firstAccess = DateTime.Now;
                        }
                        UserCurrent.lastAccess = DateTime.Now;
                        Session["FirstName"] = UserCurrent.FirstName;
                        Session["LastName"] = UserCurrent.LastName;
                        Session["sesion"] = UserCurrent.SesionUser;
                        UserManager.Update(UserCurrent);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        var table = ApplicationDbContext.TableChanges.Find(72);
                        var code = ApplicationDbContext.CodeLogs.Find(157);
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
                                Log_Description = "El usuario con id: " + UserCurrent.Id + " intento iniciar sesion dos veces con su misma cuenta en la compañia con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }
                        TempData["Info"] = "Ya hay una sesión iniciada con este usuario, esta sesión se cerrara por seguridad";
                        model.UrlLogo = GetUrlLogo();
                        ApplicationUser UserCurrent1 = UserManager.FindByName(model.Email);
                        UserCurrent1.SesionUser = SESION.No;
                        ApplicationDbContext.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Datos Incorrectos");
                    model.UrlLogo = GetUrlLogo();
                    model.Sesion = SESION.Si;
                    return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendEmail(FormViewModel model)
        {

            if (model.Form_Name != null && model.Form_Email != null && model.Form_Comment != null)
            {
                string id = "e648c132-f5dc-475b-87c6-171cf829efe9";
                var tutor = ApplicationDbContext.Users.Find(id);
                var correo = tutor.Email;
                var module = ApplicationDbContext.Modules.Find(model.Modu_Id);
                var enroll = ApplicationDbContext.Enrollments.Where(x => x.Modu_Id == model.Modu_Id && x.Enro_RoleEnrollment == ROLEENROLLMENT.Docente).ToList();
                if (enroll.Count != 0)
                {
                    foreach (var item in enroll)
                    {
                        MailMessage solicitudes = new MailMessage();
                        solicitudes.Subject = "Contacto tutor";
                        solicitudes.Body = "Cordial saludo " + "<br/>" +

                           "<br/>" + "El usuario : " + model.Form_Name + ", desea contactarse con un tutor del curso " + module.Modu_Name + " para tratar el siguiente asunto: " +
                           "<br/>" + model.Form_Comment +
                           "<br/>" +
                           "<br/>" + "El correo del usuario es el siguiente " + model.Form_Email +
                           "<br/>" +
                           "<br/>" + "Equipo de Soporte UnAula";

                        solicitudes.To.Add(item.ApplicationUser.Email);
                        solicitudes.IsBodyHtml = true;
                        var smtp22 = new SmtpClient();
                        smtp22.Send(solicitudes);
                    }
                }
                MailMessage solicitud = new MailMessage();
                solicitud.Subject = "Contácto tutor";
                solicitud.Body = "Cordial saludo " + "<br/>" +

                   "<br/>" + "El usuario : " + model.Form_Name + " desea contactarse con un tutor del curso " + module.Modu_Name + " para tratar el siguiente asunto: " +
                   "<br/>" + model.Form_Comment +
                   "<br/>" +
                   "<br/>" + "El correo del usuario es el siguiente " + model.Form_Email +
                   "<br/>" +
                   "<br/>" + "Equipo de Soporte UnAula";

                solicitud.To.Add(correo);
                solicitud.IsBodyHtml = true;
                var smtp2 = new SmtpClient();
                smtp2.Send(solicitud);
            }

            return RedirectToAction("Index", "Home");
        }
        //
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            model.UrlLogo = ApplicationDbContext.StylesLogos.Find(1).UrlLogo;
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //, Company = ApplicationDbContext.Companies.Find(model.CompanyId), Position = ApplicationDbContext.Position.Find(model.PositionId), Area = ApplicationDbContext.Areas.Find(model.AreaId), City = ApplicationDbContext.City.Find(model.CityId), Location = ApplicationDbContext.Location.Find(model.LocationId)
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Role = (ROLES)model.Role, Document = model.Document, Country = (COUNTRY)model.Country };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            var CompanySearch = ApplicationDbContext.StylesLogos.Find(2);
            ForgotPasswordViewModel model = new ForgotPasswordViewModel
            {
                UrlLogo = CompanySearch.UrlLogo,
                Sesion = SESION.Si
            };
            return View(model);
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                //Evalua si la variable del modelo es vacia o no, si es vacia se establece por defecto
                if (model.UserName == null)
                {
                    var user = DefaultIfEmpty;
                }
                //Si no es vacia, ejecuta la funcion por busqueda del usuario
                else
                {
                    var user = await UserManager.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        var table = ApplicationDbContext.TableChanges.Find(72);
                        var code = ApplicationDbContext.CodeLogs.Find(154);
                        var idcompany = user.CompanyId;
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
                                ApplicationUser = user,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = user.Id,
                                Log_Description = "El usuario con id: " + user.Id + " olvido su contraseña y acaba de enviar un correo para restablecerla, el usuario pertenece a la compañia con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }

                        UserManager.RemovePassword(user.Id);
                        UserManager.AddPassword(user.Id, user.UserName);
                        SendEmail(user.FirstName + " " + user.LastName, user.Email, user.UserName, user.Company.CompanyName);
                        var user2 = user;
                        UserManager.Update(user2);
                        TempData["Menssages"] = "Hemos enviado un correo a su cuenta con la información solicitada ";

                        return RedirectToAction("Index", "Home");
                    }

                }
                // si user esta vacio y se establece la variable user por defecto entra a la validacion por mail
                //Funcion para el mail
                //Evalua si la variable del modelo es vacia o no, si es vacia se establece por defecto
                if (model.UserMail == null)
                {
                    var mail = DefaultIfEmpty;
                }
                else
                {
                    var mail = await UserManager.FindByEmailAsync(model.UserMail);
                    if (mail != null)
                    {
                        var table = ApplicationDbContext.TableChanges.Find(72);
                        var code = ApplicationDbContext.CodeLogs.Find(154);
                        var idcompany = mail.CompanyId;
                        if (idcompany != null)
                        {
                            var company = ApplicationDbContext.Companies.Find(idcompany);
                            string ip = IpUser();
                            var idchange = new IdChange
                            {
                                IdCh_IdChange = mail.Id
                            };
                            ApplicationDbContext.IdChanges.Add(idchange);
                            ApplicationDbContext.SaveChanges();
                            Log logsesiontrue = new Log
                            {
                                ApplicationUser = mail,
                                CoLo_Id = code.CoLo_Id,
                                CodeLogs = code,
                                Log_Date = DateTime.Now,
                                Log_StateLogs = LOGSTATE.Realizado,
                                TableChange = table,
                                TaCh_Id = table.TaCh_Id,
                                IdChange = idchange,
                                IdCh_Id = idchange.IdCh_Id,
                                User_Id = mail.Id,
                                Log_Description = "El usuario con id: " + mail.Id + " olvido su contraseña y acaba de enviar un correo para restablecerla, el usuario pertenece a la compañia con id " + company.CompanyId,
                                Company = company,
                                Company_Id = company.CompanyId,
                                Log_Ip = ip
                            };
                            ApplicationDbContext.Logs.Add(logsesiontrue);
                            ApplicationDbContext.SaveChanges();
                        }

                        UserManager.RemovePassword(mail.Id);
                        UserManager.AddPassword(mail.Id, mail.UserName);
                        SendEmail(mail.FirstName + " " + mail.LastName, mail.Email, mail.UserName, mail.Company.CompanyName);
                        var mail2 = mail;
                        UserManager.Update(mail2);
                        TempData["Menssages"] = "Hemos enviado un correo a su cuenta con la información solicitada ";

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            // fin de la funciojn para el mail
            // If we got this far, something failed, redisplay form
            model.Sesion = SESION.Si;
            model.Logo = GetUrlLogo();
            return View(model);
        }



        private void SendEmail(string NameUser, string Email, string Usuario, string company)
        {
            MailMessage solicitud = new MailMessage();
            solicitud.Subject = "Bienvenida a la Comunidad Social de Conocimiento Bureau Veritas";
            solicitud.Body = "<img src='http://aprendeyavanza2.com.co/bureauveritastrainingcommunity/content/images/image6.jpg' width='45%' />"
            + "<br/>" + "Cordial saludo, " + "<br/>" + "<br/>" +
                "Respetado Sr(a). " + NameUser +
               "<br/>" + "Es un gusto darle la bienvenida a la comunidad social de conocimiento de Bureau Veritas." +
               "<br/>" + "Agradecemos ingresar y comenzar con su experiencia de aprendizaje con los siguientes datos de acceso:" + "<br/>" +
                 "<br/>" + "Usuario: " + Usuario +
               "<br/>" + "Contraseña: " + Usuario + "<br/>" +
               "<br/>" + "Link de ingreso a la plataforma: " +
            "<br/>" + "https://www.aprendeyavanza2.com.co/bureauveritastrainingcommunity/" +
            "" + "<br/>" +
               "<br/>" + "Bureau Veritas" +
               "<br/>" + "Construyendo Confianza" +
               "<br/>" + "<img src='http://aprendeyavanza2.com.co/bureauveritastrainingcommunity/content/images/firmabureau.png' width='80%' />";
            solicitud.To.Add(Email);
            solicitud.IsBodyHtml = true;
            var smtp2 = new SmtpClient();
            smtp2.Send(solicitud);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ForgotPasswordConfirmation(ForgotPasswordConfirmationViewModel model)
        {
            var user = await UserManager.FindByNameAsync(model.user);
            if (user != null)
            {
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                if (code == model.Code)
                {

                }
            }
            return View();
        }


        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            string id = GetActualUserId().Id;
            var user = ApplicationDbContext.Users.Find(id);
            user.SesionUser = SESION.No;
            ApplicationDbContext.SaveChanges();
            if (GetActualUserId().CompanyId != null)
            {
                var table = ApplicationDbContext.TableChanges.Find(72);
                var code = ApplicationDbContext.CodeLogs.Find(152);
                var idcompany = user.CompanyId;
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
                    ApplicationUser = user,
                    CoLo_Id = code.CoLo_Id,
                    CodeLogs = code,
                    Log_Date = DateTime.Now,
                    Log_StateLogs = LOGSTATE.Realizado,
                    TableChange = table,
                    TaCh_Id = table.TaCh_Id,
                    IdChange = idchange,
                    IdCh_Id = idchange.IdCh_Id,
                    User_Id = user.Id,
                    Log_Description = "El usuario con id: " + user.Id + "acaba de cerrar sesión en la compañia con id " + company.CompanyId,
                    Company = company,
                    Company_Id = company.CompanyId,
                    Log_Ip = ip
                };
                ApplicationDbContext.Logs.Add(logsesiontrue);
                ApplicationDbContext.SaveChanges();
                string a = GetActualUserId().Company.CompanyName;
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserManager != null)
                {
                    UserManager.Dispose();
                    UserManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public object DefaultIfEmpty { get; private set; }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}