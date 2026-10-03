using System;
using System.Web;

namespace Tour_Management
{
    /// <summary>
    /// Health check endpoint for containerization liveness/readiness probes.
    /// Accessible at GET /Health.ashx
    /// Returns HTTP 200 with JSON body {"status":"healthy"} when the application is running.
    /// </summary>
    public class Health : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            context.Response.Write("{\"status\":\"healthy\",\"application\":\"Tour_Management\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
