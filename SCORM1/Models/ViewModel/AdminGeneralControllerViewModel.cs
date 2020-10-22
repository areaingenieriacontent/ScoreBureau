using SCORM1.Enum;
using SCORM1.Models.Engagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCORM1.Models.SCORM1;
using SCORM1.Models.Newspaper;
using SCORM1.Models.Lms;
using PagedList;

namespace SCORM1.Models.ViewModel
{
    public class AdminGeneralControllerViewModel : BaseViewModel
    {
        [Display(Name = "Acepto Los terminos y condiciones")]
        public bool termsandconditions { get; set; }
        public ApplicationUser Admin { get; set; }
        public List<Module> Listmodulevirtual { get; set; }
        public List<Module> Listmoduleevaluative { get; set; }
        public GAME juego { get; set; }
        public Edition EditionCurrentToActive { get; set; }
        public List<Article> ListArticles { get; set; }
    }



    public class AdminGeneralCategoryPrizeViewModel : BaseViewModel
    {
        public List<CategoryPrize> ListCategoryPrize { get; set; }
        public CategoryPrize CategoryPrize { get; set; }
        public int Capr_Id { get; set; }
        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "Se Debe Contar con un Nombre de Categoria")]
        public string Capr_category { get; set; }
        public string searchCategoryPrize { get; set; }
        public List<Prize> ListPrize { get; internal set; }
        public string Priz_Name { get; internal set; }
        public string Priz_Description { get; internal set; }
        public int Priz_RequiredPoints { get; internal set; }
        public int Priz_Quantity { get; internal set; }
        public PRIZESTATE Priz_Stateprize { get; internal set; }
        public DateTime Priz_Date { get; internal set; }
        public Company Company { get; internal set; }
    }


    public class AdminGeneralPrizeViewModel : BaseViewModel
    {
        public List<Prize> ListPrize { get; set; }
        public List<CategoryPrize> ListCategoryPrize { get; set; }
        [Display(Name = "Categorias")]
        [Required(ErrorMessage = "Se Debe Contar con una Categoria")]
        public int Priz_Capr_Id { get; set; }
        public IEnumerable<SelectListItem> CategoryPrize { get; set; }
        public int Modu_CompanyId { get; set; }
        public Prize Prize { get; set; }
        public int Priz_Id { get; set; }
        [Display(Name = "Nombre Premio")]
        [Required(ErrorMessage = "Se Debe Contar con un Nombre de Categoria")]
        public string Priz_Name { get; set; }
      
        [Display(Name = "Descripción")]
        public string Priz_Description { get; set; }
        [Display(Name = "Puntos Requeridos")]
        public int Priz_RequiredPoints { get; set; }
        [Display(Name = "Cantidad")]
        public int Priz_Quantity { get; set; }
        [Display(Name = "Estado Premio")]
        public PRIZESTATE Priz_Stateprize { get; set; }
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime Priz_Date { get; set; }
        [Display(Name = "Icono")]
        public ICONPRIZE Prize_Icon { get; set; }

        public string searchPrize { get; set; }
    }

    public class AdminPointsGeneral : BaseViewModel
    {
        public int TotalPointUser { get; set; }
        public int Exch_Id { get; set; }
        [Display(Name = "Usuarios")]
        public List<ApplicationUser> Users { get; set; }
        [Display(Name = "Puntos")]
        public List<Point> points { get; set; }
        [Display(Name = "Premios")]
        public List<Prize> prizes { get; set; }
        [Display(Name = "Estado solicitud")]
        public STATEEXCHANGE StateExchange { get; set; }
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime Exch_date { get; set; }

        [Display(Name = "Fecha de Aprobación")]
        [DataType(DataType.Date)]
        public DateTime? Exch_Finishdate { get; set; }
        public List<Exchange> Exchanges { get; set; }

    }

    public class Points : BaseViewModel

    {

        public int Poin_Id { get; set; }
        [Display(Name = "Cantidad de puntos ")]
        public int Quantity_Points { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime Poin_Date { get; set; }
        public int TyPo_Id { get; set; }
        public TypePoint TypePoint { get; set; }
        public string User_Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Exchange> Exchange { get; set; }


    }

    public class AdminReportsUserIndividual : BaseViewModel
    {
        public IPagedList<ApplicationUser> UserEnrollment { get; set; }
        public string SearchUser { get; set; }
    }

    public class AdminReportsUserLogs : BaseViewModel
    {
        public IPagedList<ApplicationUser> UserLogs { get; set; }
        public string SearchUser { get; set; }
        [Display(Name = "Cantidad")]
        [Range(1, 365, ErrorMessage = "mumeros mayores que cero")]
        public int Number { get; set; }
        [Display(Name = "Tiempo")]
        public VIGENCIA Time { get; set; }
        public List<ApplicationUser> UserSelected { get; set; }
        
    }

    public class AdminReports: BaseViewModel

       {
       
        public ApplicationUser user { get; set; }
        public List<Module> Modules { get; set; }
        public List<AdvanceCourse> AdvanceCoourceList { get; set; }
        public int PointUserTotal { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<Exchange> Exchanges { get; set; }








        //        public List<PointManagerCategory> PointManagerCategory { get; set; }
        //        public PointManagerCategory PointComments { get; set; }
        //        public int PoMaCa_Id { get; set; }
        //        [Display(Name = "Puntos por Comentarios del Periodico")]

        //        public string PoMaCa_Periodical { get; set; }
        //        [Display(Name = "Puntos Comentarios del Curso")]
        //        public string PoMaCa_course { get; set; }


        //        public string searchPointManagerCategory { get; set; }
        //        public List<Prize> ListPrize { get; internal set; }
        //        public string Priz_Name { get; internal set; }
        //        public string Priz_Description { get; internal set; }
        //        public int Priz_RequiredPoints { get; internal set; }
        //        public int Priz_Quantity { get; internal set; }
        //        public PRIZESTATE Priz_Stateprize { get; internal set; }
        //        public DateTime Priz_Date { get; internal set; }
        //        public Company Company { get; internal set; }
        //    }

    }

}
