using System;
using System.Web.UI;

namespace Tour_Management
{
    /// <summary>
    /// allbooking.aspx code-behind - MIGRATED TO ASP.NET MVC
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    ///
    /// This Web Form has been replaced by:
    ///   - Controller: BookingController.AllBooking (GET) in Controllers/BookingController.cs
    ///   - View: Views/Booking/AllBooking.cshtml
    ///   - Model: Models/BookingViewModel (in Models/TourModels.cs)
    ///
    /// The original Web Forms patterns removed:
    ///   - System.Web.UI.Page inheritance (line 10) → replaced by Controller : Controller
    ///   - System.Web.UI / System.Web.UI.WebControls using statements (lines 5-6) → removed
    ///   - asp:GridView with asp:SqlDataSource controls (lines 10, 12) → replaced by Razor table
    ///   - Page_Load event handler → replaced by [HttpGet] ActionResult
    ///   - SqlDataSource SelectCommand → replaced by Dapper query in controller action
    /// </summary>
    public partial class allbooking : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect to the ASP.NET MVC equivalent route
            Response.RedirectPermanent("~/Booking/AllBooking");
        }
    }
}
