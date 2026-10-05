using System;
using System.Web;

namespace Tour_Management
{
    /// <summary>
    /// Health check HTTP handler for containerization readiness.
    /// Accessible at /health
    /// </summary>
    public class HealthCheckHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            context.Response.Write("{\"status\":\"healthy\",\"application\":\"Tour_Management\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
        }
    }
}
