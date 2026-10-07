// cr-dotnet-1041: CspHttpModule – Custom HttpModule that enforces Content Security Policy
// headers at the application level for ASP.NET Web Forms running on AWS (EC2 / Elastic Beanstalk).
//
// This module works in tandem with:
//   1. The <customHeaders> block in Web.config (static baseline CSP)
//   2. AWS WAF managed rules (e.g., AWSManagedRulesCommonRuleSet) that enforce additional
//      cloud-layer security policies for SOC2 and penetration-test compliance.
//
// The module removes any pre-existing CSP header set by IIS/web.config and re-applies it
// programmatically so that per-request nonces or dynamic overrides can be injected in the
// future without touching the static configuration.

using System;
using System.Configuration;
using System.Web;

namespace Tour_Management
{
    /// <summary>
    /// HTTP Module that adds Content-Security-Policy and related security headers to every
    /// HTTP response.  Registered in both &lt;system.webServer&gt;/&lt;modules&gt; (Integrated
    /// Pipeline) and &lt;system.web&gt;/&lt;httpModules&gt; (Classic Pipeline) in Web.config.
    /// </summary>
    public class CspHttpModule : IHttpModule
    {
        // ---------------------------------------------------------------------------
        // Default CSP directive – can be overridden via AWS Systems Manager Parameter
        // Store / environment variables injected at deployment time.
        // ---------------------------------------------------------------------------
        private const string DefaultCspPolicy =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' data:; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "form-action 'self'; " +
            "base-uri 'self';";

        /// <summary>
        /// Initialises the module and registers the event handler.
        /// </summary>
        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        /// <summary>
        /// Fires just before IIS sends the response headers to the client.
        /// Removes any duplicate CSP header that may have been set by the static
        /// &lt;customHeaders&gt; configuration and re-applies the definitive value.
        /// </summary>
        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null)
                return;

            HttpResponse response = application.Response;
            if (response == null)
                return;

            // Check whether CSP enforcement is enabled (default: true).
            bool cspEnabled = true;
            string cspEnabledSetting = ConfigurationManager.AppSettings["CSP:Enabled"];
            if (!string.IsNullOrEmpty(cspEnabledSetting))
            {
                bool.TryParse(cspEnabledSetting, out cspEnabled);
            }

            if (!cspEnabled)
                return;

            // Build the CSP policy string.
            string cspPolicy = BuildCspPolicy();

            // Remove any existing CSP header (set by <customHeaders> or earlier code)
            // to avoid duplicate / conflicting values.
            response.Headers.Remove("Content-Security-Policy");
            response.Headers.Remove("X-Content-Type-Options");
            response.Headers.Remove("X-Frame-Options");
            response.Headers.Remove("X-XSS-Protection");
            response.Headers.Remove("Referrer-Policy");
            response.Headers.Remove("Permissions-Policy");

            // Apply the definitive security headers.
            response.Headers.Set("Content-Security-Policy", cspPolicy);
            response.Headers.Set("X-Content-Type-Options", "nosniff");
            response.Headers.Set("X-Frame-Options", "DENY");
            response.Headers.Set("X-XSS-Protection", "1; mode=block");
            response.Headers.Set("Referrer-Policy", "strict-origin-when-cross-origin");
            response.Headers.Set("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

            // Optionally append report-uri directive when configured (e.g., via AWS Parameter Store).
            string reportUri = ConfigurationManager.AppSettings["CSP:ReportUri"];
            if (!string.IsNullOrWhiteSpace(reportUri))
            {
                string existingCsp = response.Headers["Content-Security-Policy"];
                response.Headers.Set(
                    "Content-Security-Policy",
                    existingCsp.TrimEnd(';', ' ') + "; report-uri " + reportUri + ";");
            }
        }

        /// <summary>
        /// Builds the Content-Security-Policy directive string.
        /// The value can be overridden at runtime via the CSP_POLICY environment variable
        /// (injected by AWS Elastic Beanstalk environment properties or ECS task definitions).
        /// </summary>
        private static string BuildCspPolicy()
        {
            // Allow runtime override via environment variable (AWS EB / ECS / Lambda).
            string envPolicy = Environment.GetEnvironmentVariable("CSP_POLICY");
            if (!string.IsNullOrWhiteSpace(envPolicy))
                return envPolicy;

            return DefaultCspPolicy;
        }

        /// <summary>
        /// Disposes the module.  No unmanaged resources to release.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}
