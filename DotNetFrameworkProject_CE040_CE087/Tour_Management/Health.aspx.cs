using System;
using System.Web;

namespace Tour_Management
{
    /// <summary>
    /// Health check endpoint for containerization readiness.
    /// Returns HTTP 200 with JSON status at GET /Health.aspx
    /// </summary>
    public partial class Health : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            Response.Write("{\"status\":\"healthy\",\"service\":\"Tour_Management\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
            Response.End();
        }
    }
}
