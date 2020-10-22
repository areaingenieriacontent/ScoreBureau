using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using System.Data.Entity;
using SCORM1.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chart.Mvc.ComplexChart;
using System;
using SCORM1.Models.MeasuringSystem;

namespace SCORM1.Controllers
{
    public class ResultsController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public ResultsController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET: Results
        public ActionResult ViewResults(string userId, ROLES role)
        {
            List<Score> finishScores = GetUserLastScores(userId);
            UserResultsViewModel model = new UserResultsViewModel
            {
                ActualRole = role,
                FirstScore = new Result { Scores = finishScores },
                ResultChart = new List<string> { string.Join(",", JsonGraphics(userId)) },
                UserId = userId,
                ResultChartUser = GeneralGraphics(userId)
            };
            return View(model);
        }
        public List<string> GeneralGraphics(string userId)
        {
            try
            {
                List<Result> allresults = ApplicationDbContext.Results.Where(r => r.QualifiedUser.Id == userId).OrderByDescending(r => r.ResultDate).ToList();
                List<Result> UserPersonal = allresults.Where(z => z.Measure.Test.EvaluateTo == EVALUATE_TO.Personal).ToList();
                List<Result> UserIguales = allresults.Where(z => z.Measure.Test.EvaluateTo == EVALUATE_TO.Iguales).ToList();
                List<Result> UserSuperiores = allresults.Where(z => z.Measure.Test.EvaluateTo == EVALUATE_TO.Superiores).ToList();
                List<Result> UserACargo = allresults.Where(z => z.Measure.Test.EvaluateTo == EVALUATE_TO.ACargo).ToList();
                List<Result> UserClientes = allresults.Where(z => z.Measure.Test.EvaluateTo == EVALUATE_TO.Clientes).ToList();
                List<string> list = new List<string>();
                list.Add(string.Join(",", ObtainedScoreGraphics(UserPersonal)));
                list.Add(string.Join(",", ObtainedScoreGraphics(UserIguales)));
                list.Add(string.Join(",", ObtainedScoreGraphics(UserSuperiores)));
                list.Add(string.Join(",", ObtainedScoreGraphics(UserClientes)));
                list.Add(string.Join(",", ObtainedScoreGraphics(UserACargo)));
                return list;
            }
            catch (Exception e) { }
            return null;
        }
        private List<double> ObtainedScoreGraphics(List<Result> ListMesureOb)
        {
            List<double> finalScores = new List<double>();
            foreach (Proficiency proficiency in ApplicationDbContext.Proficiencies.ToList())
            {
                List<Score> proficiencyScores = ListMesureOb.SelectMany(ar => ar.Scores).Where(s => s.Proficiency == proficiency).ToList();
                if (proficiencyScores.Count > 0)
                {
                    finalScores.Add((int)proficiencyScores.Average(ps => ps.Value));
                }
                else
                {
                    finalScores.Add(0);
                }
            }
            return finalScores;
        }

        [Authorize]
        public List<Score> GetUserLastScores(string userId)
        {
            try
            {
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
            catch (Exception e)
            {
                List<Score> finalScores = new List<Score>();
                return finalScores;
            }
        }


        [Authorize]
        public List<double> JsonGraphics(string userId)
        {
            var ListJsonTest = GetUserLastScores(userId);
            List<double> scoresValues = new List<double>();
            foreach (var Score in ListJsonTest.ToList())
            {
                scoresValues.Add(Score.Value);
            }
            return scoresValues;
        }

        [Authorize]
        public List<Score> AverageOfScoresOfSpecificUser(List<Measure> measures, Proficiency proficiency, string userId)
        {
            List<Result> allresults = ApplicationDbContext.Results.Where(r => r.ResultOwner.Id == userId).ToList();
            List<Score> finalScores = new List<Score>();
            foreach (Measure measure in measures)
            {
                List<Score> proficiencyScores = allresults.SelectMany(ar => ar.Scores).Where(s => s.Proficiency == proficiency && s.Results.MeasureId == measure.MeasureId).ToList();
                finalScores.Add(new Score { Proficiency = proficiency, Value = (int)proficiencyScores.Average(ps => ps.Value), Results = proficiencyScores.Where(pr => pr.Results.MeasureId == measure.MeasureId).ToList().FirstOrDefault().Results });
            }
            return finalScores;
        }

        [Authorize]
        public LineChart GraphicsForUserProficiency(int id, string userId)
        {
            int companyId = ApplicationDbContext.Users.Find(userId).CompanyId.Value;
            Proficiency profiency = ApplicationDbContext.Proficiencies.Find(id);
            List<Measure> measuresList = ApplicationDbContext.Measures.Where(m => m.CompanyId == companyId).OrderByDescending(m => m.MeasureId).Take(10).ToList();
            List<Score> listJsonTest = AverageOfScoresOfSpecificUser(measuresList, profiency, userId);
            List<ComplexDataset> list = new List<ComplexDataset>();
            List<string> labels = new List<string>();
            List<double> scoresValues = new List<double>();
            foreach (Score score in listJsonTest.ToList())
            {
                scoresValues.Add(score.Value);
                labels.Add(score.Results.ResultDate.ToShortDateString());
            }
            list.Add(new ComplexDataset { Data = scoresValues, FillColor = "rgba(255, 86, 3, 1)" });
            LineChart chart = new LineChart();
            chart.ComplexData.Datasets = list;
            chart.ComplexData.Labels = labels;
            return chart;
        }

        //[Authorize]
        //public ActionResult GraphicsForUser(int GraphicId, string userId)
        //{
        //    var finishScores = GraphicsForUserProficiency(GraphicId, userId);
        //    UserResultsViewModel model = new UserResultsViewModel { ResultChartUser = finishScores };
        //    return PartialView("_GraphicsForUser", model);

        //}
        //[Authorize]
        //public ActionResult GraphicsForUserGeneral(string userId)
        //{
        //    var finishScores = JsonGraphics(userId);
        //    UserResultsViewModel model = new UserResultsViewModel { ResultChart = finishScores };
        //    return PartialView("_GraphicsForUser", model);
        //}
    }
}