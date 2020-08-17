using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCSProject.Models
{
    public class Admin
    {
        public int AdmId { get; set; }

        public string FirstName { get; set; }

        public string Lastname { get; set; }
        public string Password { get; set; }

        public string LoginErrorMessage { get; set; }
    }
}