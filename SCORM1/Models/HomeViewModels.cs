using SCORM1.Enum;
using SCORM1.Models.Newspaper;
using SCORM1.Models.ViewModel;
using System.Collections.Generic;

namespace SCORM1.Models
{
    public class HomeViewModels : ViewModel.HomeViewModels
    {
     
    }

    public class AdminInformationHomeViewModel : BaseViewModel
    {
        public Edition EditionCurrentToActive { get; set; }
        public List<Article> ListArticles { get; set; }
    }

}