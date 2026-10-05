using System;
using System.Configuration;
using System.Web;

namespace Tour_Management.Security
{
    /// <summary>
    /// ASP.NET Web Forms HttpModule that injects Content-Security-Policy (CSP) headers
    /// into every HTTP response.
    ///
    /// Cloud readiness fix for rule cr-dotnet-1041:
    ///   "Missing Content Security Policy Headers"
    ///
    /// The CSP policy is read from appSettings so it can be overridden at runtime via
    /// environment variables / AWS Systems Manager Parameter Store / AWS Secrets Manager
    /// without redeploying the application.
    ///
    /// Key: CSP:Policy
    /// Default (if key is absent): a strict policy that covers the application's own
    ///   origins and common CDN sources used by the Tour Management UI.
    ///
    /// AWS WAF managed rules (AWSManagedRulesCommonRuleSet) should be enabled on the
    /// ALB / CloudFront distribution in front of this application to provide an
    /// additional layer of XSS protection for SOC2 / penetration-test compliance.
    /// </summary>
    public sealed class CspHttpModule : IHttpModule
    {
        // ---------------------------------------------------------------------------
        // Default CSP policy
        // ---------------------------------------------------------------------------
        // Adjust the default to match the actual CDN / font / script origins used by
        // the application.  The value can always be overridden at runtime via the
        // CSP:Policy appSetting (or the corresponding environment variable when the
        // app is hosted on AWS Elastic Beanstalk / ECS / App Runner).
        // ---------------------------------------------------------------------------
        private const string DefaultCspPolicy =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' https://ajax.googleapis.com https://cdnjs.cloudflare.com; " +
            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdnjs.cloudflare.com; " +
            "font-src 'self' https://fonts.gstatic.com; " +
            "img-src 'self' data: https:; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "form-action 'self'; " +
            "base-uri 'self'; " +
            "object-src 'none'";

        // ---------------------------------------------------------------------------
        // IHttpModule implementation
        // ---------------------------------------------------------------------------

        /// <summary>Initialises the module and subscribes to the PostReleaseRequestState event.</summary>
        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            // PostReleaseRequestState fires after the response has been generated but
            // before it is flushed to the client – the ideal place to add security headers.
            context.PostReleaseRequestState += OnPostReleaseRequestState;
        }

        /// <summary>Disposes any resources held by the module (none in this case).</summary>
        public void Dispose()
        {
            // Nothing to dispose.
        }

        // ---------------------------------------------------------------------------
        // Event handler
        // ---------------------------------------------------------------------------

        private static void OnPostReleaseRequestState(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null)
                return;

            HttpResponse response = application.Response;
            if (response == null)
                return;

            // Only add headers to HTML responses; skip binary / API responses.
            string contentType = response.ContentType;
            if (!string.IsNullOrEmpty(contentType) &&
                !contentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Read the policy from configuration so it can be overridden at runtime.
            string cspPolicy = ConfigurationManager.AppSettings["CSP:Policy"];
            if (string.IsNullOrWhiteSpace(cspPolicy))
            {
                cspPolicy = DefaultCspPolicy;
            }

            // Content-Security-Policy
            if (!response.Headers.AllKeys.Contains("Content-Security-Policy"))
            {
                response.Headers.Set("Content-Security-Policy", cspPolicy);
            }

            // X-Content-Type-Options – prevents MIME-type sniffing (complementary header)
            if (!response.Headers.AllKeys.Contains("X-Content-Type-Options"))
            {
                response.Headers.Set("X-Content-Type-Options", "nosniff");
            }

            // X-Frame-Options – belt-and-suspenders protection against clickjacking
            if (!response.Headers.AllKeys.Contains("X-Frame-Options"))
            {
                response.Headers.Set("X-Frame-Options", "DENY");
            }

            // Referrer-Policy
            if (!response.Headers.AllKeys.Contains("Referrer-Policy"))
            {
                response.Headers.Set("Referrer-Policy", "strict-origin-when-cross-origin");
            }

            // Permissions-Policy (formerly Feature-Policy)
            if (!response.Headers.AllKeys.Contains("Permissions-Policy"))
            {
                response.Headers.Set("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            }
        }
    }
}
