using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.Models
{
    public class PaymentModel
    {
        public int CardNumber { get; set; }

        public int Cvv { get; set; }

        public int ExpiryDate { get; set; }

        public string Cardholder_Name { get; set; }
    }
}
