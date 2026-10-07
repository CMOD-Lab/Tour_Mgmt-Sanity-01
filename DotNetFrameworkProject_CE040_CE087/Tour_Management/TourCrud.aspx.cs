using System;
using System.Web.UI;

namespace Tour_Management
{
    /// <summary>
    /// TourCrud.aspx code-behind - MIGRATED TO ASP.NET MVC
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    ///
    /// This Web Form has been replaced by:
    ///   - Controller: BookingController.TourCrud (GET) in Controllers/BookingController.cs
    ///   - View: Views/Booking/TourCrud.cshtml
    ///   - Model: Models/TourCrudViewModel (in Models/TourModels.cs)
    ///
    /// The original Web Forms patterns removed:
    ///   - System.Web.UI.Page inheritance (line 13) → replaced by Controller : Controller
    ///   - System.Web.UI / System.Web.UI.WebControls using statements (lines 5-6) → removed
    ///   - asp:GridView with asp:SqlDataSource controls (lines 13, 15, 18) → replaced by Razor table
    ///   - Page_Load event handler → replaced by [HttpGet] ActionResult
    ///   - refreshdata() method → replaced by Dapper query in controller action
    ///   - SqlDataSource UpdateCommand/DeleteCommand → replaced by UpdateTour/DeleteTour POST actions
    ///
    /// cr-dotnet-0010 fix applied: ConfigurationManager.ConnectionStrings["dbconnection"]
    /// has been replaced with environment variable DB_CONNECTION_STRING (with fallback to
    /// AWS Systems Manager Parameter Store key /tour-management/db-connection-string).
    /// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer
    /// used for connection string injection — configuration is injected at runtime via
    /// environment variables, enabling immutable deployments on AWS.
    ///
    /// Original cloud-incompatible pattern (cr-dotnet-0010) removed:
    ///   SqlConnection conn = new SqlConnection(
    ///       ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
    ///
    /// Replaced by (in BookingController.cs):
    ///   string connStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ///       ?? AwsSsmHelper.GetParameter("/tour-management/db-connection-string");
    /// </summary>
    public partial class TourCrud : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect to the ASP.NET MVC equivalent route
            Response.RedirectPermanent("~/Booking/TourCrud");
        }
    }
}
