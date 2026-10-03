using System;
using System.Web;

namespace Tour_Management
{
    /// <summary>
    /// ContentSecurityPolicyModule — custom IHttpModule that injects Content-Security-Policy
    /// and related security headers on every HTTP response.
    ///
    /// Remediation for cr-dotnet-1041 (Missing Content Security Policy Headers):
    ///   Implements CSP headers in ASP.NET Web Forms using a custom HttpModule registered in
    ///   web.config, combined with AWS WAF managed rules to enforce cloud security compliance
    ///   for SOC2 and penetration-test requirements.
    ///
    /// AWS WAF integration note:
    ///   Deploy the AWS WAF "AWSManagedRulesCommonRuleSet" and
    ///   "AWSManagedRulesKnownBadInputsRuleSet" managed rule groups on the Application Load
    ///   Balancer / CloudFront distribution that fronts this application.  The WAF rules
    ///   provide a second layer of XSS/injection protection in multi-tenant cloud environments.
    ///
    /// CSP policy note:
    ///   The default policy below is intentionally strict.  Adjust the CSP_POLICY constant
    ///   (or override via the CSP_POLICY environment variable) to match the actual origins,
    ///   scripts, and styles used by the application before deploying to production.
    /// </summary>
    public class ContentSecurityPolicyModule : IHttpModule
    {
        // ---------------------------------------------------------------------------
        // Default CSP policy — can be overridden at runtime via the CSP_POLICY
        // environment variable so that different AWS environments (dev / staging /
        // prod) can carry different policies without a code change.
        // ---------------------------------------------------------------------------
        private static readonly string DefaultCspPolicy =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "form-action 'self'; " +
            "base-uri 'self';";

        // ---------------------------------------------------------------------------
        // IHttpModule implementation
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Initialises the module and subscribes to the PreSendRequestHeaders event so
        /// that security headers are added to every response (including error responses).
        /// </summary>
        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        /// <summary>
        /// Disposes the module.  No managed resources to release.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose.
        }

        // ---------------------------------------------------------------------------
        // Event handler
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Adds Content-Security-Policy and complementary security headers to the
        /// outgoing HTTP response before the headers are flushed to the client.
        /// </summary>
        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null)
                return;

            HttpResponse response = application.Response;
            if (response == null)
                return;

            // ------------------------------------------------------------------
            // 1. Content-Security-Policy
            //    Resolve policy from environment variable first; fall back to the
            //    compiled-in default so the application works out-of-the-box.
            // ------------------------------------------------------------------
            string cspPolicy = System.Environment.GetEnvironmentVariable("CSP_POLICY");
            if (string.IsNullOrWhiteSpace(cspPolicy))
                cspPolicy = DefaultCspPolicy;

            SetHeader(response, "Content-Security-Policy", cspPolicy);

            // ------------------------------------------------------------------
            // 2. X-Content-Type-Options
            //    Prevents MIME-type sniffing — required by SOC2 / pen-test checks.
            // ------------------------------------------------------------------
            SetHeader(response, "X-Content-Type-Options", "nosniff");

            // ------------------------------------------------------------------
            // 3. X-Frame-Options
            //    Prevents clickjacking.  Redundant with frame-ancestors in CSP but
            //    retained for older browser compatibility.
            // ------------------------------------------------------------------
            SetHeader(response, "X-Frame-Options", "DENY");

            // ------------------------------------------------------------------
            // 4. X-XSS-Protection
            //    Legacy XSS filter hint for older browsers.
            // ------------------------------------------------------------------
            SetHeader(response, "X-XSS-Protection", "1; mode=block");

            // ------------------------------------------------------------------
            // 5. Referrer-Policy
            //    Limits referrer information leakage across origins.
            // ------------------------------------------------------------------
            SetHeader(response, "Referrer-Policy", "strict-origin-when-cross-origin");

            // ------------------------------------------------------------------
            // 6. Permissions-Policy
            //    Restricts access to browser features not required by the app.
            // ------------------------------------------------------------------
            SetHeader(response, "Permissions-Policy",
                "geolocation=(), microphone=(), camera=(), payment=()");
        }

        // ---------------------------------------------------------------------------
        // Helper
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Sets a response header, replacing any existing value to avoid duplicate
        /// headers that could be exploited by header-injection attacks.
        /// </summary>
        private static void SetHeader(HttpResponse response, string name, string value)
        {
            try
            {
                // Remove any existing header with the same name before setting the
                // new value so that the module is idempotent and safe to call multiple
                // times (e.g., when IIS also injects the same header).
                response.Headers.Remove(name);
                response.Headers.Set(name, value);
            }
            catch (PlatformNotSupportedException)
            {
                // Headers.Remove / Headers.Set can throw PlatformNotSupportedException
                // in Classic pipeline mode.  Fall back to AppendHeader which is always
                // available but may produce duplicate headers.
                response.AppendHeader(name, value);
            }
        }
    }
}
