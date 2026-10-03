using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for DisplayTours - migrated from ASP.NET Web Forms (DisplayTours.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    ///
    /// Cloud Readiness Fix (Rule: cr-dotnet-1034):
    /// Replaces synchronous asp:GridView data binding (DataSourceID="SqlDataSource1") and
    /// asp:SqlDataSource SelectCommand with async Task-based patterns using Entity Framework
    /// Core connected to Amazon RDS. This prevents thread pool exhaustion under load and
    /// enables efficient auto-scaling in cloud deployments.
    ///
    /// Key changes from original Web Forms implementation:
    ///   - Removed: asp:SqlDataSource synchronous SelectCommand
    ///   - Removed: asp:GridView synchronous DataBind() / DataSourceID binding
    ///   - Added:   async OnGetAsync() using EF Core DbContext.Tours.ToListAsync()
    ///   - Added:   Amazon RDS connection via DB_CONNECTION_STRING environment variable
    /// </summary>
    public class DisplayToursModel : PageModel
    {
        private readonly TourManagementDbContext _context;
        private readonly ILogger<DisplayToursModel> _logger;

        public DisplayToursModel(TourManagementDbContext context, ILogger<DisplayToursModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<TourViewModel> Tours { get; private set; } = new List<TourViewModel>();
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Async GET handler - replaces synchronous asp:GridView data binding.
        /// Uses EF Core async query to prevent thread pool exhaustion under cloud load.
        /// Connects to Amazon RDS via DB_CONNECTION_STRING environment variable.
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                // Async EF Core query - prevents thread pool exhaustion under load
                // Replaces synchronous asp:SqlDataSource SelectCommand:
                //   SELECT [TOUR_NAME], [pic], [PRICE], [DAYS], [LOCATIONS], [TOUR_ID] FROM [Tour]
                var tours = await _context.Tours
                    .AsNoTracking()
                    .ToListAsync();

                Tours = new List<TourViewModel>();
                foreach (var t in tours)
                {
                    Tours.Add(new TourViewModel
                    {
                        TourId    = t.TourId,
                        TourName  = t.TourName,
                        Pic       = t.Pic,
                        Price     = t.Price,
                        Days      = t.Days,
                        Locations = t.Locations
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tours from Amazon RDS");
                ErrorMessage = $"Unable to load tours: {ex.Message}";
            }
        }
    }

    /// <summary>
    /// View model representing a single tour record from the database.
    /// </summary>
    public class TourViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Pic { get; set; }
        public decimal Price { get; set; }
        public string Days { get; set; }
        public string Locations { get; set; }
    }
}
