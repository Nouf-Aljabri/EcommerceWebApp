using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace EcommerceWebApp.Controllers
{
    public class HomeController : Controller
    {
       

        private readonly ILogger<HomeController> _logger;
        CustomerDBAccessLayer custdb = new CustomerDBAccessLayer();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
      
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetQuote(CustomerModel customer)
        {
            // Get the premium from PQM API 
            var url = "https://salama.com.sa/api/v2/assignments/pqm";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";

            var data = new
            {
                city = customer.City,
                dob = customer.Dob,
                gender = customer.Gender,
                marital_status = customer.Marital_status,
                vehicle_make = customer.Vehicle_make
            };

            // Store data 
            TempData["name"] = customer.Name;
            TempData["nationalID"] = customer.NationalID;
            TempData["city"] = customer.City;
            TempData["dob"] = customer.Dob;
            TempData["gender"] = customer.Gender;
            TempData["marital_status"] = customer.Marital_status;
            TempData["vehicle_make"] = customer.Vehicle_make;
         
            string json_data = JsonConvert.SerializeObject(data);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(json_data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            // Get the stream associated with the response.
            Stream receiveStream = httpResponse.GetResponseStream();
            // Pipes the stream to a higher level stream reader with the required encoding format.
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            
            var res = JsonConvert.DeserializeObject<dynamic>(readStream.ReadToEnd());

            // send data to view 
            ViewData["tax"]= res.tax;
            ViewData["premium"] = res.premium;

            TempData["insurance_price"] = (float.Parse(res.tax.ToString()) + float.Parse(res.premium.ToString()))+" ";
            return View("Checkout");
        }

        public IActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Buy(PaymentModel Payment)
        {
            // post card information to the SPG is a payment gateway 
            var url = "https://salama.com.sa/api/v2/assignments/spg";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";

            // generate quotation id 
            var quotation_id = System.Guid.NewGuid().ToString().Substring(0, 12);
            TempData["quotation_id"] = quotation_id;

            var data = new
            {
              card_number=Payment.CardNumber,
              cvv = Payment.Cvv,
              expiry_date = Payment.ExpiryDate,
              cardholder_name = Payment.Cardholder_Name,
              quotation_id= quotation_id
            };
            string json_data = JsonConvert.SerializeObject(data);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(json_data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            Stream receiveStream = httpResponse.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var res = JsonConvert.DeserializeObject<dynamic>(readStream.ReadToEnd());
            
            // if the payment status = true 
            if (res.status == true)
            {
                var check = true;
                var policy_number="";
                do {
                // post the data to SALAMA Core System to get policy number 
                 var url2 = "https://salama.com.sa/api/v2/assignments/issue-policy";

                var httpRequest2 = (HttpWebRequest)WebRequest.Create(url2);
                httpRequest2.Method = "POST";
                httpRequest2.Accept = "application/json";
                httpRequest2.ContentType = "application/json";


                var data2 = new
                {
                    policy_holder_name = TempData["name"] as string,
                    policy_holder_id = TempData["nationalID"] as string,
                    quotation_id = TempData["quotation_id"] as string,
                    city = TempData["city"] as string,
                    dob = TempData["dob"] as string,
                    gender = TempData["gender"] as string,
                    marital_status = TempData["marital_status"] as string,
                    vehicle_make = TempData["vehicle_make"] as string

                };
                string json_data2 = JsonConvert.SerializeObject(data2);
                using (var streamWriter = new StreamWriter(httpRequest2.GetRequestStream()))
                {
                    streamWriter.Write(json_data2);
                }

                var httpResponse2 = (HttpWebResponse)httpRequest2.GetResponse();
                Stream receiveStream2 = httpResponse2.GetResponseStream();
                StreamReader readStream2 = new StreamReader(receiveStream2, Encoding.UTF8);
                var res2 = JsonConvert.DeserializeObject<dynamic>(readStream2.ReadToEnd());
                TempData["policy_number"] = res2.policy_number;
                 policy_number = res2.policy_number;
      
                if (res2.status == true){
                    check = false;
                }
                } while (check);

                // add to database 
                String s = custdb.AddCustomer(new CustomerDB
                {
                    customer_Name = TempData["name"] as string,
                    national_ID = TempData["nationalID"] as string,
                    vehicle = TempData["vehicle_make"] as string,
                    insurance_price = TempData["insurance_price"] as string,
                    quotation_id = TempData["quotation_id"] as string,
                    policy_number = policy_number,

                });
                Console.WriteLine(s);

                return View("policy");
            }


            // if the payment status = false print the error 
            else {
                ViewData["message"] = res.ToString();
                return View("Payment");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
