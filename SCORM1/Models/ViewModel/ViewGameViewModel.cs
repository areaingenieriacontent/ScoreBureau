using Chart.Mvc.ComplexChart;
using SCORM1.Enum;
using SCORM1.Models.MainGame;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ViewModel
{
    public class ViewGameViewModel
    {
    }
    public class AdminPreviw:BaseViewModel
    {
        //Variable urilizada para asignar una instancia de configuración del juego
        public MG_SettingMp setting { get; set;}
        //Variable urilizada para asignar el id de la configuración del juego
        public int Sett_Id { get; set; }
        //Variable urilizada para asignar la cantidad de preguntas faciles
        public int facil { get; set; }
        //Variable urilizada para asignar la cantidad de preguntas medias
        public int medio { get; set; }
        //Variable urilizada para asignar la cantidad de preguntas dificiles
        public int dificil { get; set; }
        //Variable urilizada para asignar el contador de preguntas
        public int contador { get; set; }
        //Lista de preguntas de nivel facil
        public List<MultipleChoiceFacil>listquestionsFacil { get; set;}
        //Lista de preguntas de nivel medio
        public List<MultipleChoiceMedio> listquestionsMedio { get; set; }
        //Lista de preguntas de nivel dificil
        public List<MultipleChoiceDificil> listquestionsDificil { get; set; }
        //Lista de ranking de usuarios
        public List<listranking> ListRanking { get; set; }

    }
    public class UserGame : BaseViewModel
    {
        //Variable utilizada para asignar los terminos y condiciones del usuario
        [Display(Name = "Acepto Los terminos del juego.")]
        public bool termsandGame { get; set; }

    }
    public class UserPreviw : BaseViewModel
    {
        //Variable utilizada para asignar los intentos del usuario
        public int attemptUser { get; set; }
        //Variable utilizada para asignar los intentos del juego
        public int attempts { get; set; }
        //Variable utilizada para asignar el id del usuario
        public string User_Id { get; set; }
        //Variable utilizada para asignar una instancia de un usuario
        public ApplicationUser User { get; set; }
        //Variable utilizada para asignar una instancia de una configuración de juego
        public MG_SettingMp setting { get; set; }
        //Variable utilizada para asignar el id de la configuración del juego
        public int Sett_Id { get; set; }
        //Variable utilizada para asignar la cantidad de preguntas faciles
        public int facil { get; set; }
        //Variable utilizada para asignar la cantidad de preguntas medias
        public int medio { get; set; }
        //Variable utilizada para asignar la cantidad de preguntas dificiles
        public int dificil { get; set; }
        //Variable utilizada para asignar el contador de preguntas del usuario
        public int contador { get; set; }
        //Lista de preguntas de nivel facil
        public List<MultipleChoiceFacil> listquestionsFacil { get; set; }
        //Lista de preguntas de nivel medio
        public List<MultipleChoiceMedio> listquestionsMedio { get; set; }
        //Lista de preguntas de nivel dificil
        public List<MultipleChoiceDificil> listquestionsDificil { get; set; }
        //Lista de ranking de usuarios
        public List<listranking>ListRanking { get; set;}
        //Lista de ranking personal del usuario
        public List<listranking> ListRankingUser { get; set; }
    }
    public class MultipleChoiceFacil
    {
        //Variable utilizada para asignar el nivel de una pregunta
        public LEVEL MuCh_Level { get; set; }
        //Variable utilizada para asignar el id de una configuración de juego
        public int Sett_Id { get; set; }
        //Variable utilizada para asignar el id de una pregunta
        public int MuCh_ID { get; set; }
        //Variable utilizada para asignar la descripción de una pregunta
        public string MuCh_Description { get; set; }
        //Variable utilizada para asignar la pregunta
        public string MuCh_NameQuestion { get; set; }
        //Variable utilizada para asignar la imagen de una pregunta
        public string MuCh_ImageQuestion { get; set; }
        //Variable utilizada para asignar la retroalimentación de una pregunta
        public string MuCh_Feedback { get; set; }
        //Variable utilizada para asignar el id de la respuesta
        public int AnMul_ID { get; set; }
        //Lista de respuestas
        public List<MG_AnswerMultipleChoice> listanswerM { get; set; }
    }
    public class MultipleChoiceMedio
    {
        //Variable utilizada para asignar el nivel de una pregunta
        public LEVEL MuCh_Level { get; set; }
        //Variable utilizada para asignar el id de una configuración de juego
        public int Sett_Id { get; set; }
        //Variable utilizada para asignar el id de una pregunta
        public int MuCh_ID { get; set; }
        //Variable utilizada para asignar la descripción de una pregunta
        public string MuCh_Description { get; set; }
        //Variable utilizada para asignar la pregunta
        public string MuCh_NameQuestion { get; set; }
        //Variable utilizada para asignar la imagen de una pregunta
        public string MuCh_ImageQuestion { get; set; }
        //Variable utilizada para asignar la retroalimentación de una pregunta
        public string MuCh_Feedback { get; set; }
        //Variable utilizada para asignar el id de la respuesta
        public int AnMul_ID { get; set; }
        //Lista de respuestas
        public List<MG_AnswerMultipleChoice> listanswerM { get; set; }
    }
    public class MultipleChoiceDificil
    {
        //Variable utilizada para asignar el nivel de una pregunta
        public LEVEL MuCh_Level { get; set; }
        //Variable utilizada para asignar el id de una configuración de juego
        public int Sett_Id { get; set; }
        //Variable utilizada para asignar el id de una pregunta
        public int MuCh_ID { get; set; }
        //Variable utilizada para asignar la descripción de una pregunta
        public string MuCh_Description { get; set; }
        //Variable utilizada para asignar la pregunta
        public string MuCh_NameQuestion { get; set; }
        //Variable utilizada para asignar la imagen de una pregunta
        public string MuCh_ImageQuestion { get; set; }
        //Variable utilizada para asignar la retroalimentación de una pregunta
        public string MuCh_Feedback { get; set; }
        //Variable utilizada para asignar el id de la respuesta
        public int AnMul_ID { get; set; }
        //Lista de respuestas
        public List<MG_AnswerMultipleChoice> listanswerM { get; set; }
    }
    public class QuestionSelect : BaseViewModel
    {
        //Variable utilizada para asignar la fecha de ingreso del usuario
        public string fechaingreso { get; set; }
        //Variable utilizada para asignar los segundos de la pregunta
        public int seg { get; set; }
        //Variable utilizada para asignar la instancia de la configuración del juego
        public MG_SettingMp setting { get; set; }
        //Variable utilizada para asignar el id de la configuración del juego
        public int Sett_Id { get; set; }
        //Variable utilizada para asignar la cantidad de preguntas de nivel facil
        public int facil { get; set; }
        //Variable utilizada para asignar la cantidad de preguntas de nivel medio
        public int medio { get; set; }
        //Variable utilizada para asignar la cantidad de preguntas de nivel dificil
        public int dificil { get; set; }
        //Variable utilizada para asignar el contador de preguntas contestada por el usuario
        public int contador { get; set; }
        //Lista de pregunta seleccionada
        public List<MultipleChoiceselect> listquestionsselect { get; set; }
        //Variable utilizada para asignar el primer comodin
        public int cmd1 { get; set; }
        //Variable utilizada para asignar el segundo comodin
        public int cmd2 { get; set; }
        //Variable utilizada para asignar la cantidad de comodines utilizados
        public int v1 { get; set; }
        //Variable utilizada para asignar la cantidad de comodines utilizados 
        public int v2 { get; set; }
        //Variable utilizada para asignar el diagrama de barras
        public BarChart Result { get; set; }

    }
    public class MultipleChoiceselect
    {
        //Variable utilizada para asignar el nivel de una pregunta
        public LEVEL MuCh_Level { get; set; }
        //Variable utilizada para asignar el id de una configuración de juego
        public int Sett_Id { get; set; }
        //Variable utilizada para asignar el id de una pregunta
        public int MuCh_ID { get; set; }
        //Variable utilizada para asignar la descripción de una pregunta
        public string MuCh_Description { get; set; }
        //Variable utilizada para asignar la pregunta
        public string MuCh_NameQuestion { get; set; }
        //Variable utilizada para asignar la imagen de una pregunta
        public string MuCh_ImageQuestion { get; set; }
        //Variable utilizada para asignar la retroalimentación de una pregunta
        public string MuCh_Feedback { get; set; }
        //Variable utilizada para asignar el id de la respuesta
        public int AnMul_ID { get; set; }
        //Lista de respuestas
        public List<MG_AnswerMultipleChoice> listanswerM { get; set; }
    }
    public class listranking
    {
        //Variable utilizada para asignar la instancia de un usuario
        public ApplicationUser user { get; set; }
        //Variable utilizada para asignar los puntos del usuario
        public int puntos { get; set; }
        //Variable utilizada para asignar la lista de ranking del usuario
        public int Ranking { get; set; }
    }
}