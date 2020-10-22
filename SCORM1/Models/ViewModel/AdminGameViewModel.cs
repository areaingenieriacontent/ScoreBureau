using SCORM1.Enum;
using SCORM1.Models.MainGame;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCORM1.Models.ViewModel
{
    public class AdminGameViewModel
    {
    }

    public class AdminGameSetting : BaseViewModel
    {
        //Variable utilizada para asignar el nombre del documento de los terminos y condiciones
        [Display(Name = "Terminos del juego")]
        public string TermsGame { get; set; }
        //Lista de las configuraciónes de los juegos creadas
        public List<MG_SettingMp> ListSetting { get; set;}
        //Variable utilizada para asignar el id del juego
        public int Sett_Id { get; set; }
        //Variable utilizada para asignar los intentos del juego
        [Display(Name = "Intentos")]
        [Required(ErrorMessage = "Se Debe Contar con un número de intentos disponibles")]
        public int Sett_Attemps { get; set; }
        //Variable utilizada para asignar la fecha de inicio el juego
        [Display(Name = "Fecha de Inicio")]
        [Required(ErrorMessage = "Se Debe Contar con una fecha inicial")]
        [DataType(DataType.Date)]
        public DateTime Sett_InitialDate { get; set; }
        //Variable utilizada para asignar la fecha de finalización del juego
        [Display(Name = "Fecha de Finalización")]
        [Required(ErrorMessage = "Se Debe Contar con una fecha de finalización")]
        [DataType(DataType.Date)]
        public DateTime Sett_CloseDate { get; set; }
        //Variable utilizada para asignar el número de preguntas del juego
        [Display(Name = "Preguntas")]
        public int Sett_NumberOfQuestions { get; set; }
        //Variable utilizada para asignar el id de la plantilla del juego
        [Display(Name = "Plantilla")]
        [Required(ErrorMessage = "Se Debe Contar con una plantilla para el juego")]
        public int Plan_Id { get; set; }
        //Lista desplegable de las plantillas del juego
        public IEnumerable<SelectListItem> Sett_Templates { get; set; }
        //Lista desplegable de los audios del juego
        public IEnumerable<SelectListItem> Sett_Audios { get; set; }
        //Variable utilizada para asignar el auido de instrucciones
        [Display(Name = "Audio_Instruciones")]
        public string Sett_Audio1 { get; set; }
        //Variable utilizada para asignar el auido de de respuestas
        [Display(Name = "Audio_Respuesta")]
        public string Sett_Audio2 { get; set; }
        //Variable utilizada para asignar el auido de inicio
        [Display(Name = "Audio_Inicio")]
        public string Sett_Audio3 { get; set; }
        //Variable utilizada para asignar el auido de de pregunta
        [Display(Name = "Audio_Pregunta")]
        public string Sett_Audio4 { get; set; }
        //Variable utilizada para asignar el auido de ganador
        [Display(Name = "Audio_Ganador")]
        public string Sett_Audio5 { get; set; }
    }

    public class AdminTemplate : BaseViewModel
    {
        //Lista de plantillas disponibles
        public List<MG_Template> ListTemplate { get; set;}
        //Variable utilizada para asignar el id de la plantilla 
        public int Plant_Id { get; set; }
        //Variable utilizada para asignar el nombre de la plantilla
        public string Plant_ImagePath { get; set; }
    }
    public class AdminGameQuestionViewModel : BaseViewModel
    {
        //Variable utilizada para asignar el id de la plantilla
        public int Sett_Id { get; set; }
        //Variable utilizada para asignar la instancia de la configuración del juego
        public MG_SettingMp Setting { get; set; }
        //Variable utilizada para asignar el total de preguntas del juego
        public int TotalQuestion { get; set; }
        //Variable utilizada para asignar el id de la pregunta
        public int MuCh_ID { get; set; }
        //Variable utilizada para asignar la descripción de la pregunta
        [Display(Name = "Descripción")]
        public string MuCh_Description { get; set; }
        //Variable utilizada para asignar la pregunta
        [Display(Name = "Pregunta")]
        public string MuCh_NameQuestion { get; set; }
        //Variable utilizada para asignar la retroalimentación de la pregunta
        [Display(Name = "Retroalimentación")]
        public string MuCh_Feedback { get; set; }
        //Variable utilizada para asignar el nivel de la pregunta
        [Display(Name = "Nivel")]
        public LEVEL MuCh_Level { get; set; }
        //Variable utilizada para asignar el id de la respuesta
        public int AnMu_Id { get; set; }
        //Variable utilizada para asignar la descripción de la respuesta
        [Display(Name = "Respuesta")]
        public string AnMul_Description { get; set; }
        //Variable utilizada para asignar el estado de la respuesta
        [Display(Name = "Estado")]
        public OPTIONANSWER AnMul_TrueAnswer { get; set; }
    }
    public class Mg_MultipleChoise : BaseViewModel
    {
        //Variable utilizada para asignar el id de la pregunta
        public int OpMu_Id { get; set; }
        //Variable utilizada para asignar la pregunta
        [Display(Name = "Pregunta")]
        public string OpMu_Question { get; set; }
        //Variable utilizada para asignar el id de la respuesta
        [Display(Name = "Puntaje")]
        public int AnOp_Id { get; set; }
        //Variable utilizada para asignar la descripción de la respuesta
        [Display(Name = "Descripción Pregunta")]
        public string OpMu_Description { get; set; }
        //Variable utilizada para asignar el id de la configuración
        public int Sett_Id { get; set; }
        //Lista de respuestas
        public List<MG_AnswerMultipleChoice> listanswer { get; set; }


    }
    public class AdmingameMassiveQuestion:BaseViewModel
    {
        //Variable utilizada para asignar la instancia de una configuración
        public MG_SettingMp Setting { get; set; }
    } 
}