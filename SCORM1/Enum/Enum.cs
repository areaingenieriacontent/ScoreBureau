using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCORM1.Enum
{
    public enum CMD
    {
        N,
        CMB,
        EST
    }
    public enum RESPUESTA
    {
        Correcta,
        Incorrecta
    }
    public enum LEVEL
    {
        Fácil,
        Medio,
        Difícil
    }
    public enum GAME
    {
        No,
        Si
    }
    public enum CURSO
    {
        Virtual,
        Evaluativo

    }

    public enum SESION
    {
        No,
        Si
    }

    public enum ATTEMPTS
    {
        No,
        Si
    }

    public enum Terms_and_Conditions

    {
        No_apceptado,
        Aceptado

    }
    public enum VIDEOS

    {
        No_apceptado,
        Opción1,
        Opción2

    }

    public enum TYPEQUESTIONS
    {
        OptionMultiple,
        Pairing,
        TrueorFalse
    }
    public enum VIGENCIA
    {     
        Dias,
        Meses,
        Años
    }

    public enum TYPEPOINTS
    {
        EXTRA,
        LMS,
        PERIODICO
    }

    public enum FORO
    {
        Si,
        No
    }

    public enum TYPE
    {
        Conocimiento,
        Evaluativo
    }
    public enum ROLES
    {
        Usuario,
        AdministradordeInformacion,
        AdministradordeFormacion,
        AdministradoGeneral,
        AdministradordePuntos,
        SuperAdministradoGeneral,
        UsuarioJefe,
    }
    

    public enum TYPESUPPORTDOCUMENT
    {
        Imagen,
        Video,
        Documento
    }
    public enum ICONPRIZE
    {
        Deporte,
        Tecnologia,
        Musica,
        Juegos_Mesa,
        Arte,
        Cine,
        Eventos,
        Viajes


    }
    public enum STATEEXCHANGE
    {
        Pendiente,
        Autorizado
      
    }
    public enum ADMIN_CREATE_ROLES
    {
        Usuario,    
        AdministradordeInformacion,
        AdministradordeFormacion,
        AdministradoGeneral,
        AdministradordePuntos   
    }
    public enum ASINGUSER
    {
        Superiores,
        Clientes,
        iguales,
        miCargo
    }
    public enum EVALUATE_TO
    {
        Personal,
        Iguales,
        Superiores,
        ACargo,
        Clientes
    }

    public enum STATEUSER
    {
        Usuario,
        AdministradordeMedicion,
        AdministradordeInformacion,
        AdministradordeFormacion,
        AdministradoGeneral
    }

    public enum COUNTRY
    {
        Colombia,
        Antigua_y_Barbuda,
        Argentina,
        Bahamas,
        Barbados,
        Belice,
        Bolivia,
        Brasil,
        Canadá,
        Costa_Rica,
        Cuba,
        Chile,
        Dominica,
        Ecuador,
        El_Salvador,
        España,
        Estados_Unidos,
        Francia,
        Granada,
        Guatemala,
        Guyana,
        Haiti,
        Honduras,
        Jamaica,
        México,
        Nicaragua,
        Panamá,
        Paraguay,
        Perú,
        República_dominicana,
        San_Cristóbal_y_Nieves,
        San_Vicente_y_las_Granadinas,
        Santa_Lucía,
        Surinam,
        Trinidad_y_Tobago,
        Uruguay,
        Venezuela
    }
    
    public enum PRIZESTATE
    {
        Inactivo,
        Activo

    }
    public enum STATESCORE
    {
        No_Calificado,
        Calificado,
        Aceptado,
        No_Aceptado
    }
    public enum MODULESTATE
    {
        Inactivo,
        Activo

    }
    public enum TYPEJOB
    {
        Tarea,
        Foro
    }

    public enum IMPROVEMENTSTATE
    {
        En_Espera,
        Autorizado,
        Rechazado

    }
    public enum BETTERPRACTICESTATE
    {
        En_Espera,
        Autorizado,
        Rechazado

    }

    public enum ENROLLMENTSTATE
    {
        Inactivo,
        Activo

    }
    public enum ROLEENROLLMENT
    {
        Estudiante,
        Docente

    }
    public enum LOGSTATE
    {
        Realizado,
        NoRealizado,
        Error

    }
    public enum ARTICLESTATE
    {
        Inactivo,
        Activo

    }

    public enum EDITIONSTATE
    {
        Inactivo,
        Activo

    }

    public enum COMMENTSTATE
    {
        Inactivo,
        Activo,
        rechazado,
        aceptado
    }

    public enum ARTICLEWITHCOMMENT
    {
        Aceptar_Comentario,
        Sin_Comentarios
    }

    public enum COMPANY_TYPE
    {
        CincuentaPersonas,
        CienPersonas,
        DoscientasPersonas
    }

    public enum COMPANY_SECTOR
    {
        Tecnologico,
        Diseño,
        Servicios,
        Ganadero,
        Pesquero,
        Minero,
        Forestal,
        Industrial,
        Energetico,
        Construccion,
        Transportes,
        Comunicaciones,
        Comercial,
        Turistico,
        Sanitario,
        Educativo,
        Financiero,
        Administracion
    }

    public enum PLAN_TO
    {
        Empleados,
        Companias
    }

    public enum QUESTION_TYPE
    {
        Two = 2,
        Three = 3,
        Ten = 10,
        Eleven = 11,
        Thirteen = 13,
        Positiva = 14,
        Negativa = 15
    }

    public enum OPTIONANSWER
    {
        Falso,
        Verdadero


    }
    public enum REQUIREDEVALUATION
    {
        Si,
        No

    }

    public enum ACTIVEUSERTOENTER
    {
        Activado,
        Inactivo
    }

}