using Quartz;
using Quartz.Impl;
using System;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using SCORM1.Models;
using SCORM1.Enum;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Data.Entity;
using SCORM1.Models.MeasuringSystem;

namespace SCORM1.Controllers
{
    public class SendOfEmailsController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public SendOfEmailsController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        private static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            if (scheduler.IsStarted != true)
            {
                scheduler.Start();
                IJobDetail job = JobBuilder.Create<EmailJob>().Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInMinutes(15)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                      )
                    .Build();

                scheduler.ScheduleJob(job, trigger);
            }

        }

        private void SendEmail(List<UsuariosySusPlanes> Usuarios)
        {
            foreach (var usuario in Usuarios)
            {
                using (var message = new MailMessage("testingplenamente@gmail.com", usuario.Correo))
                {
                    var de = "testingplenamente@gmail.com";
                    var pass = "Sgaleano.1420";
                    message.Subject = "Recordatorio planes a completar";
                    message.Body = "el plan que te ha tocado resolver es: por favor accede al siguient link.";

                    foreach (var plans in usuario.planes_Asignados)
                    {
                        message.Body += Environment.NewLine + "https://localhost:44323/File/GetFile/" + usuario.IdUsuario + "-" + plans.PlanId;
                    }

                    try
                    {
                        using (SmtpClient client = new SmtpClient
                        {
                            EnableSsl = true,
                            Host = "smtp.gmail.com",
                            Port = 587,
                            Credentials = new NetworkCredential(de, pass)
                        })
                        {
                            client.Send(message);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }

        }


        private List<Plan> ObtenerPlanesporUsuario(string Id)
        {
            Result userLastResult = ApplicationDbContext.Results.Include(r => r.Scores).Where(r => r.QualifiedUser.Id == Id).OrderByDescending(r => r.ResultDate).FirstOrDefault();
            if (userLastResult != null)
            {
                List<Score> lowerScores = ObtainedScoreOfPlans(Id);
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

        public void MedicionRealizadas()
        {
            List<MeasureUser> ListMe = ApplicationDbContext.MeasureUser.ToList();
            List<UsuariosySusPlanes> n = new List<UsuariosySusPlanes>();
            foreach (var userActual in ListMe)
            {
                ApplicationUser Usuario = ApplicationDbContext.Users.Find(userActual.UserEvaluate);
                n.Add(new UsuariosySusPlanes
                {
                    Nombre = Usuario.FirstName,
                    Apellido = Usuario.LastName,
                    Correo = Usuario.Email,
                    IdUsuario = Usuario.Id,
                    Nombre_medicion = userActual.Measure.Test.TestDescription,
                    planes_Asignados = ObtenerPlanesporUsuario(Usuario.Id)
                });
            }
            SendEmail(n);
        }

        public void TriggerAplication()
        {
            SendOfEmailsController.Start();
        }
    }
    public class EmailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SendOfEmailsController send = new SendOfEmailsController();
            send.MedicionRealizadas();
        }
    }

}