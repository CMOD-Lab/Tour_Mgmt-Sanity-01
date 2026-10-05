using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace TourManagement.Web.Pages;

/// <summary>
/// Page model for the error page.
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    /// <summary>Gets the request ID.</summary>
    public string? RequestId { get; set; }

    /// <summary>Gets whether to show the request ID.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    /// <summary>Initializes a new instance of <see cref="ErrorModel"/>.</summary>
    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    /// <summary>Handles GET requests for the error page.</summary>
    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
