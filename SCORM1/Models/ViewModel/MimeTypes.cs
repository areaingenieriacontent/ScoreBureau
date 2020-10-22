using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCORM1.Models.ViewModel
{
    public class MimeTypes
    {
        public static Dictionary<string, string> ImageMimeTypes = new Dictionary<string, string>
        {
            { ".gif", "image/gif" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".png", "image/png" },
        };
    }
}