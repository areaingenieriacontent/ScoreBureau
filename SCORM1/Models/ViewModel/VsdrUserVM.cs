using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCORM1.Models.VSDR;

namespace SCORM1.Models.ViewModel
{
    public class VsdrUserVM : BaseViewModel
    {
        public VsdrSession actualVsdr;
        public List<VsdrSession> listOfVsdr;
        public List<VsdrUserFile> listOfIssuedFiles;
        public List<VsdrTeacherComment> listOfComments;
        public string teacherName;
        public string teacherLastName;
        public bool meetingAvailable;

        /*file upload variables*/
        public VsdrUserFile vsdrFileToAdd;
    }
}