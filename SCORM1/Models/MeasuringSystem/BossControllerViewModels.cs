using Chart.Mvc.ComplexChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCORM1.Models.MeasuringSystem
{
    public class BossProfileViewModel : BaseViewModel
    {
        public ApplicationUser Boss { get; set; }
        public List<ApplicationUser> boss { get; set; }
        public String Selected { get; set; }
        public int Ranking { get; set; }
        public Result FirstResult { get; set; }
    }

    public class BossAreaViewModel : BaseViewModel
    {
        public ApplicationUser Boss { get; set; }
        public List<ApplicationUser> BossArea { get; set; }
        public List<ApplicationUser> BossUser { get; set; }
    }


    public class BossTestViewModel : BaseViewModel
    {
        public List<UserTestIndiv> PendanteMeasures { get; set; }
        public List<UserTestIndiv> FinishedMeasures { get; set; }
        public string UserId { get; set; }
        public int TestId { get; set; }
        public int MeasureId { get; set; }
        public List<Result> Results { get; set; }
        public List<AnswerQuestionData> Questions { get; set; }
        public bool validator { get; set; }
        //user to evaluate 
        public ApplicationUser UserEvaluate { get; set; }
        public string UserType { get; set; }
    }

    public class BossResultsViewModel : BaseViewModel
    {
        public Result FirstScore { get; set; }
        public List<string> ResultChart { get; set; }
        public List<string> ResultChartUser { get; set; }
        public string UserId { get; set; }
        public int NumberMesuare { get; set; }
    }
    public class Datasets
    {
        public List<double> data { get; set; }
        public string label { get; set; }
        public string borderColor { get; set; }
        public bool fill { get; set; }
    }
    public class BossPlansViewModel : BaseViewModel
    {
        public List<Plan> Plans { get; set; }
        public Result  FirstScore { get; set; }
    }
}