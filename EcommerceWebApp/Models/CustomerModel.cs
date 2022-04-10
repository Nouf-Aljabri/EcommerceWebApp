using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class CustomerModel
    {
        public string NationalID { get; set; }

        public string Name { get; set; }
    
        public string City { get; set; }
     
        public string Dob { get; set; }
       
        public string Gender { get; set; }
     
        public string Marital_status { get; set; }
 
        public string Vehicle_make { get; set; }

    }
}
