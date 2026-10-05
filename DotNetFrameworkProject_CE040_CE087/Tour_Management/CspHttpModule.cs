// cr-dotnet-1041: Content Security Policy HttpModule
// Implements CSP headers for ASP.NET Web Forms to meet cloud security compliance
// requirements (SOC2, penetration test) in AWS cloud environments.
// Works in conjunction with AWS WAF managed rules for defence-in-depth XSS protection.

using System;
using System.Web;

namespace Tour_Management
{
    /// <summary>
    /// Custom HttpModule that injects Content-Security-Policy (CSP) and related
    /// security headers into every HTTP response.  Registered in Web.config under
    /// system.webServer/modules so it runs in IIS Integrated Pipeline mode.
    ///
    /// AWS deployment note:
    ///   This module provides application-layer CSP enforcement.  AWS WAF managed
    ///   rule groups (e.g. AWSManagedRulesCommonRuleSet) should be attached to the
    ///   ALB / CloudFront distribution in front of this application to provide an
    ///   additional layer of XSS and injection protection required for SOC2
    ///   certification and penetration-test compliance in multi-tenant cloud
    ///   environments.
    /// </summary>
    public class CspHttpModule : IHttpModule
    {
        // ---------------------------------------------------------------------------
        // CSP policy value – adjust directives to match your application's actual
        // resource origins.  The policy below follows a strict allow-list approach:
        //   default-src 'self'          – only same-origin resources by default
        //   script-src  'self'          – no inline scripts; add nonces if needed
        //   style-src   'self' 'unsafe-inline' – inline styles kept for Web Forms
        //   img-src     'self' data:    – data URIs needed for chart images
        //   font-src    'self'          – same-origin fonts only
        //   connect-src 'self'          – XHR/fetch to same origin only
        //   frame-ancestors 'none'      – prevents clickjacking (replaces X-Frame-Options)
        //   form-action 'self'          – form submissions to same origin only
        //   base-uri    'self'          – prevents base-tag injection
        //   object-src  'none'          – disables Flash / plugins
        // ---------------------------------------------------------------------------
        private const string CspPolicy =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "form-action 'self'; " +
            "base-uri 'self'; " +
            "object-src 'none'";

        /// <summary>
        /// Initialises the module and subscribes to the PreSendRequestHeaders event
        /// so that security headers are added to every response, including error pages.
        /// </summary>
        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        /// <summary>
        /// Adds Content-Security-Policy and complementary security headers to the
        /// outgoing HTTP response.
        /// </summary>
        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null)
                return;

            HttpResponse response = application.Response;
            if (response == null)
                return;

            // Primary CSP header – enforced by the browser
            SetHeaderIfAbsent(response, "Content-Security-Policy", CspPolicy);

            // Complementary security headers for defence-in-depth
            // X-Content-Type-Options: prevents MIME-type sniffing attacks
            SetHeaderIfAbsent(response, "X-Content-Type-Options", "nosniff");

            // X-Frame-Options: legacy clickjacking protection (CSP frame-ancestors
            // is the modern equivalent but both are set for broad browser support)
            SetHeaderIfAbsent(response, "X-Frame-Options", "DENY");

            // X-XSS-Protection: enables the browser's built-in XSS filter (IE/Edge)
            SetHeaderIfAbsent(response, "X-XSS-Protection", "1; mode=block");

            // Referrer-Policy: limits referrer information sent to third parties
            SetHeaderIfAbsent(response, "Referrer-Policy", "strict-origin-when-cross-origin");

            // Permissions-Policy: disables browser features not required by the app
            SetHeaderIfAbsent(response, "Permissions-Policy",
                "geolocation=(), microphone=(), camera=(), payment=()");
        }

        /// <summary>
        /// Sets a response header only if it has not already been set, preventing
        /// duplicate header values.
        /// </summary>
        private static void SetHeaderIfAbsent(HttpResponse response, string name, string value)
        {
            if (string.IsNullOrEmpty(response.Headers[name]))
            {
                response.Headers[name] = value;
            }
        }

        /// <summary>
        /// Disposes resources held by the module (none in this implementation).
        /// </summary>
        public void Dispose()
        {
            // No unmanaged resources to release.
        }
    }
}
