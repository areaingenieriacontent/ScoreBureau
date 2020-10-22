using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCORM1.Models
{
    public class ValidationUser
    {
        public static string CodeHashOfPassword(string password, string passwordHas)
        {
            IPasswordHasher test = new PasswordHasher();
            string Answer= test.VerifyHashedPassword(passwordHas, password).ToString();
            return Answer;
        }

    }
}