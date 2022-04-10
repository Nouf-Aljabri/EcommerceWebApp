using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using EcommerceWebApp.Models;

namespace EcommerceWebApp.Models
{
    public class CustomerDBAccessLayer
    {
        SqlConnection con = new("Server=DESKTOP-EL3J9QR;Database=MyDB;Trusted_Connection=True;");
        public string AddCustomer(CustomerDB customer) {
           
            
                SqlCommand cmd = new SqlCommand("Customer_Add", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customer_Name", customer.customer_Name);
                cmd.Parameters.AddWithValue("@national_ID", customer.national_ID);
                cmd.Parameters.AddWithValue("@vehicle", customer.vehicle);
                cmd.Parameters.AddWithValue("@insurance_price", customer.insurance_price);
                cmd.Parameters.AddWithValue("@quotation_id", customer.quotation_id);
                cmd.Parameters.AddWithValue("@policy_number", customer.policy_number);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return ("Data Added Successfully");
         
           
        } 
    }
}
