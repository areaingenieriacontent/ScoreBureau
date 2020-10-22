using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SCORM1.Enum;
using Chart.Mvc.ComplexChart;
using System.Net.Mail;
using SCORM1.Models.MeasuringSystem;

namespace SCORM1.Controllers
{
    public class UserPlenamenteController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public UserPlenamenteController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        public new ActionResult Profile(UserProfileViewModel model)
        {
            var companyId = GetActualUserId().CompanyId;
            var getUserProfile = GetActualUserId().Id;
            List<ApplicationUser> USER = ApplicationDbContext.Users.Where(x => x.Id == getUserProfile).ToList();
            List<Score> ScoreObtained = GetUserLastScores();
            Result resultUser = ApplicationDbContext.Results.Where(r => r.QualifiedUser.Id == getUserProfile).OrderByDescending(r => r.ResultDate).FirstOrDefault();
            if (resultUser != null)
            {
                model = new UserProfileViewModel { ActualRole = GetActualUserId().Role, user = USER, FirstResult = new Result { Scores = ScoreObtained }, Ranking = ObtainedRankin(getUserProfile, companyId), Sesion = GetActualUserId().SesionUser };
                return View(model);
            }
            else
            {
                model = new UserProfileViewModel { ActualRole = GetActualUserId().Role, user = USER, FirstResult = null , Sesion = GetActualUserId().SesionUser };
                return View(model);
            }
        }
        public int ObtainedRankin(string getUserProfile, int? companyId)
        {
            List<Result> resultList = ApplicationDbContext.Results.Include(r => r.QualifiedUser).Where(r => r.QualifiedUser.CompanyId == companyId).OrderByDescending(r => r.ResultTotalScore).ToList();
            List<ClassRankingTemporal> nS = new List<ClassRankingTemporal>();
            foreach (var item in resultList)
            {
                if (nS.Any(x => x.id == item.QualifiedUser.Id))
                { }
                else
                {
                    nS.Add(new ClassRankingTemporal { id = item.QualifiedUser.Id, num = (int)resultList.Where(x => x.QualifiedUser.Id == item.QualifiedUser.Id).ToList().Average(x => x.ResultTotalScore) });
                }
            }
            nS.OrderBy(x => x.num);
            int ranking = 0;
            for (int i = 0; i < nS.Count; i++)
            {
                if (nS[i].id == getUserProfile)
                {
                    ranking = i + 1;
                }
            }
            return ranking;
        }
        private class ClassRankingTemporal
        {
            public string id { get; set; }
            public int num { get; set; }
        }

        [Authorize]
        public ActionResult Plans()
        {
            var userID = GetActualUserId().Id;
            UserPlansViewModel model = new UserPlansViewModel { ActualRole = GetActualUserId().Role, Plans = GetLowerScores(), userId =GetActualUserId().Id, Sesion = GetActualUserId().SesionUser };
            return View(model);
        }

        public List<Plan> GetLowerScores()
        {
            var userID = GetActualUserId().Id;
            Result userLastResult = ApplicationDbContext.Results.Include(r => r.Scores).Where(r => r.QualifiedUser.Id == userID).OrderByDescending(r => r.ResultDate).FirstOrDefault();
            if (userLastResult != null)
            {
                List<Score> lowerScores = ObtainedScoreOfPlans(userID);
                List<Plan> selectPlans = new List<Plan>();
                foreach (Score score in lowerScores)
                {
                    foreach (Plan plan in ApplicationDbContext.Plans.Where(p => p.PlanTo == PLAN_TO.Empleados))
                    {
                        if ((plan.PlanMinScore <= score.Value && plan.PlanMaxScore >= score.Value) && plan.ProficiencyId == score.Proficiency.ProficiencyId)
                        {
                            selectPlans.Add(plan);
                        }
                    }
                }
                return selectPlans;
            }
            return new List<Plan>();
        }

        private List<Score> ObtainedScoreOfPlans(string getUserProfile)
        {
            List<Result> userLastResult = ApplicationDbContext.Results.Include(x => x.Scores).Where(x => x.QualifiedUser.Id == getUserProfile).OrderByDescending(r => r.ResultDate).ToList();
            List<Score> finalScores = new List<Score>();

            foreach (Proficiency proficiency in ApplicationDbContext.Proficiencies.ToList())
            {
                List<Score> proficiencyScores = userLastResult.SelectMany(ar => ar.Scores).Where(s => s.Proficiency == proficiency).ToList();
                finalScores.Add(new Score { Proficiency = proficiency, Value = (int)proficiencyScores.Average(ps => ps.Value) });
            }
            List<Score> Scores = finalScores.OrderBy(x => x.Value).Take(3).ToList();
            return Scores;
        }

        [Authorize]
        public ActionResult Results()
        {
            List<Score> finishScores = GetUserLastScores();
            ApplicationUser user = UserManager.FindById(GetActualUserId().Id);
            int Numbers = user.Measures.ToList().Count();

            UserResultsViewModel model = new UserResultsViewModel
            {
                ActualRole = GetActualUserId().Role,
                FirstScore = new Result { Scores = finishScores },
                ResultChart = new List<string> { string.Join(",", JsonGraphics()) },
                NumberMesuare = Numbers,
                Sesion = GetActualUserId().SesionUser
            };
            return View(model);
        }
        

        [Authorize]
        public List<Score> GetUserLastScores()
        {
            try
            {
                var userId = GetActualUserId().Id;
                List<Result> allresults = ApplicationDbContext.Results.Where(r => r.QualifiedUser.Id == userId).OrderByDescending(r => r.ResultDate).ToList();
                List<Score> finalScores = new List<Score>();
                foreach (var item in allresults)
                {
                    foreach (Proficiency proficiency in ApplicationDbContext.Proficiencies.ToList())
                    {
                        List<Score> proficiencyScores = item.Scores.Where(s => s.Proficiency == proficiency).ToList();
                        finalScores.Add(new Score { Proficiency = proficiency, Value = (int)proficiencyScores.Average(ps => ps.Value) });
                    }
                }
                List<Score> finalScores2 = new List<Score>();

                foreach (var item in finalScores)
                {
                    if (finalScores2.Any(z => z.Proficiency.ProficiencyId == item.Proficiency.ProficiencyId))
                    { }
                    else
                    {
                        finalScores2.Add(new Score { Proficiency = item.Proficiency, Value = (int)finalScores.Where(a => a.Proficiency.ProficiencyId == item.Proficiency.ProficiencyId).ToList().Average(z => z.Value) });
                    }
                }
                return finalScores2;
            }
            catch (Exception)
            {
                List<Score> finalScores = new List<Score>();
                return finalScores;
            }
        }
        [Authorize]
        public List<double> JsonGraphics()
        {
            var ListJsonTest = GetUserLastScores();
            List<double> scoresValues = new List<double>();
            foreach (var Score in ListJsonTest.ToList())
            {
                scoresValues.Add(Score.Value);
            }
            return scoresValues;
        }

        [Authorize]
        public List<Score> AverageOfScoresOfSpecificUser(List<Measure> measures, Proficiency proficiency)
        {
            var userId = GetActualUserId().Id;
            List<Result> allresults = ApplicationDbContext.Results.Where(r => r.ResultOwner.Id == userId).ToList();
            List<Score> finalScores = new List<Score>();
            foreach (Measure measure in measures)
            {
                List<Score> proficiencyScores = allresults.SelectMany(ar => ar.Scores).Where(s => s.Proficiency == proficiency && s.Results.MeasureId == measure.MeasureId).ToList();
                finalScores.Add(new Score { Proficiency = proficiency, Value = (int)proficiencyScores.Average(ps => ps.Value), Results = proficiencyScores.Where(pr => pr.Results.MeasureId == measure.MeasureId).ToList().FirstOrDefault().Results });
            }
            return finalScores;
        }

        //[Authorize]
        //public LineChart GraphicsForUserProficiency(int id)
        //{
        //    int companyId = GetActualUserId().CompanyId.Value;
        //    Proficiency profiency = ApplicationDbContext.Proficiencies.Find(id);
        //    ApplicationUser user = UserManager.FindById(GetActualUserId().Id);
        //    List<Measure> measuresList = user.Measures.Where(x=>x.UserId==user).ToList();
        //    List<Score> listJsonTest = AverageOfScoresOfSpecificUser(measuresList, profiency);
        //    List<ComplexDataset> list = new List<ComplexDataset>();
        //    List<string> labels = new List<string>();
        //    List<double> scoresValues = new List<double>();
        //    foreach (Score score in listJsonTest.ToList())
        //    {
        //        scoresValues.Add(score.Value);
        //        labels.Add(score.Results.ResultDate.ToShortDateString());
        //    }
        //    list.Add(new ComplexDataset { Data = scoresValues, FillColor = "rgba(255, 86, 3, 1)" });
        //    LineChart chart = new LineChart();
        //    chart.ComplexData.Datasets = list;
        //    chart.ComplexData.Labels = labels;
        //    return chart;
        //}

        //  [Authorize]
        //public ActionResult GraphicsForUser(int GraphicId)
        //{
        //    var finishScores = GraphicsForUserProficiency(GraphicId);
        //    UserResultsViewModel model = new UserResultsViewModel { ResultChartUser = finishScores };
        //    return PartialView("_GraphicsForUser", model);

        //}
        //[Authorize]
        //public ActionResult GraphicsForUserGeneral()
        //{
        //    var finishScores = JsonGraphics();
        //    UserResultsViewModel model = new UserResultsViewModel { ResultChart = finishScores };
        //    return PartialView("_GraphicsForUser", model);
        //}

        [Authorize]
        public ActionResult Tests()
        {
            int companyId = GetActualUserId().Company.CompanyId;
            string userId = GetActualUserId().Id;
            ApplicationUser users = ApplicationDbContext.Users.Find(userId);

            List<Measure> PendanteMeasures = ApplicationDbContext.Measures.Where(m => m.CompanyId == companyId && m.MeasureInitDate <= DateTime.Now && m.MeasureFinishDate >= DateTime.Now).ToList();
            List<UserTestIndiv> Measurefinish = new List<UserTestIndiv>();
            foreach (var item in PendanteMeasures)
            {
                switch (item.Test.EvaluateTo)
                {
                    case EVALUATE_TO.ACargo:
                        for (int i = 0; i < users.MyOfficeUsers.Count(); i++)
                        {
                            string s = users.MyOfficeUsers.ElementAt(i).Id;
                            if (ApplicationDbContext.MeasureUser.Where(x => x.UserEvaluate == s).FirstOrDefault() == null)
                            {
                                Measurefinish.Add(new UserTestIndiv { User = users.MyOfficeUsers.ElementAt(i), description = "Colaboradores", Measure = item });
                            }
                        }
                        break;
                    case EVALUATE_TO.Clientes:
                        for (int i = 0; i < users.ClientsUsers.Count(); i++)
                        {
                            string s = users.ClientsUsers.ElementAt(i).Id;
                            if (ApplicationDbContext.MeasureUser.Where(x => x.UserEvaluate == s).FirstOrDefault() == null)
                            {
                                Measurefinish.Add(new UserTestIndiv { User = users.ClientsUsers.ElementAt(i), description = "Cliente", Measure = item });
                            }
                        }
                        break;
                    case EVALUATE_TO.Iguales:
                        for (int i = 0; i < users.EqualUsers.Count(); i++)
                        {
                            string s = users.EqualUsers.ElementAt(i).Id;
                            var zz = ApplicationDbContext.MeasureUser.Where(x => x.UserEvaluate == s).FirstOrDefault();
                            if (zz == null)
                            {
                                Measurefinish.Add(new UserTestIndiv { User = users.EqualUsers.ElementAt(i), description = "Compañero", Measure = item });
                            }
                        }
                        break;
                    case EVALUATE_TO.Personal:
                        var User = GetActualUserId();
                        if (ApplicationDbContext.MeasureUser.Where(x => x.UserEvaluate == User.Id && x.UsersId == User.Id).FirstOrDefault() == null)
                        {
                            Measurefinish.Add(new UserTestIndiv { User = User, description = "Personal", Measure = item });
                        }
                        break;
                    case EVALUATE_TO.Superiores:
                        for (int i = 0; i < users.SuperiorUsers.Count(); i++)
                        {
                            string s = users.SuperiorUsers.ElementAt(i).Id;
                            if (ApplicationDbContext.MeasureUser.Where(x => x.UserEvaluate == s).FirstOrDefault() == null)
                            {
                                Measurefinish.Add(new UserTestIndiv { User = users.SuperiorUsers.ElementAt(i), description = "Jefe", Measure = item });
                            }
                        }
                        break;
                }

            }
            UserTestViewModel model = new UserTestViewModel
            {
                ActualRole = GetActualUserId().Role,
                UserTestId = userId,
                PendanteMeasures = Measurefinish,
                Results = ApplicationDbContext.Results.Where(r => r.ResultOwner.Id == userId).ToList(),
                Sesion= GetActualUserId().SesionUser
            };
            return View(model);
        }


        public ActionResult Test(int testId, string userId, int measureId, string UserEvaluated, string UserType)
        {
            List<AnswerQuestionData> modelQuestions = new List<AnswerQuestionData>();
            List<Question> questions = ApplicationDbContext.Tests.Find(testId).Questions.ToList();
            foreach (Question question in questions)
            {
                modelQuestions.Add(new AnswerQuestionData
                {
                    QuestionId = question.QuestionId,
                    QuestionDescription = question.QuestionDescription,
                    ProficiencyId = question.Proficiency.ProficiencyId,
                    QuestionType = question.QuestionType,
                    Assigned = 0
                });
            }
            ApplicationUser UserEvaluate = ApplicationDbContext.Users.Find(UserEvaluated);
            UserTestViewModel model = new UserTestViewModel
            {
                ActualRole = GetActualUserId().Role,
                UserTestId = userId,
                MeasureId = measureId,
                TestId = testId,
                Questions = modelQuestions,
                UserEvaluate = UserEvaluate,
                UserType = UserType,
                Sesion = GetActualUserId().SesionUser
            };
            return View(model);
        }
        
        [HttpPost]
        public ActionResult FinishedTest(UserTestViewModel model)
        {
            if (ModelState.IsValid)
            {
                Measure finishedMeasure = ApplicationDbContext.Measures.Find(model.MeasureId);
                MeasureUser InsertUser = new MeasureUser
                {
                    UsersId = model.UserTestId,
                    UserEvaluate = model.UserEvaluate.Id,
                    MausureId = model.MeasureId
                };
                finishedMeasure.CompletedUsers.Add(InsertUser);
                List<Proficiency> proficiencies = ApplicationDbContext.Proficiencies.ToList();
                List<Score> scores = new List<Score>();

                foreach (Proficiency proficiency in proficiencies)
                {
                    List<AnswerQuestionData> questions = model.Questions.Where(q => q.ProficiencyId == proficiency.ProficiencyId).ToList();
                    scores.Add(new Score
                    {
                        Proficiency = proficiency,
                        Value = ReturnQuestionsProficiencyValue(questions)
                    });
                }
                Result newResult = new Result
                {
                    ResultDate = DateTime.Now,
                    ResultOwner = ApplicationDbContext.Users.Find(model.UserTestId),
                    QualifiedUser = ApplicationDbContext.Users.Find(model.UserEvaluate.Id),
                    ResultTotalScore = GetTotalScoreOfResults(scores),
                    Measure = ApplicationDbContext.Measures.Find(model.MeasureId),
                    Scores = scores
                };
                ApplicationDbContext.Results.Add(newResult);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Tests");

            }
            else
            {
                return RedirectToAction("Test", new { testId = model.TestId, userId = model.UserTestId, measureId = model.MeasureId });
            }

        }

        private int GetTotalScoreOfResults(List<Score> scores)
        {
            int value = 0;
            foreach (Score score in scores)
            {
                value = value + score.Value;
            }
            value = value / scores.Count;
            return value;
        }

        private int ReturnQuestionsProficiencyValue(List<AnswerQuestionData> questions)
        {
            double valueObtained = 0;
            double ValueAssigned = questions.Count*2.5;
            foreach (AnswerQuestionData question in questions)
            {
                switch (question.Assigned)
                {
                    case 1:
                        valueObtained = valueObtained + 0;
                        break;
                    case 2:
                        valueObtained = valueObtained + 1;
                        break;
                    case 3:
                        valueObtained = valueObtained + 2;
                        break;
                    case 4:
                        valueObtained = valueObtained + 2.5;
                        break;
                }
            }
            return (int)((100*valueObtained)/ValueAssigned) ;
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }
    }
}