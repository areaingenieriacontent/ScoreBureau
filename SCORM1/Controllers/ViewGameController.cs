using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Enum;
using SCORM1.Models;
using SCORM1.Models.PageCustomization;
using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Controllers
{
    public class ViewGameController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        public ViewGameController()
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
         * Metodo utilizado para cargar la 
         * previsualización del juego
         * Esté metdo recibe el id de la configuración del juego
         */ 
        [Authorize]
        public ActionResult Preview(int Id)
        {
            var setting = ApplicationDbContext.MG_SettingMp.Find(Id);
            AdminPreviw model = new AdminPreviw
            {
                setting=setting,
                Sett_Id=setting.Sett_Id,
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
        public ActionResult Instructions(int Id, int? contador)
        {
            var setting = ApplicationDbContext.MG_SettingMp.Find(Id);
            int facil=0;
            int medio=0;
            int dificil=0;
            int cont=0 ;
            if (contador ==null)
            {
                cont = 1;
            }
            else
            {
                cont = (int)contador;
            }
            List<MultipleChoiceFacil> ListQuestionsfacil = new List<MultipleChoiceFacil>();
            List<MultipleChoiceMedio> ListQuestionsmedio = new List<MultipleChoiceMedio>();
            List<MultipleChoiceDificil> ListQuestionsdificil = new List<MultipleChoiceDificil>();
            List<listranking> ListRanking = new List<listranking>();
            List<listranking> ListRankingFinal = new List<listranking>();
            if (setting.MG_MultipleChoice.Count != 0)
            {
                foreach (var item in setting.MG_MultipleChoice)
                {
                    switch (item.MuCh_Level)
                    {
                        case Enum.LEVEL.Fácil:
                            facil =facil+ 1;
                            break;
                        case Enum.LEVEL.Medio:
                            medio = 1+medio;
                            break;
                        case Enum.LEVEL.Difícil:
                            dificil = 1+dificil;
                            break;
                        default:
                            break;
                    }
                }

                foreach (var item in setting.MG_MultipleChoice.Where(x=>x.MuCh_Level==LEVEL.Fácil))
                {
                    ListQuestionsfacil.Add(new MultipleChoiceFacil
                    {
                       Sett_Id=item.Sett_Id,
                       MuCh_ID=item.MuCh_ID,
                       MuCh_NameQuestion=item.MuCh_NameQuestion,
                       MuCh_Description=item.MuCh_Description,
                       MuCh_Feedback=item.MuCh_Feedback,
                       MuCh_ImageQuestion=item.MuCh_ImageQuestion,
                       MuCh_Level=item.MuCh_Level,
                       listanswerM=item.MG_AnswerMultipleChoice.ToList()
                    });
                }
                foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Medio))
                {
                    ListQuestionsmedio.Add(new MultipleChoiceMedio
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
                foreach (var item in setting.MG_MultipleChoice.Where(x => x.MuCh_Level == LEVEL.Difícil))
                {
                    ListQuestionsdificil.Add(new MultipleChoiceDificil
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
            }
            var rnd = new Random();

            var randomlistfacil = ListQuestionsfacil.OrderBy(x => rnd.Next()).ToList();
            var Newlistfacil = randomlistfacil.Take(4);
            List<MultipleChoiceFacil> ListFinalfacil = new List<MultipleChoiceFacil>(Newlistfacil);

            var randomlistmedio = ListQuestionsmedio.OrderBy(x => rnd.Next()).ToList();
            var Newlistmedio = randomlistmedio.Take(6);
            List<MultipleChoiceMedio> ListFinalmedio = new List<MultipleChoiceMedio>(Newlistmedio);

            var randomlistdificil = ListQuestionsdificil.OrderBy(x => rnd.Next()).ToList();
            var Newlistdificil = randomlistdificil.Take(5);
            List<MultipleChoiceDificil> ListFinaldificil = new List<MultipleChoiceDificil>(Newlistdificil);
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
            AdminPreviw model = new AdminPreviw
            {
                setting = setting,
                Sett_Id = setting.Sett_Id,
                Logo = GetUrlLogo(),
                facil=facil,
                medio=medio,
                dificil=dificil,
                listquestionsFacil=ListFinalfacil,
                listquestionsMedio=ListFinalmedio,
                listquestionsDificil=ListFinaldificil,
                contador=cont,
                ListRanking=ListRankingFinal

            };
            TempData["Info"] = "El Juego solo se le habilitara al usuario cuanto todas las preguntas estén creadas por niveles. Nivel facil = total 15 preguntas, nivel medio = total 18 preguntas, nivel dificil =total 15 preguntas.";
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
        /*
         * Metodo tulizado para cargar 
         * la pregunta que se le visualiza al usaurio
         * Esté metodo recibe el id de la pregunta y la cantidad
         * de preguntas contestadas por el usuario
         */ 
        [Authorize]
        public ActionResult Question(int id, int contador)
        {
            var question = ApplicationDbContext.MG_MultipleChoice.Find(id);
            List<MultipleChoiceselect> listselect = new List<MultipleChoiceselect>();            
            if (question != null )
            {
                listselect.Add(new MultipleChoiceselect
                {
                    MuCh_ID=question.MuCh_ID,
                    MuCh_Description=question.MuCh_Description,
                    MuCh_Feedback=question.MuCh_Feedback,
                    MuCh_ImageQuestion=question.MuCh_ImageQuestion,
                    MuCh_Level=question.MuCh_Level,
                    MuCh_NameQuestion=question.MuCh_NameQuestion,
                    Sett_Id=question.Sett_Id,
                    listanswerM=question.MG_AnswerMultipleChoice.ToList()
                });
                QuestionSelect model = new QuestionSelect
                {
                    Sett_Id=question.Sett_Id,
                    contador=contador,
                    listquestionsselect=listselect,
                    setting=question.MG_SettingMp, 
                    Sesion=GetActualUserId().SesionUser,
                    Logo=GetUrlLogo()                   
                };
                return View(model);
            }
            return RedirectToAction("Instructions", new { Id = question.Sett_Id});
        }
        /*
         * Metodo utilizado para borrar todo los datos
         * del juego actual que han echo los usuarios
         * Esté metodo recibe el id de la configuración del juego
         */ 
        [Authorize]
        public ActionResult ResetGame(int id)
        {
            var setting = ApplicationDbContext.MG_SettingMp.Find(id);
            var ansuser = ApplicationDbContext.MG_AnswerUser.Where(x => x.ApplicationUser.Company.CompanyId == setting.Company_Id).ToList();
            var bloguser= ApplicationDbContext.MG_BlockGameUser.Where(x => x.ApplicationUser.Company.CompanyId == setting.Company_Id).ToList();
            var pointuse= ApplicationDbContext.MG_Point.Where(x => x.ApplicationUser.Company.CompanyId == setting.Company_Id).ToList();
            if (ansuser.Count != 0)
            {
                foreach (var item in ansuser)
                {
                    ApplicationDbContext.MG_AnswerUser.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            if (bloguser.Count != 0)
            {
                foreach (var item in bloguser)
                {
                    ApplicationDbContext.MG_BlockGameUser.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            if (pointuse.Count != 0)
            {
                foreach (var item in pointuse)
                {
                    ApplicationDbContext.MG_Point.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            TempData["Info"] = "Datos eliminados de forma éxitosa.";
            return RedirectToAction("Instructions", new { Id = setting.Sett_Id });
        }
    }
}