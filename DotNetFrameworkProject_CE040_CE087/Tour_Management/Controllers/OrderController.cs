using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Dapper;
using Tour_Management.Models;

namespace Tour_Management.Controllers
{
    /// <summary>
    /// ASP.NET Core MVC controller replacing Order.aspx Web Form.
    /// Migrated from Web Forms (System.Web.UI.Page) to ASP.NET MVC Controller pattern.
    /// </summary>
    public class OrderController : Controller
    {
        // Retrieve connection string from environment variable for RDS Proxy compatibility.
        // Falls back to Web.config connectionString when the environment variable is not set.
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
        }

        // -----------------------------------------------------------------------
        // GET: /Order/Order
        // Replaces: Order.aspx initial render (Page_Load)
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Order()
        {
            return View(new OrderViewModel());
        }

        // -----------------------------------------------------------------------
        // POST: /Order/Order
        // Replaces: Order.aspx.cs btn_click event handler
        // Original logic: INSERT into booking table, then Response.Redirect("mybooking.aspx")
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Order(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string insertQuery = "INSERT INTO booking(TOUR_NAME, PLACE, Email, FirstName) " +
                                 "VALUES(@TOUR_NAME, @PLACE, @Email, @FirstName)";

            var parameters = new
            {
                TOUR_NAME = model.TourName,
                PLACE     = model.City,
                Email     = model.MobileNumber,
                FirstName = model.Name
            };

            // Use Dapper with a managed SqlConnection targeting Amazon RDS Proxy.
            // The 'using' block ensures the connection is returned to the pool after use.
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Execute(insertQuery, parameters);
            }

            TempData["SuccessMessage"] = "Registration Successful";
            // Replaces: Response.Redirect("mybooking.aspx")
            return RedirectToAction("MyBooking", "Booking");
        }
    }
}
