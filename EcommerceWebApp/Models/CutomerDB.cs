using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class CustomerDB
    {
        [Required]
        public string customer_Name { get; set; }
        [Required]
        public string national_ID { get; set; }
        [Required]
        public string vehicle { get; set; }
        [Required]
        public string insurance_price { get; set; }
        [Required]
        public string quotation_id { get; set; }
        [Required]
        public string policy_number { get; set; }





    }
}
