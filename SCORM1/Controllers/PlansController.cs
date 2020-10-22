using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models;
using System.Data.Entity;
using SCORM1.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCORM1.Models.MeasuringSystem;

namespace SCORM1.Controllers
{
    public class PlansController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public PlansController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET: Plans
        [Authorize]
        public ActionResult ViewPlans(string userId, ROLES role)
        {
            var user = UserManager.FindById(userId);
            var companyId = user.CompanyId;

            UserPlansViewModel model = new UserPlansViewModel { ActualRole = role, Plans = GetLowerScores(userId) };
            return View("Plans", model);
        }

        public List<Plan> GetLowerScores(string userId)
        {
            Result userLastResult = ApplicationDbContext.Results.Include(r => r.Scores).Where(r => r.QualifiedUser.Id == userId).OrderByDescending(r => r.ResultDate).FirstOrDefault();
            if (userLastResult != null)
            {
                List<Score> lowerScores = ObtainedScoreOfPlans(userId);
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
    }
}