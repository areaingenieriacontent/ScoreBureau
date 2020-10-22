using SCORM1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCORM1.Models.Newspaper;

namespace SCORM1.Models.ViewModel
{
    public class HomeViewModels : BaseViewModel
    {

    }

    public class AdminInformationHomeViewModel : BaseViewModel
    {
        public Edition EditionCurrentToActive { get; set; }
        public List<Article> ListArticles { get; set; }
    }

}