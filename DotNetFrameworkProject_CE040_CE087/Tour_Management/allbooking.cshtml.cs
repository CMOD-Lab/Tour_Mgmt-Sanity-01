using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for AllBooking - migrated from ASP.NET Web Forms (allbooking.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    ///
    /// Cloud Readiness Fix (Rule: cr-dotnet-1034):
    /// Replaces synchronous asp:GridView data binding and asp:SqlDataSource SelectCommand
    /// with async Task-based patterns using Entity Framework Core connected to Amazon RDS.
    /// This prevents thread pool exhaustion under load and enables efficient auto-scaling
    /// in cloud deployments.
    ///
    /// Key changes from original Web Forms implementation:
    ///   - Removed: asp:SqlDataSource synchronous SelectCommand (SELECT * FROM [booking])
    ///   - Removed: asp:GridView synchronous DataBind() / DataSourceID binding
    ///   - Added:   async OnGetAsync() using EF Core DbContext.Bookings.ToListAsync()
    ///   - Added:   Amazon RDS connection via DB_CONNECTION_STRING environment variable
    /// </summary>
    public class AllBookingModel : PageModel
    {
        private readonly TourManagementDbContext _context;
        private readonly ILogger<AllBookingModel> _logger;

        public AllBookingModel(TourManagementDbContext context, ILogger<AllBookingModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<BookingViewModel> Bookings { get; private set; } = new List<BookingViewModel>();
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Async GET handler - replaces synchronous asp:GridView data binding.
        /// Uses EF Core async query to prevent thread pool exhaustion under cloud load.
        /// Connects to Amazon RDS via DB_CONNECTION_STRING environment variable.
        /// Replaces original asp:SqlDataSource SelectCommand: SELECT * FROM [booking]
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadBookingsAsync();
        }

        private async Task LoadBookingsAsync()
        {
            try
            {
                // Async EF Core query - prevents thread pool exhaustion under load
                // Replaces synchronous asp:SqlDataSource SelectCommand:
                //   SELECT * FROM [booking]
                var bookings = await _context.Bookings
                    .AsNoTracking()
                    .ToListAsync();

                Bookings = new List<BookingViewModel>();
                foreach (var b in bookings)
                {
                    Bookings.Add(new BookingViewModel
                    {
                        TourId    = b.TourId,
                        TourName  = b.TourName,
                        Place     = b.Place,
                        Email     = b.Email,
                        FirstName = b.FirstName
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bookings from Amazon RDS");
                ErrorMessage = $"Unable to load bookings: {ex.Message}";
            }
        }
    }

    /// <summary>
    /// View model representing a single booking record for the all-bookings page.
    /// </summary>
    public class BookingViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Place { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
    }
}
