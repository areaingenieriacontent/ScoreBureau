using SCORM1.Enum;
using SCORM1.Models.Lms;
using SCORM1.Models.RigidCourse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ViewModel
{
    public class CursoRigidoViewModel : BaseViewModel
    {

        public IEnumerable<SelectListItem> AnswerPairing { get; set; }
        //topic
        public TopicsCourse topic { get; set; }
        public Module module { get; set; }
        public List<TopicsCourse> ListTopic { get; set; }
        public List<ApplicationUser> ListUser { get; set; }
        public List<Module> ListModule { get; set; }
        public int ToCo_Id { get; set; }
        //bank question
        public List<FlashTest> ListFlashTest { get; set; }
        public FlashTest flashTest { get; set; }
        public int FlashTestId { get; set; }
        [Display(Name = "Nombre Prueba")]
        public string BaQu_Name { get; set; }
        [Display(Name = "Contenido")]
        [AllowHtml]
        public string BaQu_Content { get; set; }
        [Display(Name = "Total Preguntas")]
        public int BaQu_TotalQuestion { get; set; }
        [Display(Name = "Preguntas certificación")]
        public int BaQu_QuestionUser { get; set; }
        public int TotalQuestion { get; set; }
        [Display(Name = "Seleccionar Preguntas")]
        public FORO BaQu_SelectQuestion { get; set; }


        //Option Multiple
        public List<FlashQuestion> flashQuestions { get; set; }
        public int FlashQuestionId { get; set; }
        [Display(Name = "Pregunta")]
        public string Enunciado { get; set; }


        //Answer Option Multiple
        public List<FlashQuestionAnswer> ListFlashQuestionAnswer { get; set; }
        public FlashQuestionAnswer flashQuestionAnswer { get; set; }
        public int FlashQuestionAnswerId { get; set; }
        [Display(Name = "Respuesta")]
        public string Content { get; set; }
        public int CorrectAnswer { get; set; }

        //User Answers
        public List<int> userAnswers { get; set; }

    }
}