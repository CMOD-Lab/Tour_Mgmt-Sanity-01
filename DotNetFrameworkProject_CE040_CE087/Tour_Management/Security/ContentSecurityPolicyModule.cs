using System;
using System.Web;

namespace Tour_Management.Security
{
    /// <summary>
    /// Custom HttpModule that injects Content-Security-Policy (CSP) headers into every
    /// HTTP response.  Registering CSP at the module level ensures the header is present
    /// on all responses (HTML pages, AJAX calls, error pages) without requiring changes
    /// to individual ASPX pages or controllers.
    ///
    /// Cloud / AWS context
    /// -------------------
    /// This module satisfies the application-side requirement for SOC2 / penetration-test
    /// compliance.  AWS WAF managed rules (e.g. AWSManagedRulesCommonRuleSet) provide the
    /// network-layer enforcement layer; together they form a defence-in-depth posture for
    /// multi-tenant cloud deployments.
    ///
    /// The CSP policy below follows a "strict-but-practical" baseline:
    ///   - default-src 'self'          : only same-origin resources by default
    ///   - script-src  'self'          : no inline scripts, no eval
    ///   - style-src   'self' 'unsafe-inline' : Bootstrap / legacy inline styles allowed
    ///   - img-src     'self' data: https: : images from same origin, data URIs, and HTTPS
    ///   - font-src    'self' https:   : web fonts over HTTPS
    ///   - connect-src 'self'          : XHR / fetch only to same origin
    ///   - frame-ancestors 'none'      : prevents clickjacking (replaces X-Frame-Options)
    ///   - form-action  'self'         : form submissions only to same origin
    ///   - base-uri     'self'         : prevents base-tag hijacking
    ///   - object-src   'none'         : disables Flash / plugins
    ///
    /// To customise the policy (e.g. allow a CDN), update the CspPolicy constant or
    /// read it from an environment variable / appSetting so it can be overridden per
    /// environment without a code change.
    /// </summary>
    public class ContentSecurityPolicyModule : IHttpModule
    {
        // ---------------------------------------------------------------------------
        // CSP policy string.
        // Read from the environment variable CSP_POLICY if set; otherwise fall back to
        // the compile-time default.  This allows AWS ECS / Elastic Beanstalk task
        // definitions or Parameter Store injection to override the policy at runtime.
        // ---------------------------------------------------------------------------
        private static readonly string CspPolicy =
            System.Environment.GetEnvironmentVariable("CSP_POLICY")
            ?? System.Configuration.ConfigurationManager.AppSettings["CspPolicy"]
            ?? "default-src 'self'; "
             + "script-src 'self'; "
             + "style-src 'self' 'unsafe-inline'; "
             + "img-src 'self' data: https:; "
             + "font-src 'self' https:; "
             + "connect-src 'self'; "
             + "frame-ancestors 'none'; "
             + "form-action 'self'; "
             + "base-uri 'self'; "
             + "object-src 'none'";

        // ---------------------------------------------------------------------------
        // IHttpModule implementation
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Initialises the module and subscribes to the PreSendRequestHeaders event so
        /// that the CSP header is added as late as possible (after all handlers have run)
        /// but before the response headers are flushed to the client.
        /// </summary>
        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        /// <summary>
        /// Disposes any resources held by the module.  Nothing to release here.
        /// </summary>
        public void Dispose()
        {
            // No unmanaged resources to release.
        }

        // ---------------------------------------------------------------------------
        // Event handler
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Adds the Content-Security-Policy header (and complementary security headers)
        /// to the outgoing response.  Existing headers are replaced to avoid duplicates
        /// when the module is invoked multiple times (e.g. child requests).
        /// </summary>
        private static void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application == null)
                return;

            HttpResponse response = application.Response;
            if (response == null)
                return;

            try
            {
                // Primary CSP header
                response.Headers.Set("Content-Security-Policy", CspPolicy);

                // Complementary security headers recommended for cloud / SOC2 compliance
                // X-Content-Type-Options: prevents MIME-type sniffing
                response.Headers.Set("X-Content-Type-Options", "nosniff");

                // X-Frame-Options: belt-and-suspenders alongside frame-ancestors in CSP
                response.Headers.Set("X-Frame-Options", "DENY");

                // X-XSS-Protection: legacy browser XSS filter (belt-and-suspenders)
                response.Headers.Set("X-XSS-Protection", "1; mode=block");

                // Referrer-Policy: limits referrer information sent to third parties
                response.Headers.Set("Referrer-Policy", "strict-origin-when-cross-origin");

                // Permissions-Policy: disables browser features not needed by this app
                response.Headers.Set("Permissions-Policy",
                    "geolocation=(), microphone=(), camera=(), payment=()");
            }
            catch (Exception ex)
            {
                // Log but do not rethrow — a header injection failure must never break
                // the application response.
                System.Diagnostics.Trace.TraceWarning(
                    "[ContentSecurityPolicyModule] Failed to set security headers: {0}", ex.Message);
            }
        }
    }
}
