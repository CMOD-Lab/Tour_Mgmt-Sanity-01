// cr-dotnet-1041: Content Security Policy HttpModule
// Implements CSP headers for ASP.NET Web Forms to meet cloud security compliance
// requirements (SOC2, penetration test) in AWS cloud environments.
// Combined with AWS WAF managed rules for defence-in-depth XSS protection.

using System;
using System.Web;

namespace Tour_Management
{
    /// <summary>
    /// Custom IHttpModule that injects a Content-Security-Policy header into every
    /// HTTP response served by this ASP.NET Web Forms application.
    ///
    /// Registration:
    ///   - Classic pipeline : system.web/httpModules
    ///   - Integrated pipeline: system.webServer/modules  (see Web.config)
    ///
    /// AWS WAF complement:
    ///   Deploy the "AWSManagedRulesCommonRuleSet" and
    ///   "AWSManagedRulesKnownBadInputsRuleSet" managed rule groups on the WAF
    ///   WebACL associated with the ALB / CloudFront distribution in front of
    ///   this application to enforce XSS filtering at the edge layer.
    /// </summary>
    public class ContentSecurityPolicyModule : IHttpModule
    {
        // ---------------------------------------------------------------------------
        // CSP policy value.
        // Adjust the directives below to match the application's actual resource
        // origins.  The defaults here follow a strict deny-by-default posture that
        // satisfies SOC2 and common penetration-test requirements:
        //   - default-src 'self'          : only same-origin resources allowed
        //   - script-src  'self'          : no inline scripts, no eval
        //   - style-src   'self' 'unsafe-inline' : inline styles permitted (Web Forms
        //                                  uses them for validation controls)
        //   - img-src     'self' data:    : data URIs needed for chart images
        //   - font-src    'self'          : same-origin fonts only
        //   - connect-src 'self'          : XHR / fetch to same origin only
        //   - frame-ancestors 'none'      : prevents clickjacking (replaces X-Frame-Options)
        //   - form-action  'self'         : form POSTs to same origin only
        //   - base-uri     'self'         : prevents base-tag injection
        //   - object-src   'none'         : no Flash / plugins
        // ---------------------------------------------------------------------------
        private const string CspHeaderName  = "Content-Security-Policy";
        private const string CspHeaderValue =
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

        // Additional security headers added alongside CSP for defence-in-depth
        private const string XContentTypeOptionsHeader = "X-Content-Type-Options";
        private const string XContentTypeOptionsValue  = "nosniff";

        private const string ReferrerPolicyHeader = "Referrer-Policy";
        private const string ReferrerPolicyValue  = "strict-origin-when-cross-origin";

        // ---------------------------------------------------------------------------

        /// <summary>Wires up the PreSendRequestHeaders event.</summary>
        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        /// <summary>Injects CSP and companion security headers before headers are sent.</summary>
        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application == null)
                return;

            HttpResponse response = application.Response;
            if (response == null)
                return;

            // Avoid duplicate headers (e.g. when the module fires more than once
            // for a single request, which can happen with child requests).
            if (response.Headers[CspHeaderName] == null)
            {
                response.Headers.Set(CspHeaderName, CspHeaderValue);
            }

            if (response.Headers[XContentTypeOptionsHeader] == null)
            {
                response.Headers.Set(XContentTypeOptionsHeader, XContentTypeOptionsValue);
            }

            if (response.Headers[ReferrerPolicyHeader] == null)
            {
                response.Headers.Set(ReferrerPolicyHeader, ReferrerPolicyValue);
            }
        }

        /// <summary>Releases resources held by the module.</summary>
        public void Dispose()
        {
            // No unmanaged resources to release.
        }
    }
}
