using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;
using SCORM1.Models.SCORM1;
using SCORM1.Models.VSDR;

namespace SCORM1.Models.ViewModel
{
    public class VsdrUserVM : BaseViewModel
    {
      
        public VsdrSession actualVsdr { get; set; }
        public List<VsdrSession> listOfVsdr { get; set; }
        public List<VsdrUserFile> listOfIssuedFiles { get; set; }
        public List<VsdrTeacherComment> listOfComments { get; set; }
        public List<ApplicationUser> listOfUsers { get; set; }
        public List<VsdrEnrollment> ListEnrollment { get; set; }
        public string teacherName { get; set; }
        public string teacherLastName { get; set; }
        public string idUserdos { get; set; }
        public bool meetingAvailable { get; set; }
        public int Id_VSDR { get; set; }
        public string User { get; set; }


        /*file upload variables*/
        public VsdrUserFile vsdrFileToAdd { get; set; }
        public VsdrTeacherComment VsdrTeacherComment { get; set; }

        public VsdrEnrollment VsdrEnrollment { get; set; }
        public VsdrSession session { get; set; }
    }
    public class CreateVsdrSession : BaseViewModel
    {
        public VsdrSession actualVsdr1 { get; set; }
        public VsdrEnrollment vsdrEnrollment { get; set; }
        public List<VsdrEnrollment> listEnrollment { get; set; }
        public List<VsdrSession> listOfVsdr { get; set; }
        public List<UserAndMassiveManagementViewModel> listUser { get; set; }
        public IPagedList<ApplicationUser> UserOfCompany { get; set; }
        public List<ApplicationUser> listUser123 { get; set; }
        public int Id_VSDR { get; set; }
        public string User { get; set; }
        public VsdrSession session { get; set; }
        [Display(Name = "Areas")]
        public int Area_Id { get; set; }
        [Display(Name = "Cargos")]
        public int Posi_Id { get; set; }
        [Display(Name = "Ciudades")]
        public int Cyty_Id { get; set; }
        [Display(Name = "Ubicaciones")]
        public int Loca_Id { get; set; }
        public List<AllUserVSDR> ListAllUser { get; set; }
        public List<AreasVSDR> ListAreas { get; set; }
        public List<PositionsVSDR> ListPositions { get; set; }
        public List<CitiesVSDR> ListCitices { get; set; }
        public List<LocationsVSDR> ListLocations { get; set; }
        public List<user> ListUserCancel { get; set; }
        public List<user1> ListUserCancel1 { get; set; }
        public List<user2> ListUserCancel2 { get; set; }
    }
    public class AllUserVSDR : BaseViewModel
    {
        public int VSDR_Id { get; set; }
        public string User_Id { get; set; }
    }

    public class AreasVSDR : BaseViewModel
    {
        public int Area_Id { get; set; }
        public int VSDR_Id { get; set; }
        public List<Area> Listareas { get; set; }
    }

    public class PositionsVSDR : BaseViewModel
    {
        public int Posi_Id { get; set; }
        public int VSDR_Id { get; set; }
        public List<Position> Listpositions { get; set; }
    }

    public class CitiesVSDR : BaseViewModel
    {
        public int City_Id { get; set; }
        public int VSDR_Id { get; set; }
        public List<City> Listcities { get; set; }
    }

    public class LocationsVSDR : BaseViewModel
    {
        public int Loca_Id { get; set; }
        public int VSDR_Id { get; set; }
        public List<Location> Listlocations { get; set; }
    }

}