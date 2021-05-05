using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCORM1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models.ViewModel;
using SCORM1.Models.Survey;
using SCORM1.Models.Lms;
using System.Collections.Generic;
using System;

namespace SCORM1.Controllers
{
    public class SurveyController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public SurveyController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

        // GET: Survey
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Survey(int id)
        {
            string userId = GetActualUserId().Id;
            //Check if user already has a session, if the user has one, load it
            //------------------------
            SurveyModule survey = ApplicationDbContext.Surveys.Where(x => x.survey_id == id).FirstOrDefault();
            Module module = ApplicationDbContext.Modules.Find(survey.modu_id);
            Enrollment enrollment = ApplicationDbContext.Enrollments.Where(x => x.User_Id == userId && x.Modu_Id == module.Modu_Id).FirstOrDefault();
            UserSurveyResponse usr = GetUserSurvey(enrollment.Enro_Id);
            //now check if the session is still valid
            if (usr == null)
            {
                usr = new UserSurveyResponse
                {
                    calification = 0,
                    enro_id = enrollment.Enro_Id,
                    survey_initial_time = DateTime.Now,
                    survey_finish_time = DateTime.Now.AddMinutes(survey.survey_time_minutes)
                };
                ApplicationDbContext.UserSurveyResponses.Add(usr);
                ApplicationDbContext.SaveChanges();
            }
            //Else create a session and load whole survey
            SurveyQuestionBank sqb = ApplicationDbContext.SurveyQuestionBanks.Where(x => x.survey_id == survey.survey_id).FirstOrDefault();
            List<MultipleOptionsSurveyQuestion> mosq = ApplicationDbContext.MultipleOptionsSurveyQuestions.Where(x => x.bank_id == sqb.bank_id).ToList();
            List<MultipleOptionsSurveyAnswer> mosa = ApplicationDbContext.MultipleOptionsSurveyAnswers.ToList();//Check just load answers of questions
            List<TrueFalseSurveyQuestion> tfsq = ApplicationDbContext.TrueFalseSurveyQuestions.Where(x => x.bank_id == sqb.bank_id).ToList();

            //Populate the questions of the viewmodelto use in the view
            List<MultiOptionSurveyQuestion> listOfQuestions = new List<MultiOptionSurveyQuestion>();
            List<TrueFalseSurveyQuestion> listOfTFQuestions = new List<TrueFalseSurveyQuestion>();

            //These are for storing users answers
            List<MultipleOptionAnswer> ans = new List<MultipleOptionAnswer>();
            List<TrueFalseSurveyAnswer> tfa = new List<TrueFalseSurveyAnswer>();

            //Populate multiple option questions and prepare user answers
            for(int cont = 0; cont < mosq.Count; cont++)
            {
                MultiOptionSurveyQuestion questionToAdd = new MultiOptionSurveyQuestion();
                MultipleOptionAnswer ans2 = new MultipleOptionAnswer();
                questionToAdd.answers = new List<MultipleOptionsSurveyAnswer>();
                questionToAdd.question=mosq[cont];
                ans2.questionId = mosq[cont].mosq_id;
                ans2.answerId = 0;
                for (int cont2=0;cont2< mosa.Count; cont2++)
                {
                    if (mosa[cont2].mosq_id == mosq[cont].mosq_id)
                    {
                        questionToAdd.answers.Add(mosa[cont2]);
                    }
                }
                ans.Add(ans2);
                listOfQuestions.Add(questionToAdd);
            }

            //Populate True False questions and prepare user answers
            for(int cont = 0; cont < tfsq.Count; cont++)
            {
                listOfTFQuestions.Add(tfsq[cont]);
                TrueFalseSurveyAnswer a = new TrueFalseSurveyAnswer
                {
                    questionId = tfsq[cont].tfsq_id,
                    value = 2
                };
                tfa.Add(a);
            }

            SurveyViewModel model = new SurveyViewModel
            {
                survey = survey,
                questionBank = sqb,
                multipleOptionQuestions = listOfQuestions,
                userAnswers = ans,
                trueFalseSurveyQuestions = listOfTFQuestions,
                userAnswerTrueFalse = tfa
            };

            if (usr.survey_finish_time > DateTime.Now)
            {
                model.validSession = true;
            }
            else
            {
                model.validSession = false;
            }
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }

        public UserSurveyResponse GetUserSurvey(int enrollMentId)
        {
            if (ApplicationDbContext.UserSurveyResponses.Any(x => x.enro_id == enrollMentId))
            {
                UserSurveyResponse usr = ApplicationDbContext.UserSurveyResponses.Where(x => x.enro_id == enrollMentId).OrderBy(x => x.survey_initial_time).FirstOrDefault();
                return usr;
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SurveyResponse(SurveyViewModel model)
        {
            //To-Do calculate results
            UserSurveyResponse usr = ApplicationDbContext.UserSurveyResponses.Where(x=>x.enro_id==model.enro_id).FirstOrDefault();
            for(int cont = 0; cont < model.userAnswers.Count; cont++)
            {
                MultipleOptionsSurveyUser answer = new MultipleOptionsSurveyUser
                {

                };
            }
            model.Sesion = GetActualUserId().SesionUser;
            return View();
        }
    }
}