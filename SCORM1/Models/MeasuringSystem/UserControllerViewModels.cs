using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCORM1.Enum;
using Chart.Mvc.ComplexChart;

namespace SCORM1.Models.MeasuringSystem
{
    public class UserProfileViewModel : BaseViewModel
    {
        public ApplicationUser User { get; set; }
        public List<ApplicationUser> user { get; set; }
        public int Ranking { get; set; }
        public Result FirstResult { get; set; }
        public Result ActualResult { get; set; }
        public String Selected { get; set; }
        public List<Proficiency> profiency { get; set; }
        public List<Result> resultprofile { get; set; }
    }
    public class UserTestIndiv
    {
        public ApplicationUser User { get; set; }
        public Measure Measure { get; set; }
        public string description { get; set; }
    }

    public class UserTestViewModel : BaseViewModel
    {

        public List<UserTestIndiv> PendanteMeasures { get; set; }
        public List<UserTestIndiv> FinishedMeasures { get; set; }
        public string UserTestId { get; set; }
        public int TestId { get; set; }
        public int MeasureId { get; set; }
        public List<Result> Results { get; set; }
        public List<AnswerQuestionData> Questions { get; set; }
        public bool validator { get; set; }
        //user to evaluate 
        public ApplicationUser UserEvaluate { get; set; }
        public string UserType { get; set; }
    }

    public class AnswerQuestionData
    {
        public int QuestionId { get; set; }
        public string QuestionDescription { get; set; }
        public int Assigned { get; set; }
        public QUESTION_TYPE QuestionType { get; set; }
        public int ProficiencyId { get; set; }
    }

    public class UserResultsViewModel : BaseViewModel
    {
        public Result FirstScore { get; set; }
        public List<string> ResultChart { get; set; }
        public List<string> ResultChartUser { get; set; }
        public string UserId { get; set; }
        public int NumberMesuare { get; set; }

    }

    public class UserPlansViewModel : BaseViewModel
    {
        public List<Plan> Plans { get; set; }
    }
}