using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for TourCrud - migrated from ASP.NET Web Forms (TourCrud.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    ///
    /// Cloud Readiness Fix (Rule: cr-dotnet-1034):
    /// Replaces synchronous asp:GridView data binding and asp:SqlDataSource
    /// SelectCommand/UpdateCommand/DeleteCommand with async Task-based patterns using
    /// Entity Framework Core connected to Amazon RDS. This prevents thread pool
    /// exhaustion under load and enables efficient auto-scaling in cloud deployments.
    ///
    /// Key changes from original Web Forms implementation:
    ///   - Removed: asp:SqlDataSource synchronous SelectCommand/UpdateCommand/DeleteCommand
    ///   - Removed: asp:GridView synchronous DataBind() / DataSourceID binding
    ///   - Added:   async OnGetAsync() using EF Core DbContext.Tours.ToListAsync()
    ///   - Added:   async OnPostUpdateAsync() using EF Core SaveChangesAsync()
    ///   - Added:   async OnPostDeleteAsync() using EF Core SaveChangesAsync()
    ///   - Added:   Amazon RDS connection via DB_CONNECTION_STRING environment variable
    /// </summary>
    public class TourCrudModel : PageModel
    {
        private readonly TourManagementDbContext _context;
        private readonly ILogger<TourCrudModel> _logger;

        public TourCrudModel(TourManagementDbContext context, ILogger<TourCrudModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<TourCrudViewModel> Tours { get; private set; } = new List<TourCrudViewModel>();
        public string ErrorMessage { get; private set; }
        public string StatusMessage { get; private set; }
        public bool IsSuccess { get; private set; }
        public int? EditingTourId { get; private set; }

        // Bound properties for edit/update form
        [BindProperty]
        public int TourId { get; set; }

        [BindProperty]
        public string TourName { get; set; }

        [BindProperty]
        public string Place { get; set; }

        [BindProperty]
        public string Days { get; set; }

        [BindProperty]
        public decimal Price { get; set; }

        [BindProperty]
        public string Locations { get; set; }

        [BindProperty]
        public string TourInfo { get; set; }

        /// <summary>
        /// Async GET handler - replaces synchronous asp:GridView data binding.
        /// Uses EF Core async query to prevent thread pool exhaustion under cloud load.
        /// Connects to Amazon RDS via DB_CONNECTION_STRING environment variable.
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadToursAsync();
        }

        /// <summary>
        /// Handles the Edit button click — sets the editing tour ID and reloads the page
        /// with the selected row in edit mode.
        /// </summary>
        public async Task<IActionResult> OnPostEditAsync()
        {
            EditingTourId = TourId;
            await LoadToursAsync();
            return Page();
        }

        /// <summary>
        /// Handles the Cancel Edit button click — returns to read-only view.
        /// </summary>
        public async Task<IActionResult> OnPostCancelEditAsync()
        {
            await LoadToursAsync();
            return Page();
        }

        /// <summary>
        /// Async Update handler - replaces synchronous asp:SqlDataSource UpdateCommand.
        /// Uses EF Core async SaveChangesAsync() to prevent thread pool exhaustion.
        /// Replaces original UpdateCommand:
        ///   UPDATE [Tour] SET [TOUR_NAME]=@TOUR_NAME, [PLACE]=@PLACE, [DAYS]=@DAYS,
        ///   [PRICE]=@PRICE, [LOCATIONS]=@LOCATIONS, [TOUR_INFO]=@TOUR_INFO
        ///   WHERE [TOUR_ID]=@TOUR_ID
        /// </summary>
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            try
            {
                // Async EF Core update - prevents thread pool exhaustion under cloud load
                var tour = await _context.Tours.FindAsync(TourId);
                if (tour != null)
                {
                    tour.TourName  = TourName;
                    tour.Place     = Place;
                    tour.Days      = Days;
                    tour.Price     = Price;
                    tour.Locations = Locations;
                    tour.TourInfo  = TourInfo;

                    await _context.SaveChangesAsync();
                    IsSuccess = true;
                    StatusMessage = "Tour updated successfully.";
                }
                else
                {
                    StatusMessage = $"Tour with ID {TourId} not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tour {TourId} in Amazon RDS", TourId);
                StatusMessage = $"Update failed: {ex.Message}";
            }

            await LoadToursAsync();
            return Page();
        }

        /// <summary>
        /// Async Delete handler - replaces synchronous asp:SqlDataSource DeleteCommand.
        /// Uses EF Core async SaveChangesAsync() to prevent thread pool exhaustion.
        /// Replaces original DeleteCommand:
        ///   DELETE FROM [Tour] WHERE [TOUR_ID]=@TOUR_ID
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            try
            {
                // Async EF Core delete - prevents thread pool exhaustion under cloud load
                var tour = await _context.Tours.FindAsync(TourId);
                if (tour != null)
                {
                    _context.Tours.Remove(tour);
                    await _context.SaveChangesAsync();
                    IsSuccess = true;
                    StatusMessage = "Tour deleted successfully.";
                }
                else
                {
                    StatusMessage = $"Tour with ID {TourId} not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tour {TourId} from Amazon RDS", TourId);
                StatusMessage = $"Delete failed: {ex.Message}";
            }

            await LoadToursAsync();
            return Page();
        }

        private async Task LoadToursAsync()
        {
            try
            {
                // Async EF Core query - prevents thread pool exhaustion under load
                // Replaces synchronous asp:SqlDataSource SelectCommand:
                //   SELECT [TOUR_ID], [TOUR_NAME], [PLACE], [DAYS], [PRICE],
                //          [LOCATIONS], [TOUR_INFO], [pic] FROM [Tour]
                var tours = await _context.Tours
                    .AsNoTracking()
                    .ToListAsync();

                Tours = new List<TourCrudViewModel>();
                foreach (var t in tours)
                {
                    Tours.Add(new TourCrudViewModel
                    {
                        TourId    = t.TourId,
                        TourName  = t.TourName,
                        Place     = t.Place,
                        Days      = t.Days,
                        Price     = t.Price,
                        Locations = t.Locations,
                        TourInfo  = t.TourInfo,
                        Pic       = t.Pic
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
    /// View model representing a single tour record for the CRUD management page.
    /// </summary>
    public class TourCrudViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Place { get; set; }
        public string Days { get; set; }
        public decimal Price { get; set; }
        public string Locations { get; set; }
        public string TourInfo { get; set; }
        public string Pic { get; set; }
    }
}
