using SCORM1.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ViewModel
{
    public class AttemptsOfLevel
    {
        public int PointsAssigned { get; set; }
        public DateTime date { get; set; }
    }

    public class Listlevel
    {
        public int Level { get; set; }
        public List<AttemptsOfLevel> AttemptsOfLevel { get; set; }
    }
}