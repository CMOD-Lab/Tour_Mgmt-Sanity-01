using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class Order : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          
        }

        protected void btn_click(object sender, EventArgs e)
        {
               // cz-dotnet-0055: Replaced Web.config XDT transform-based connection string with environment variable
               SqlConnection conn = new SqlConnection(System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
                conn.Open();
                string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) values(@TOUR_NAME,@PLACE,@Email,@FirstName)";
                SqlCommand com = new SqlCommand(insertQuery, conn);
                com.Parameters.AddWithValue("@TOUR_NAME", tour_name.Text);
                
                com.Parameters.AddWithValue("@PLACE", city.Text);
               
                com.Parameters.AddWithValue("@Email",number.Text);
                com.Parameters.AddWithValue("@FirstName", name.Text);

                com.ExecuteNonQuery();
                Response.Write("Registration Successful");
                Response.Redirect("mybooking.aspx");
                Server.Transfer("mybooking.aspx");
                conn.Close();



            
           
        }
    }
}