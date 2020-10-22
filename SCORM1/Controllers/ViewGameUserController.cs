using Chart.Mvc.ComplexChart;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.Engagement;
using SCORM1.Models.MainGame;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class ViewGameUserController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        public ViewGameUserController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
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
        /*
         * Metodo utilizado para activar la vista de los 
         * terminos y condiciones del juego por compañia
         */ 
        [Authorize]
        public ActionResult TermsGame()
        {
            int CompanyUser = (int)GetActualUserId().CompanyId;
            var terms = ApplicationDbContext.MG_SettingMp.Where(x => x.Company_Id == CompanyUser).FirstOrDefault();
            if (GetActualUserId().TermsJuego == Terms_and_Conditions.No_apceptado)
            {
                UserGame model = new UserGame { ActualRole = GetActualUserId().Role, Logo = GetUrlLogo() };               
                model.FileTerms = terms.Sett_Instruction;
                model.Sesion = GetActualUserId().SesionUser;
                return View(model);
            }
            else
            {
                return RedirectToAction("Preview", "ViewGameuser", new { Id = terms.Sett_Id });
            }
           
        }
        /*
         * Metodo utilizado para validar la respuesta del 
         * usuario sobre los terminos y condiciones del juego
         * Esté metodo recibe un modelo con todos los datos 
         * necesarios para hacer la validación de los terminos
         */ 
        [Authorize]
        public ActionResult Validateterms(UserGame model)
        {
            if (model.termsandGame == true)
            {
                int CompanyUser = (int)GetActualUserId().CompanyId;
                var id = GetActualUserId().Id;
                ApplicationUser user = ApplicationDbContext.Users.Find(id);
                user.TermsJuego = Terms_and_Conditions.Aceptado;
                ApplicationDbContext.SaveChanges();
                var terms = ApplicationDbContext.MG_SettingMp.Where(x => x.Company_Id == CompanyUser).FirstOrDefault();
                return RedirectToAction("Preview", "ViewGameuser", new {Id=terms.Sett_Id});
            }
            else
            {
                TempData["Info"] = "Para ingresar al contenido debe aceptar los terminos primero";
                return RedirectToAction("TermsGame", "ViewGameuser");
            }
        }
        /*
         * Metodo utilizado para cargar la 
         * previsualización del juego
         * Esté metdo recibe el id de la configuración del juego
         */
        [Authorize]
        public ActionResult Preview(int Id)
        {
            var setting = ApplicationDbContext.MG_SettingMp.Find(Id);
            UserPreviw model = new UserPreviw
            {
                setting = setting,
                Sett_Id = setting.Sett_Id,
                Logo = GetUrlLogo()
            };
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
        /*
         * Metodo utilizado para cargar la vista de del menú del juego
         * Esté metodo recibe el id de la configuración del juego y la cantidad 
         * de preguntas contestadas por el usuario
         */
        [Authorize]
        public ActionResult Instructions(int Id)
        {
            var setting = ApplicationDbContext.MG_SettingMp.Find(Id);
            int facil = 0;
            int medio = 0;
            int dificil = 0;
            int cont = 0;
            int cm1 = 0;
            int cm2 = 0;
            int v1 = 0;
            int v2 = 0;
            List<listranking> ListRanking = new List<listranking>();
            List<listranking> ListRankingFinal = new List<listranking>();
            List<listranking> ListRankingUser = new List<listranking>();
            if (setting.MG_MultipleChoice.Count != 0)
            {
                foreach (var item in setting.MG_MultipleChoice)
                {
                    switch (item.MuCh_Level)
                    {
                        case Enum.LEVEL.Fácil:
                            facil = facil + 1;
                            break;
                        case Enum.LEVEL.Medio:
                            medio = 1 + medio;
                            break;
                        case Enum.LEVEL.Difícil:
                            dificil = 1 + dificil;
                            break;
                        default:
                            break;
                    }
                }
            }
                var useractual = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                int attemptsError = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.Respuesta == RESPUESTA.Incorrecta).OrderByDescending(x => x.AnUs_Id).Count();
                int attemptsUser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id).GroupBy(x => x.Attemps).Count();
                var attemptsUserblo = attempts.GroupBy(x => x.Attemps).ToList();
                var block = ApplicationDbContext.MG_BlockGameUser.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.BlGa_Id).ToList();
                if (block.Count != 0)
                {
                var bu = block.FirstOrDefault();
                if (bu.BlGa_Fecha < DateTime.Now)
                {
                    ApplicationDbContext.MG_BlockGameUser.Remove(bu);
                    ApplicationDbContext.SaveChanges();
                    var a = attemptsUserblo.FirstOrDefault();
                    var b = a.FirstOrDefault();
                    if (b.Respuesta == RESPUESTA.Incorrecta)
                    {
                        foreach (var item in a)
                        {
                            ApplicationDbContext.MG_AnswerUser.Remove(item);
                            ApplicationDbContext.SaveChanges();
                        }

                        return RedirectToAction("Instructions", new { Id = setting.Sett_Id });
                    }
                }
                else
                {
                    TempData["Info"] = "Usted está bloqueado por 24 horas.";
                    return RedirectToAction("Index", "Home");
                }
                }

            if (attempts.Count != 0)
                {
                    var a = attempts.FirstOrDefault();
                if (a.Respuesta == RESPUESTA.Incorrecta)
                {
                    if (attemptsUser == setting.Sett_Attemps)
                    {
                        cont = 0;
                    }
                    else
                    {
                        cont = 1;
                    }
                 
                }
                else
                {
                    var fecha = (DateTime.Now - a.FechaEnvio);
                    int w = (Int32)fecha.Value.TotalMinutes;
                    if (w > 15)
                    {
                       bool val= ValidTime();
                        if (val == true)
                        {
                            TempData["Info"] = "El tiempo para responder la pregunta se ha excedido,recuerde que tiene 15 minutos por tanto has sido bloqueado por 24 horas y has perdido tus puntos.";
                            return RedirectToAction("Instructions", new { Id = setting.Sett_Id });
                        }
                        else
                        {
                            TempData["Info"] = "Ya no tiene más intentos disponibles.";
                            return RedirectToAction("Instructions", new { Id = setting.Sett_Id });
                        }
                    }
                    else
                    {
                    int att = ApplicationDbContext.MG_AnswerUser.Where(x => x.Attemps == a.Attemps && x.Respuesta == RESPUESTA.Correcta && x.User_Id == useractual.Id).Count();
                    cont = att + 1;
                        if (att == 15)
                        {
                            if (TempData["ultimo"] == null)
                            {
                                if (a.Attemps == setting.Sett_Attemps)
                                {
                                    TempData["ultimo"] = "¡Gracias por tu participación!";
                                    TempData["ultimo1"] = "¡Ya puedes cerrar tu sesión!";
                                }
                                else
                                {
                                    TempData["ultimo"] = "¡Felicidades has pasado el nivel sigue participando hasta el último nivel!";
                                }
                            var attt = attempts.GroupBy(x => x.Attemps).ToList();
                            var gru = attt.FirstOrDefault();
                            foreach (var item in gru)
                            {
                                if (item.Comodin == CMD.CMB)
                                {
                                    cm1 = 1;
                                }
                                if (item.Comodin == CMD.EST)
                                {
                                    cm2 = 2;
                                }
                            }
                            if (cm1 == 0)
                            {
                                v1 = 70;
                            }
                            if (cm2 == 0)
                            {
                                v2 = 70;
                            }

                            var point = ApplicationDbContext.MG_Point.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.point_Id).ToList(); ;
                            var lastpoint = point.FirstOrDefault();
                            lastpoint.point_pointOfUser = lastpoint.point_pointOfUser + v1 + v2;
                            ApplicationDbContext.SaveChanges();
                            if (attemptsUser == setting.Sett_Attemps)
                            {
                                cont = 0;
                            }
                            else
                            {
                                cont = 1;
                            }
                        }
                         
                        }
                }
                    }
                }
                else
                {
                    cont = 1;
                }
                var puntos = ApplicationDbContext.MG_Point.Where(x => x.MG_SettingMp.Company_Id == setting.Company_Id).GroupBy(x => x.ApplicationUser).ToList();
                int posicion = 0;
                if (puntos.Count != 0)
                {

                    foreach (var item in puntos)
                    {
                        int p = item.Sum(x => x.point_pointOfUser);
                        var a = item.FirstOrDefault();
                        posicion++;
                        int puntosusaurio = ApplicationDbContext.MG_Point.Where(x => x.User_Id == a.User_Id).Sum(x => x.point_pointOfUser);
                        ListRanking.Add(new listranking
                        {
                            user = a.ApplicationUser,
                            puntos = puntosusaurio,
                            Ranking = posicion
                        });
                    }
                }
                var newlist = ListRanking.OrderByDescending(x => x.puntos).ToList();
                posicion = 0;
                foreach (var item in newlist)
                {

                    posicion++;
                    ListRankingFinal.Add(new listranking
                    {
                        user = item.user,
                        puntos = item.puntos,
                        Ranking = posicion
                    });
                }
                var puntosUser = ApplicationDbContext.MG_Point.Where(x => x.MG_SettingMp.Company_Id == setting.Company_Id && x.User_Id == useractual.Id).ToList();
                if (puntosUser.Count != 0)
                {
                    int puntosusaurio = puntosUser.Sum(x => x.point_pointOfUser);

                    ListRankingUser.Add(new listranking
                    {
                        user = useractual,
                        puntos = puntosusaurio,
                    });
                }
            
            UserPreviw model1 = new UserPreviw
            {
                setting = setting,
                Sett_Id = setting.Sett_Id,
                Logo = GetUrlLogo(),
                facil = facil,
                medio = medio,
                dificil = dificil,
                contador = cont,
                ListRanking=ListRankingFinal,
                ListRankingUser=ListRankingUser,
                User_Id=useractual.Id,
                attempts=attemptsError,
                attemptUser=attemptsUser
            };
            model1.Sesion = GetActualUserId().SesionUser;
            return View(model1);
        }
        /*
         * Metodo tulizado para cargar 
         * la pregunta que se le visualiza al usaurio
         * Esté metodo recibe el id de la pregunta y la cantidad
         * de preguntas contestadas por el usuario
         */
        [Authorize]
        public ActionResult Question()
        {
                        var useractual = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                        var setting = ApplicationDbContext.MG_SettingMp.First(x => x.Company_Id == useractual.CompanyId);
                        //var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.AnUs_Id).GroupBy(x=>x.Attemps).ToList();
                        var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                        List<MultipleChoiceselect> listselect = new List<MultipleChoiceselect>();
                        List<MultipleChoiceselect> listselect2 = new List<MultipleChoiceselect>();
                        List<MultipleChoiceselect> ListFinalfacil = new List<MultipleChoiceselect>();
                        int cm1 = 0;
                        int cm2 = 0;
                        if (attempts.Count !=0)
                        {
                        var att = attempts.GroupBy(x => x.Attemps).ToList();
                        var a = att.FirstOrDefault();
                        var b = a.FirstOrDefault();
                var block = ApplicationDbContext.MG_BlockGameUser.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.BlGa_Id).ToList();
                if (block.Count != 0)
                {
                    var bu = block.FirstOrDefault();
                    if (bu.BlGa_Fecha > DateTime.Now)
                    {
                        TempData["Info"] = "Usted está bloqueado por 24 horas.";
                        return RedirectToAction("Index", "Home");
                    }                   
                }
                if (b.Respuesta == RESPUESTA.Incorrecta)
                {
                    var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Fácil).ToList();
                    foreach (var item1 in attuser)
                    {
                        listselect.Add(new MultipleChoiceselect
                        {
                            Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                            MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                            MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                            MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                            MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                            MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                            MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                            listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                        });
                    }
                    foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Fácil))
                    {
                        listselect.Add(new MultipleChoiceselect
                        {
                            Sett_Id = item.Sett_Id,
                            MuCh_ID = item.MuCh_ID,
                            MuCh_NameQuestion = item.MuCh_NameQuestion,
                            MuCh_Description = item.MuCh_Description,
                            MuCh_Feedback = item.MuCh_Feedback,
                            MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                            MuCh_Level = item.MuCh_Level,
                            listanswerM = item.MG_AnswerMultipleChoice.ToList()
                        });
                    }
                    var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                    foreach (var item in aa)
                    {
                        if (item.Count() == 1)
                        {
                            foreach (var item1 in item)
                            {
                                listselect2.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item1.Sett_Id,
                                    MuCh_ID = item1.MuCh_ID,
                                    MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                    MuCh_Description = item1.MuCh_Description,
                                    MuCh_Feedback = item1.MuCh_Feedback,
                                    MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                    MuCh_Level = item1.MuCh_Level,
                                    listanswerM = item1.listanswerM.ToList()
                                });
                            }
                        }

                    }
                    var rnd = new Random();
                    var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                    var Newlistselect = randomselect.Take(1);
                    ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                }
                else
                {

                    var fecha = (DateTime.Now - b.FechaEnvio);
                    int w = (Int32)fecha.Value.TotalMinutes;
                    if (w > 15)
                    {
                        bool val = ValidTime();
                        if (val == true)
                        {
                            TempData["Info"] = "El tiempo para responder la pregunta se ha excedido,recuerde que tiene 15 minutos por tanto se te resta un intento.";
                            return RedirectToAction("Instructions", new { Id = setting.Sett_Id });
                        }
                        else
                        {
                            TempData["Info"] = "Ya no tiene más intentos disponibles.";
                            return RedirectToAction("Instructions", new { Id = setting.Sett_Id });
                        }
                    }
                    else
                    {
                    int cant = a.Count();

                    if (cant <= 3)
                    {
                        var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Fácil).ToList();
                        foreach (var item1 in attuser)
                        {
                            listselect.Add(new MultipleChoiceselect
                            {
                                Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                                MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                                MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                                MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                                MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                                MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                                MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                                listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                            });
                        }
                        foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Fácil))
                        {
                            listselect.Add(new MultipleChoiceselect
                            {
                                Sett_Id = item.Sett_Id,
                                MuCh_ID = item.MuCh_ID,
                                MuCh_NameQuestion = item.MuCh_NameQuestion,
                                MuCh_Description = item.MuCh_Description,
                                MuCh_Feedback = item.MuCh_Feedback,
                                MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                MuCh_Level = item.MuCh_Level,
                                listanswerM = item.MG_AnswerMultipleChoice.ToList()
                            });
                        }
                        var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                        foreach (var item in aa)
                        {
                            if (item.Count() == 1)
                            {
                                foreach (var item1 in item)
                                {
                                    listselect2.Add(new MultipleChoiceselect
                                    {
                                        Sett_Id = item1.Sett_Id,
                                        MuCh_ID = item1.MuCh_ID,
                                        MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                        MuCh_Description = item1.MuCh_Description,
                                        MuCh_Feedback = item1.MuCh_Feedback,
                                        MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                        MuCh_Level = item1.MuCh_Level,
                                        listanswerM = item1.listanswerM.ToList()
                                    });
                                }
                            }

                        }
                        var rnd = new Random();
                        var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                        var Newlistselect = randomselect.Take(1);
                        ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                    }
                    if (cant >= 4 && cant <= 9)
                    {
                        var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Medio).ToList();
                        if (attuser.Count != 0)
                        {
                            foreach (var item1 in attuser)
                            {
                                listselect.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                                    MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                                    MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                                    MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                                    MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                                    MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                                    MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                                    listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Medio))
                            {
                                listselect.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item.Sett_Id,
                                    MuCh_ID = item.MuCh_ID,
                                    MuCh_NameQuestion = item.MuCh_NameQuestion,
                                    MuCh_Description = item.MuCh_Description,
                                    MuCh_Feedback = item.MuCh_Feedback,
                                    MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                    MuCh_Level = item.MuCh_Level,
                                    listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                            foreach (var item in aa)
                            {
                                if (item.Count() == 1)
                                {
                                    foreach (var item1 in item)
                                    {
                                        listselect2.Add(new MultipleChoiceselect
                                        {
                                            Sett_Id = item1.Sett_Id,
                                            MuCh_ID = item1.MuCh_ID,
                                            MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                            MuCh_Description = item1.MuCh_Description,
                                            MuCh_Feedback = item1.MuCh_Feedback,
                                            MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                            MuCh_Level = item1.MuCh_Level,
                                            listanswerM = item1.listanswerM.ToList()
                                        });
                                    }
                                }
                            }
                            var rnd = new Random();
                            var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                        }
                        else
                        {
                            foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Medio))
                            {
                                listselect2.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item.Sett_Id,
                                    MuCh_ID = item.MuCh_ID,
                                    MuCh_NameQuestion = item.MuCh_NameQuestion,
                                    MuCh_Description = item.MuCh_Description,
                                    MuCh_Feedback = item.MuCh_Feedback,
                                    MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                    MuCh_Level = item.MuCh_Level,
                                    listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            var rnd = new Random();
                            var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                        }

                    }
                    if (cant >= 10 && cant <= 14)
                    {
                        var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Difícil).ToList();
                        if (attuser.Count != 0)
                        {
                            foreach (var item1 in attuser)
                            {
                                listselect.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                                    MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                                    MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                                    MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                                    MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                                    MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                                    MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                                    listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Difícil))
                            {
                                listselect.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item.Sett_Id,
                                    MuCh_ID = item.MuCh_ID,
                                    MuCh_NameQuestion = item.MuCh_NameQuestion,
                                    MuCh_Description = item.MuCh_Description,
                                    MuCh_Feedback = item.MuCh_Feedback,
                                    MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                    MuCh_Level = item.MuCh_Level,
                                    listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                            foreach (var item in aa)
                            {
                                if (item.Count() == 1)
                                {
                                    foreach (var item1 in item)
                                    {
                                        listselect2.Add(new MultipleChoiceselect
                                        {
                                            Sett_Id = item1.Sett_Id,
                                            MuCh_ID = item1.MuCh_ID,
                                            MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                            MuCh_Description = item1.MuCh_Description,
                                            MuCh_Feedback = item1.MuCh_Feedback,
                                            MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                            MuCh_Level = item1.MuCh_Level,
                                            listanswerM = item1.listanswerM.ToList()
                                        });
                                    }
                                }
                            }
                            var rnd = new Random();
                            var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                        }
                        else
                        {
                            foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Difícil))
                            {
                                listselect2.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item.Sett_Id,
                                    MuCh_ID = item.MuCh_ID,
                                    MuCh_NameQuestion = item.MuCh_NameQuestion,
                                    MuCh_Description = item.MuCh_Description,
                                    MuCh_Feedback = item.MuCh_Feedback,
                                    MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                    MuCh_Level = item.MuCh_Level,
                                    listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            var rnd = new Random();
                            var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                        }

                    }
                        if (cant == 15)
                        {
                            var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Fácil).ToList();
                            if (attuser.Count != 0)
                            {
                                foreach (var item1 in attuser)
                                {
                                    listselect.Add(new MultipleChoiceselect
                                    {
                                        Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                                        MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                                        MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                                        MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                                        MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                                        MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                                        MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                                        listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                                    });
                                }
                                foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Fácil))
                                {
                                    listselect.Add(new MultipleChoiceselect
                                    {
                                        Sett_Id = item.Sett_Id,
                                        MuCh_ID = item.MuCh_ID,
                                        MuCh_NameQuestion = item.MuCh_NameQuestion,
                                        MuCh_Description = item.MuCh_Description,
                                        MuCh_Feedback = item.MuCh_Feedback,
                                        MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                        MuCh_Level = item.MuCh_Level,
                                        listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                    });
                                }
                                var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                                foreach (var item in aa)
                                {
                                    if (item.Count() == 1)
                                    {
                                        foreach (var item1 in item)
                                        {
                                            listselect2.Add(new MultipleChoiceselect
                                            {
                                                Sett_Id = item1.Sett_Id,
                                                MuCh_ID = item1.MuCh_ID,
                                                MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                                MuCh_Description = item1.MuCh_Description,
                                                MuCh_Feedback = item1.MuCh_Feedback,
                                                MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                                MuCh_Level = item1.MuCh_Level,
                                                listanswerM = item1.listanswerM.ToList()
                                            });
                                        }
                                    }
                                }
                                var rnd = new Random();
                                var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                                var Newlistselect = randomselect.Take(1);
                                ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                            }
                            else
                            {
                                foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Fácil))
                                {
                                    listselect2.Add(new MultipleChoiceselect
                                    {
                                        Sett_Id = item.Sett_Id,
                                        MuCh_ID = item.MuCh_ID,
                                        MuCh_NameQuestion = item.MuCh_NameQuestion,
                                        MuCh_Description = item.MuCh_Description,
                                        MuCh_Feedback = item.MuCh_Feedback,
                                        MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                        MuCh_Level = item.MuCh_Level,
                                        listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                    });
                                }
                                var rnd = new Random();
                                var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                                var Newlistselect = randomselect.Take(1);
                                ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                            }

                        }

                        if (cant!=15)
                        {
                            foreach (var item in a)
                            {
                                if (item.Comodin == CMD.CMB)
                                {
                                    cm1 = 1;
                                }
                                if (item.Comodin == CMD.EST)
                                {
                                    cm2 = 2;
                                }
                            }
                        }
                }
                }                            
            }
            else
                        {
                        foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Fácil))
                        {
                            listselect.Add(new MultipleChoiceselect
                            {
                                Sett_Id = item.Sett_Id,
                                MuCh_ID = item.MuCh_ID,
                                MuCh_NameQuestion = item.MuCh_NameQuestion,
                                MuCh_Description = item.MuCh_Description,
                                MuCh_Feedback = item.MuCh_Feedback,
                                MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                MuCh_Level = item.MuCh_Level,
                                listanswerM = item.MG_AnswerMultipleChoice.ToList()
                            });
                        }
                        var rnd = new Random();
                        var randomselect = listselect.OrderBy(x => rnd.Next()).ToList();
                        var Newlistselect = randomselect.Take(1);
                         ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                       }                       
            Session["Date"] = DateTime.Now;

            QuestionSelect model1 = new QuestionSelect
            {
                Sett_Id = setting.Sett_Id,
                listquestionsselect = ListFinalfacil,
                setting = setting,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                cmd1 = cm1,
                cmd2=cm2,
                v1=cm1,
                v2=cm2
                
                     
                        };
                        return View(model1);
                        }
        /*
         * Metodo utilizado para validar las respuestas
         * del usuario a la pregunta seleccionada
         * Esté metodo recibe los datos necesarios 
         * para validar la respuesta de la pregunta
         * por parte del usuario
         */ 
        [Authorize]
        public ActionResult AnswerQuestions(int id,int AnwId,int id2)
        {                   
                       DateTime fechainicio = new DateTime();
                       fechainicio = (DateTime)Session["Date"];
                       DateTime fechaenvio = DateTime.Now;
                       int rango = (fechainicio- fechaenvio).Seconds;
                       rango = rango + 60;
                       var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var block1 = ApplicationDbContext.MG_BlockGameUser.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.BlGa_Id).ToList();
            if (block1.Count != 0)
            {
                var bu = block1.FirstOrDefault();
                if (bu.BlGa_Fecha > DateTime.Now)
                {
                    TempData["Info"] = "Usted está bloqueado por 24 horas.";
                    return RedirectToAction("Index", "Home");
                }
            }
            var answer = ApplicationDbContext.MG_AnswerMultipleChoice.Find(AnwId);
                        int cm1 = 0;
                        int cm2 = 0;
                        int v1 = 0;
                        int v2 = 0;
                        if (id <= 60)
                        {
                        if (ValidQuestion(AnwId) == true)
                        {
                        var puntos = 0;                   
                    var a = ApplicationDbContext.MG_AnswerMultipleChoice.Where(x => x.MuCh_ID == answer.MuCh_ID).ToList();
                    var z = a.Single(x => x.AnMul_TrueAnswer == OPTIONANSWER.Verdadero);
                    var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                    if (z.AnMul_ID == AnwId)
                    {
                        if (attempts.Count != 0)
                        {
                            var atte = attempts.FirstOrDefault();
                            var att1 = attempts.GroupBy(x => x.Attemps).ToList();
                            var gru = att1.FirstOrDefault();
                            if (atte.Respuesta == RESPUESTA.Incorrecta)
                            {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps + 1,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Correcta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio,
                                    Comodin = (CMD)id2
                                };
                                ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                puntos = 60;
                                MG_Point punto = new MG_Point
                                {
                                    point_pointOfUser = puntos + id,
                                    MG_SettingMp = z.MG_MultipleChoice.MG_SettingMp,
                                    Sett_Id = z.MG_MultipleChoice.Sett_Id,
                                    User_Id = user.Id,
                                    ApplicationUser = user
                                };
                                ApplicationDbContext.MG_Point.Add(punto);
                            }
                            else
                            {
                                int att = ApplicationDbContext.MG_AnswerUser.Where(x => x.Attemps == atte.Attemps && x.Respuesta == RESPUESTA.Correcta && x.User_Id == user.Id).Count();
                                if (att == 15)
                                {
                                    MG_AnswerUser antiguo = new MG_AnswerUser
                                    {
                                        User_Id = user.Id,
                                        Attemps = atte.Attemps +1,
                                        ApplicationUser = user,
                                        Respuesta = RESPUESTA.Correcta,
                                        AnMul_ID = answer.AnMul_ID,
                                        MG_AnswerMultipleChoice = answer,
                                        FechaIngreso = fechainicio,
                                        FechaEnvio = fechaenvio,
                                        Comodin = (CMD)id2
                                    };
                                    ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                    puntos = 60;
                                    MG_Point punto = new MG_Point
                                    {
                                        point_pointOfUser = puntos + id,
                                        MG_SettingMp = z.MG_MultipleChoice.MG_SettingMp,
                                        Sett_Id = z.MG_MultipleChoice.Sett_Id,
                                        User_Id = user.Id,
                                        ApplicationUser = user
                                    };
                                    ApplicationDbContext.MG_Point.Add(punto);
                                    ApplicationDbContext.SaveChanges();
                                    TempData["Info1"] = "Respuesta Correcta.";

                                }
                                if (att == 14)
                                {
                                    MG_AnswerUser antiguo = new MG_AnswerUser
                                    {
                                        User_Id = user.Id,
                                        Attemps = atte.Attemps,
                                        ApplicationUser = user,
                                        Respuesta = RESPUESTA.Correcta,
                                        AnMul_ID = answer.AnMul_ID,
                                        MG_AnswerMultipleChoice = answer,
                                        FechaIngreso = fechainicio,
                                        FechaEnvio = fechaenvio,
                                        Comodin = (CMD)id2
                                    };
                                    ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                    var pointuser = ApplicationDbContext.MG_Point.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.point_Id).ToList();
                                    var updatepoint = pointuser.FirstOrDefault();
                                    updatepoint.point_pointOfUser = updatepoint.point_pointOfUser + 60 + id;
                                    ApplicationDbContext.SaveChanges();
                                    foreach (var item in gru)
                                    {
                                        if (item.Comodin == CMD.CMB)
                                        {
                                            cm1 = 1;
                                        }
                                        if (item.Comodin == CMD.EST)
                                        {
                                            cm2 = 2;
                                        }
                                    }
                                    if (cm1 == 0)
                                    {
                                        v1 = 70;
                                    }
                                    if (cm2 == 0)
                                    {
                                        v2 = 70;
                                    }
                                    var point = ApplicationDbContext.MG_Point.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.point_Id).ToList(); ;
                                    var lastpoint = point.FirstOrDefault();
                                    lastpoint.point_pointOfUser = lastpoint.point_pointOfUser + v1 + v2;
                                    ApplicationDbContext.SaveChanges();
                                   
                                }
                                if (att < 14 )
                             
                                {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Correcta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio,
                                    Comodin = (CMD)id2
                                };
                                ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                var pointuser = ApplicationDbContext.MG_Point.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.point_Id).ToList();
                                var updatepoint = pointuser.FirstOrDefault();
                                updatepoint.point_pointOfUser = updatepoint.point_pointOfUser + 60 + id;
                                ApplicationDbContext.SaveChanges();
                                    TempData["Info1"] = "Respuesta Correcta.";
                                }
                            }
                        }
                        else
                        {
                            MG_AnswerUser nuevo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = 1,
                                ApplicationUser = user,
                                AnMul_ID = answer.AnMul_ID,
                                Respuesta = RESPUESTA.Correcta,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio,
                                Comodin = (CMD)id2
                            };
                            ApplicationDbContext.MG_AnswerUser.Add(nuevo);
                            ApplicationDbContext.SaveChanges();
                            puntos = 60;
                            MG_Point punto = new MG_Point
                            {
                                point_pointOfUser = puntos+ id,
                                MG_SettingMp = z.MG_MultipleChoice.MG_SettingMp,
                                Sett_Id = z.MG_MultipleChoice.Sett_Id,
                                User_Id = user.Id,
                                ApplicationUser = user
                            };
                            ApplicationDbContext.MG_Point.Add(punto);
                            TempData["Info1"] = "Respuesta Correcta.";
                        }
                       
                    }
                    else
                    {
                        if (attempts.Count != 0)
                        {
                            var att = attempts.GroupBy(x => x.Attemps).ToList();
                            var gru = att.FirstOrDefault();
                            var atte = attempts.FirstOrDefault();
                            if (atte.Respuesta == RESPUESTA.Incorrecta)
                            {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps + 1,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Incorrecta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio,
                                    Comodin = (CMD)id2
                                };
                                ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                var block = new MG_BlockGameUser
                                {
                                    ApplicationUser = user,
                                    User_Id = user.Id,
                                    BlGa_Fecha = fechaenvio.AddDays(1)
                                };
                                ApplicationDbContext.MG_BlockGameUser.Add(block);
                                ApplicationDbContext.SaveChanges();
                            }
                            else
                            {
                                int atta = ApplicationDbContext.MG_AnswerUser.Where(x => x.Attemps == atte.Attemps && x.Respuesta == RESPUESTA.Correcta && x.User_Id == user.Id).Count();
                                if (atta == 15)
                                {
                                    MG_AnswerUser antiguo = new MG_AnswerUser
                                    {
                                        User_Id = user.Id,
                                        Attemps = atte.Attemps + 1,
                                        ApplicationUser = user,
                                        Respuesta = RESPUESTA.Incorrecta,
                                        AnMul_ID = answer.AnMul_ID,
                                        MG_AnswerMultipleChoice = answer,
                                        FechaIngreso = fechainicio,
                                        FechaEnvio = fechaenvio,
                                        Comodin = (CMD)id2
                                    };
                                    ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                    var block = new MG_BlockGameUser
                                    {
                                        ApplicationUser = user,
                                        User_Id = user.Id,
                                        BlGa_Fecha = fechaenvio.AddDays(1)
                                    };
                                    ApplicationDbContext.MG_BlockGameUser.Add(block);
                                    ApplicationDbContext.SaveChanges();
                                }
                                else
                                {
                                    MG_AnswerUser antiguo = new MG_AnswerUser
                                    {
                                        User_Id = user.Id,
                                        Attemps = atte.Attemps,
                                        ApplicationUser = user,
                                        Respuesta = RESPUESTA.Incorrecta,
                                        AnMul_ID = answer.AnMul_ID,
                                        MG_AnswerMultipleChoice = answer,
                                        FechaIngreso = fechainicio,
                                        FechaEnvio = fechaenvio,
                                        Comodin = (CMD)id2
                                    };
                                    ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                    var block = new MG_BlockGameUser
                                    {
                                        ApplicationUser = user,
                                        User_Id = user.Id,
                                        BlGa_Fecha = fechaenvio.AddDays(1)
                                    };
                                    ApplicationDbContext.MG_BlockGameUser.Add(block);
                                    ApplicationDbContext.SaveChanges();
                                    var pnt = ApplicationDbContext.MG_Point.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.point_Id).ToList();
                                    var updatepoint = pnt.FirstOrDefault();
                                    ApplicationDbContext.MG_Point.Remove(updatepoint);
                                    ApplicationDbContext.SaveChanges();
                                }                            
                            }
                        }
                        else
                        {
                            MG_AnswerUser nuevo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = 1,
                                ApplicationUser = user,
                                Respuesta = RESPUESTA.Incorrecta,
                                AnMul_ID = answer.AnMul_ID,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio,
                                Comodin = (CMD)id2
                            };
                            ApplicationDbContext.MG_AnswerUser.Add(nuevo);
                            var block = new MG_BlockGameUser
                            {
                                ApplicationUser=user,
                                User_Id=user.Id,
                                BlGa_Fecha=fechaenvio.AddDays(1)
                            };
                            ApplicationDbContext.MG_BlockGameUser.Add(block);
                        }
                        ApplicationDbContext.SaveChanges();
                        TempData["Info"] = "Respuesta incorrecta has sido bloqueado por 24 horas y has perdido los puntos de tu ultimo intente.";
                        TempData["Info2"] = "Respuesta incorrecta has sido bloqueado por 24 horas y has perdido los puntos de tu ultimo intente.";
                        return RedirectToAction("Index","Home");
                    }
                    ApplicationDbContext.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Instructions", new { Id = answer.MG_MultipleChoice.Sett_Id });
                }
                        } else
                        {
                        TempData["Info"] = "No puede modifcar el tiempo";
                        return RedirectToAction("Instructions", new {Id=answer.MG_MultipleChoice.Sett_Id });
                        }           
            return RedirectToAction("Instructions", new {Id= answer.MG_MultipleChoice.Sett_Id });
        }
        /*
         * Metodo utilizado para validar la pregunta 
         * que ha respondido el usuario es decir, 
         * que dicha pregunta solo se puede contestar una vez
         * Esté metodo recibe el id de la respuesta 
         * para validar la información
         */ 
        public bool ValidQuestion(int AnwId)
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var ans = ApplicationDbContext.MG_AnswerMultipleChoice.Find(AnwId);
            var att = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id && x.Respuesta == RESPUESTA.Incorrecta).ToList().Count();
            if (att<ans.MG_MultipleChoice.MG_SettingMp.Sett_Attemps)
            {
                var valpre = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID == ans.MuCh_ID).ToList();
                if (valpre.Count != 0)
                {
                    TempData["Info"] = "Hay una inconsistencia con los datos que enviaste,por tanto has sido bloqueado por 24 horas y has perdido los puntos de tu ultimo intente.";
                    DateTime fechainicio = new DateTime();
                    fechainicio = (DateTime)Session["Date"];
                    DateTime fechaenvio = DateTime.Now;
                    var answer = ApplicationDbContext.MG_AnswerMultipleChoice.Find(AnwId);
                    var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                    if (attempts.Count != 0)
                    {
                        var atte = attempts.FirstOrDefault();
                        if (atte.Respuesta == RESPUESTA.Incorrecta)
                        {
                            MG_AnswerUser antiguo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = atte.Attemps + 1,
                                ApplicationUser = user,
                                Respuesta = RESPUESTA.Incorrecta,
                                AnMul_ID = answer.AnMul_ID,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio
                            };
                            ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                            var block = new MG_BlockGameUser
                            {
                                ApplicationUser = user,
                                User_Id = user.Id,
                                BlGa_Fecha = fechaenvio.AddDays(1)
                            };
                            ApplicationDbContext.MG_BlockGameUser.Add(block);
                            ApplicationDbContext.SaveChanges();
                        }
                        else
                        {
                            int atta = ApplicationDbContext.MG_AnswerUser.Where(x => x.Attemps == atte.Attemps && x.Respuesta == RESPUESTA.Correcta && x.User_Id == user.Id).Count();
                            if (atta == 15)
                            {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps + 1,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Incorrecta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio
                                };
                                ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                var block = new MG_BlockGameUser
                                {
                                    ApplicationUser = user,
                                    User_Id = user.Id,
                                    BlGa_Fecha = fechaenvio.AddDays(1)
                                };
                                ApplicationDbContext.MG_BlockGameUser.Add(block);
                                ApplicationDbContext.SaveChanges();
                            }
                            else
                            {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Incorrecta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio
                                };
                                ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                                var block = new MG_BlockGameUser
                                {
                                    ApplicationUser = user,
                                    User_Id = user.Id,
                                    BlGa_Fecha = fechaenvio.AddDays(1)
                                };
                                ApplicationDbContext.MG_BlockGameUser.Add(block);
                                ApplicationDbContext.SaveChanges();
                                var pnt = ApplicationDbContext.MG_Point.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.point_Id).ToList();
                                var updatepoint = pnt.FirstOrDefault();
                                ApplicationDbContext.MG_Point.Remove(updatepoint);
                                ApplicationDbContext.SaveChanges();
                            }
                           
                        }
                    }
                    else
                    {
                        MG_AnswerUser nuevo = new MG_AnswerUser
                        {
                            User_Id = user.Id,
                            Attemps = 1,
                            ApplicationUser = user,
                            Respuesta = RESPUESTA.Incorrecta,
                            AnMul_ID = answer.AnMul_ID,
                            MG_AnswerMultipleChoice = answer,
                            FechaIngreso = fechainicio,
                            FechaEnvio = fechaenvio
                        };
                        ApplicationDbContext.MG_AnswerUser.Add(nuevo);
                        var block = new MG_BlockGameUser
                        {
                            ApplicationUser = user,
                            User_Id = user.Id,
                            BlGa_Fecha = fechaenvio.AddDays(1)
                        };
                        ApplicationDbContext.MG_BlockGameUser.Add(block);
                        ApplicationDbContext.SaveChanges();
                    }
                    ApplicationDbContext.SaveChanges();

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                TempData["Info"] = "Ya no tiene más intentos disponibles.";
                return false;
            }
          
        }
        /*
         * Metodo utilizado para validar la respuesta 
         * del usuario cuando el tiempo para contestar
         * la pregunta se ha agotado
         * Esté metodo recibe el id de la respuesta para validar la 
         * información
         */ 
        [Authorize]
        public ActionResult AnswerQuestionsTime(int id)
        {
            DateTime fechainicio = new DateTime();
            fechainicio = (DateTime)Session["Date"];
            DateTime fechaenvio = DateTime.Now;
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var answer = ApplicationDbContext.MG_AnswerMultipleChoice.Find(id);
            var att = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id && x.Respuesta == RESPUESTA.Incorrecta).ToList().Count();
            if (att <answer.MG_MultipleChoice.MG_SettingMp.Sett_Attemps)
            {
                var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                if (attempts.Count != 0)
                {
                    var atte = attempts.FirstOrDefault();
                    if (atte.Respuesta == RESPUESTA.Incorrecta)
                    {
                        MG_AnswerUser antiguo = new MG_AnswerUser
                        {
                            User_Id = user.Id,
                            Attemps = atte.Attemps + 1,
                            ApplicationUser = user,
                            Respuesta = RESPUESTA.Incorrecta,
                            AnMul_ID = answer.AnMul_ID,
                            MG_AnswerMultipleChoice = answer,
                            FechaIngreso = fechainicio,
                            FechaEnvio = fechaenvio
                        };
                        ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                        var block = new MG_BlockGameUser
                        {
                            ApplicationUser = user,
                            User_Id = user.Id,
                            BlGa_Fecha = fechaenvio.AddDays(1)
                        };
                        ApplicationDbContext.MG_BlockGameUser.Add(block);
                        ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        int atta = ApplicationDbContext.MG_AnswerUser.Where(x => x.Attemps == atte.Attemps && x.Respuesta == RESPUESTA.Correcta && x.User_Id == user.Id).Count();
                        if (atta == 15)
                        {
                            MG_AnswerUser antiguo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = atte.Attemps + 1,
                                ApplicationUser = user,
                                Respuesta = RESPUESTA.Incorrecta,
                                AnMul_ID = answer.AnMul_ID,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio                         
                            };
                            ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                            var block = new MG_BlockGameUser
                            {
                                ApplicationUser = user,
                                User_Id = user.Id,
                                BlGa_Fecha = fechaenvio.AddDays(1)
                            };
                            ApplicationDbContext.MG_BlockGameUser.Add(block);
                            ApplicationDbContext.SaveChanges();
                        }
                        else
                        {
                            MG_AnswerUser antiguo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = atte.Attemps,
                                ApplicationUser = user,
                                Respuesta = RESPUESTA.Incorrecta,
                                AnMul_ID = answer.AnMul_ID,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio                             
                            };
                            ApplicationDbContext.MG_AnswerUser.Add(antiguo);
                            var block = new MG_BlockGameUser
                            {
                                ApplicationUser = user,
                                User_Id = user.Id,
                                BlGa_Fecha = fechaenvio.AddDays(1)
                            };
                            ApplicationDbContext.MG_BlockGameUser.Add(block);
                            ApplicationDbContext.SaveChanges();
                            var pnt = ApplicationDbContext.MG_Point.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.point_Id).ToList();
                            var updatepoint = pnt.FirstOrDefault();
                            ApplicationDbContext.MG_Point.Remove(updatepoint);
                            ApplicationDbContext.SaveChanges();
                        }                      
                    }
                }
                else
                {
                    MG_AnswerUser nuevo = new MG_AnswerUser
                    {
                        User_Id = user.Id,
                        Attemps = 1,
                        ApplicationUser = user,
                        Respuesta = RESPUESTA.Incorrecta,
                        AnMul_ID = answer.AnMul_ID,
                        MG_AnswerMultipleChoice = answer,
                        FechaIngreso = fechainicio,
                        FechaEnvio = fechaenvio
                    };
                    ApplicationDbContext.MG_AnswerUser.Add(nuevo);
                    var block = new MG_BlockGameUser
                    {
                        ApplicationUser = user,
                        User_Id = user.Id,
                        BlGa_Fecha = fechaenvio.AddDays(1)
                    };
                    ApplicationDbContext.MG_BlockGameUser.Add(block);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.SaveChanges();
                TempData["Info"] = "El tiempo para responder la pregunta se ha acabado, por tanto has sido bloqueado por 24 horas y has perdido los puntos de tu ultimo intente.";
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["Info"] = "Ya no tiene más intentos disponibles.";
                return RedirectToAction("Index", "Home");
            }
        }
        /*
         * Metodo utilizado para activar el diagrama 
         * de barras en la vista de la pregunta actual
         * Esté metodo recibe los datos necesarios para 
         * activar la vista de la grafica
         */ 
        [Authorize]
        public ActionResult EstadisticOption(int id, int Much_Id)
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var much = ApplicationDbContext.MG_MultipleChoice.Find(Much_Id);
            List<MultipleChoiceselect> listselect = new List<MultipleChoiceselect>();
            int cm1 = 0;
            int cm2 = 0;
          
            var grafic = JsonGraphics(much.MG_AnswerMultipleChoice.ToList());
            var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.AnUs_Id).ToList();
            if (attempts.Count !=0)
            {
               
                var att = attempts.GroupBy(x => x.Attemps).ToList();
                var gru = att.FirstOrDefault();
                var b = gru.FirstOrDefault();
                if (b.Respuesta == RESPUESTA.Incorrecta)
                {
                }
                else
                {
                    foreach (var item in gru)
                    {
                        if (item.Comodin == CMD.CMB)
                        {
                            cm1 = 1;
                        }
                        if (item.Comodin == CMD.EST)
                        {
                            cm2 = 2;
                        }
                    }
                }
                 
              
            }

            listselect.Add(new MultipleChoiceselect
            {
                Sett_Id = much.Sett_Id,
                MuCh_ID = much.MuCh_ID,
                MuCh_NameQuestion = much.MuCh_NameQuestion,
                MuCh_Description = much.MuCh_Description,
                MuCh_Feedback = much.MuCh_Feedback,
                MuCh_ImageQuestion = much.MuCh_ImageQuestion,
                MuCh_Level = much.MuCh_Level,
                listanswerM = much.MG_AnswerMultipleChoice.ToList()
            });
            QuestionSelect model1 = new QuestionSelect
            {
                Sett_Id = much.Sett_Id,
                listquestionsselect = listselect,
                setting = much.MG_SettingMp,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                seg = id,
                cmd2 =2,
                cmd1=cm1,
                Result=grafic
            };
            TempData["Estadis"] = "Felicidades has pasado el juego disfruta tus puntos.";
            return View("Question",model1);       
        }
        /*
         * Metodo utilizado para crear el diagrama de barras 
         * correpondiente a la pregunta
         * Esté metodo recibe un modelo con los datos necesarios 
         * para formar la grafica
         */ 
        [Authorize]
        public BarChart JsonGraphics(List<MG_AnswerMultipleChoice> Answer)
        {
            var ListJsonTest = Answer;        
            List<ComplexDataset> list = new List<ComplexDataset>();
            List<string> Opciones = new List<string>();
            Opciones.Add("A");
            Opciones.Add("B");
            Opciones.Add("C");
            Opciones.Add("D");
            List<double> scoresValues = new List<double>();
            scoresValues = listsco(Answer);
            list.Add(new ComplexDataset { Data = scoresValues, FillColor = "rgba(255, 86, 3, 1)" });
            BarChart chart = new BarChart();
            chart.ComplexData.Datasets = list;
            chart.ComplexData.Labels = Opciones;
            return chart;
        }
        /*
         * Metodo utilizado para obtener los datos reales para 
         * plasmarlos en el diagrama de barras actual
         * Esté metodo recibe una lista con todo los datos necesarios para
         * obtener estos valores 
         */ 
        public List<double> listsco(List<MG_AnswerMultipleChoice> Answer)
        {
            List<double> scoresValues = new List<double>();
            Random random = new Random();
            foreach (var item in Answer)
            {
                if (item.AnMul_TrueAnswer==OPTIONANSWER.Verdadero)
                {
                    int a = random.Next(60,80);
                    scoresValues.Add(a);
                }
                else
                {
                    int a = random.Next(70);
                    scoresValues.Add(a);
                }
            }
           
            return scoresValues;
        }
        /*
         * Metodo utilizado para activar el cambio de pregunta 
         * de un juego actual
         * Esté metodo recibe los datos necesarios para realizar el cambio de 
         * pregunta y retornarlo a la vista del juego en curso
         */ 
        [Authorize]
        public ActionResult CambioOption(int id, int Much_Id)
        {
            var useractual = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var setting = ApplicationDbContext.MG_SettingMp.First(x => x.Company_Id == useractual.CompanyId);
            //var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.AnUs_Id).GroupBy(x=>x.Attemps).ToList();
            var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.AnUs_Id).ToList();
            List<MultipleChoiceselect> listselect = new List<MultipleChoiceselect>();
            List<MultipleChoiceselect> listselect2 = new List<MultipleChoiceselect>();
            List<MultipleChoiceselect> ListFinalfacil = new List<MultipleChoiceselect>();
            int cm1 = 0;
            int cm2 = 0;
            if (attempts.Count != 0)
            {
                var att = attempts.GroupBy(x => x.Attemps).ToList();
                var a = att.FirstOrDefault();
                var b = a.FirstOrDefault();

                if (b.Respuesta == RESPUESTA.Incorrecta)
                {
                    var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Fácil).ToList();
                    foreach (var item1 in attuser)
                    {
                        listselect.Add(new MultipleChoiceselect
                        {
                            Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                            MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                            MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                            MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                            MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                            MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                            MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                            listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                        });
                    }
                    foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Fácil))
                    {
                        listselect.Add(new MultipleChoiceselect
                        {
                            Sett_Id = item.Sett_Id,
                            MuCh_ID = item.MuCh_ID,
                            MuCh_NameQuestion = item.MuCh_NameQuestion,
                            MuCh_Description = item.MuCh_Description,
                            MuCh_Feedback = item.MuCh_Feedback,
                            MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                            MuCh_Level = item.MuCh_Level,
                            listanswerM = item.MG_AnswerMultipleChoice.ToList()
                        });
                    }
                    var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                    foreach (var item in aa)
                    {
                        if (item.Count() == 1)
                        {
                            foreach (var item1 in item)
                            {
                                listselect2.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item1.Sett_Id,
                                    MuCh_ID = item1.MuCh_ID,
                                    MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                    MuCh_Description = item1.MuCh_Description,
                                    MuCh_Feedback = item1.MuCh_Feedback,
                                    MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                    MuCh_Level = item1.MuCh_Level,
                                    listanswerM = item1.listanswerM.ToList()
                                });
                            }
                        }

                    }
                    var rnd = new Random();
                    var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                    var Newlistselect = randomselect.Take(1);
                    ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                }
                else
                {
                    int cant = a.Count();

                    if (cant <= 3)
                    {
                        var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Fácil).ToList();
                        foreach (var item1 in attuser)
                        {
                            listselect.Add(new MultipleChoiceselect
                            {
                                Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                                MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                                MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                                MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                                MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                                MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                                MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                                listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                            });
                        }
                        foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Fácil))
                        {
                            listselect.Add(new MultipleChoiceselect
                            {
                                Sett_Id = item.Sett_Id,
                                MuCh_ID = item.MuCh_ID,
                                MuCh_NameQuestion = item.MuCh_NameQuestion,
                                MuCh_Description = item.MuCh_Description,
                                MuCh_Feedback = item.MuCh_Feedback,
                                MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                MuCh_Level = item.MuCh_Level,
                                listanswerM = item.MG_AnswerMultipleChoice.ToList()
                            });
                        }
                        var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                        foreach (var item in aa)
                        {
                            if (item.Count() == 1)
                            {
                                foreach (var item1 in item)
                                {
                                    listselect2.Add(new MultipleChoiceselect
                                    {
                                        Sett_Id = item1.Sett_Id,
                                        MuCh_ID = item1.MuCh_ID,
                                        MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                        MuCh_Description = item1.MuCh_Description,
                                        MuCh_Feedback = item1.MuCh_Feedback,
                                        MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                        MuCh_Level = item1.MuCh_Level,
                                        listanswerM = item1.listanswerM.ToList()
                                    });
                                }
                            }

                        }
                        var rnd = new Random();
                        var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                        var Newlistselect = randomselect.Take(1);
                        ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                    }
                    if (cant >= 4 && cant <= 9)
                    {
                        var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Medio).ToList();
                        if (attuser.Count != 0)
                        {
                            foreach (var item1 in attuser)
                            {
                                listselect.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                                    MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                                    MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                                    MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                                    MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                                    MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                                    MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                                    listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Medio))
                            {
                                listselect.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item.Sett_Id,
                                    MuCh_ID = item.MuCh_ID,
                                    MuCh_NameQuestion = item.MuCh_NameQuestion,
                                    MuCh_Description = item.MuCh_Description,
                                    MuCh_Feedback = item.MuCh_Feedback,
                                    MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                    MuCh_Level = item.MuCh_Level,
                                    listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                            foreach (var item in aa)
                            {
                                if (item.Count() == 1)
                                {
                                    foreach (var item1 in item)
                                    {
                                        listselect2.Add(new MultipleChoiceselect
                                        {
                                            Sett_Id = item1.Sett_Id,
                                            MuCh_ID = item1.MuCh_ID,
                                            MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                            MuCh_Description = item1.MuCh_Description,
                                            MuCh_Feedback = item1.MuCh_Feedback,
                                            MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                            MuCh_Level = item1.MuCh_Level,
                                            listanswerM = item1.listanswerM.ToList()
                                        });
                                    }
                                }
                            }
                            var rnd = new Random();
                            var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                        }
                        else
                        {
                            foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Medio))
                            {
                                listselect2.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item.Sett_Id,
                                    MuCh_ID = item.MuCh_ID,
                                    MuCh_NameQuestion = item.MuCh_NameQuestion,
                                    MuCh_Description = item.MuCh_Description,
                                    MuCh_Feedback = item.MuCh_Feedback,
                                    MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                    MuCh_Level = item.MuCh_Level,
                                    listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            var rnd = new Random();
                            var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                        }

                    }
                    if (cant >= 10 && cant <= 14)
                    {
                        var attuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level == LEVEL.Difícil).ToList();
                        if (attuser.Count != 0)
                        {
                            foreach (var item1 in attuser)
                            {
                                listselect.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                                    MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                                    MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                                    MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                                    MuCh_Feedback = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Feedback,
                                    MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                                    MuCh_Level = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Level,
                                    listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Difícil))
                            {
                                listselect.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item.Sett_Id,
                                    MuCh_ID = item.MuCh_ID,
                                    MuCh_NameQuestion = item.MuCh_NameQuestion,
                                    MuCh_Description = item.MuCh_Description,
                                    MuCh_Feedback = item.MuCh_Feedback,
                                    MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                    MuCh_Level = item.MuCh_Level,
                                    listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                            foreach (var item in aa)
                            {
                                if (item.Count() == 1)
                                {
                                    foreach (var item1 in item)
                                    {
                                        listselect2.Add(new MultipleChoiceselect
                                        {
                                            Sett_Id = item1.Sett_Id,
                                            MuCh_ID = item1.MuCh_ID,
                                            MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                            MuCh_Description = item1.MuCh_Description,
                                            MuCh_Feedback = item1.MuCh_Feedback,
                                            MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                            MuCh_Level = item1.MuCh_Level,
                                            listanswerM = item1.listanswerM.ToList()
                                        });
                                    }
                                }
                            }
                            var rnd = new Random();
                            var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                        }
                        else
                        {
                            foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Difícil))
                            {
                                listselect2.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item.Sett_Id,
                                    MuCh_ID = item.MuCh_ID,
                                    MuCh_NameQuestion = item.MuCh_NameQuestion,
                                    MuCh_Description = item.MuCh_Description,
                                    MuCh_Feedback = item.MuCh_Feedback,
                                    MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                                    MuCh_Level = item.MuCh_Level,
                                    listanswerM = item.MG_AnswerMultipleChoice.ToList()
                                });
                            }
                            var rnd = new Random();
                            var randomselect = listselect2.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
                        }

                    }

                    foreach (var item in a)
                    {
                        if (item.Comodin == CMD.CMB)
                        {
                            cm1 = 1;
                        }
                        if (item.Comodin == CMD.EST)
                        {
                            cm2 = 2;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Fácil))
                {
                    listselect.Add(new MultipleChoiceselect
                    {
                        Sett_Id = item.Sett_Id,
                        MuCh_ID = item.MuCh_ID,
                        MuCh_NameQuestion = item.MuCh_NameQuestion,
                        MuCh_Description = item.MuCh_Description,
                        MuCh_Feedback = item.MuCh_Feedback,
                        MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                        MuCh_Level = item.MuCh_Level,
                        listanswerM = item.MG_AnswerMultipleChoice.ToList()
                    });
                }
                var rnd = new Random();
                var randomselect = listselect.OrderBy(x => rnd.Next()).ToList();
                var Newlistselect = randomselect.Take(1);
                ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
            }
            Session["Date"] = DateTime.Now;

            QuestionSelect model1 = new QuestionSelect
            {
                Sett_Id = setting.Sett_Id,
                listquestionsselect = ListFinalfacil,
                setting = setting,
                Sesion = GetActualUserId().SesionUser,
                Logo = GetUrlLogo(),
                cmd1 = 1,
                cmd2 = cm2,
                v1 = cm1,
                v2 = cm2,
                seg=id
            };
            return View("Questions",model1);        
    }
        /*
         * Metodo utilizado para validar el tiempo 
         * en el que el usuario contesto la pregunta 
         * actual
         */ 
        [Authorize]
        public bool ValidTime()
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var attempts = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.AnUs_Id).ToList();
            var ult = attempts.FirstOrDefault();
            var answer = ApplicationDbContext.MG_AnswerMultipleChoice.Find(ult.MG_AnswerMultipleChoice.AnMul_ID);
            var att = ApplicationDbContext.MG_AnswerUser.Where(x => x.User_Id == user.Id && x.Respuesta == RESPUESTA.Incorrecta).ToList().Count();
            if (att < answer.MG_MultipleChoice.MG_SettingMp.Sett_Attemps)
            {
              
                if (attempts.Count != 0)
                {
                    var atte = attempts.FirstOrDefault();
                    int atta = ApplicationDbContext.MG_AnswerUser.Where(x => x.Attemps == atte.Attemps && x.Respuesta == RESPUESTA.Correcta && x.User_Id == user.Id).Count();
                    if (atta == 15)
                    {
                       
                        var block = new MG_BlockGameUser
                        {
                            ApplicationUser = user,
                            User_Id = user.Id,
                            BlGa_Fecha = DateTime.Now.AddDays(1)
                        };
                        ApplicationDbContext.MG_BlockGameUser.Add(block);
                        ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        atte.Respuesta = RESPUESTA.Incorrecta;
                        ApplicationDbContext.SaveChanges();
                        var block = new MG_BlockGameUser
                        {
                            ApplicationUser = user,
                            User_Id = user.Id,
                            BlGa_Fecha = DateTime.Now.AddDays(1)
                        };
                        ApplicationDbContext.MG_BlockGameUser.Add(block);
                        ApplicationDbContext.SaveChanges();
                        var pnt = ApplicationDbContext.MG_Point.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.point_Id).ToList();
                        var updatepoint = pnt.FirstOrDefault();
                        ApplicationDbContext.MG_Point.Remove(updatepoint);
                        ApplicationDbContext.SaveChanges();
                    }
                        
                }

               
                return true;
            }
            else
            {
         
                return false;
            }
        }
    }
}