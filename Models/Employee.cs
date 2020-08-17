using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TCSProject.Models
{
    public class Employee
    {

        
        public int EmpId
        {
            get; set; 
        }
        
        public string FirstName { get; set; }

        
        public string LastName { get; set; }

        
        public string Password { get; set; }

        
        public string  Email { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime  DOB { get; set; }


        public int ProjectId { get; set; }
        public int WONNumber { get; set; }

        public string ProjectDetails { get; set; }
        public string LoginErrorMessage { get; set; }
    }
}