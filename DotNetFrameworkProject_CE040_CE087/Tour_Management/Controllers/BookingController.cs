using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tour_Management.Data;
using Tour_Management.Models;

namespace Tour_Management.Controllers
{
    /// <summary>
    /// ASP.NET MVC controller replacing TourCrud.aspx, allbooking.aspx, and mybooking.aspx Web Forms.
    /// cr-dotnet-1034: Synchronous GridView data binding replaced with async Task-based
    /// patterns using Entity Framework 6 connected to Amazon RDS, preventing thread pool
    /// exhaustion under cloud load and enabling efficient auto-scaling.
    ///
    /// Migrated from Web Forms (System.Web.UI.Page) to ASP.NET MVC Controller pattern.
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// </summary>
    public class BookingController : Controller
    {
        // -----------------------------------------------------------------------
        // GET: /Booking/TourCrud
        // Replaces: TourCrud.aspx (GridView with SqlDataSource for tour CRUD)
        // cr-dotnet-1034: Synchronous GridView data binding (TourCrud.aspx lines 13, 40)
        // replaced with async Task-based EF Core query on Amazon RDS.
        // Uses ToListAsync() to free the request thread during database I/O.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult> TourCrud()
        {
            List<TourCrudViewModel> tours;

            // cr-dotnet-1034: Async EF Core query replacing synchronous SqlDataSource +
            // GridView data binding. ToListAsync() releases the thread during RDS I/O.
            using (var db = new TourManagementDbContext())
            {
                tours = await db.Tours
                    .Select(t => new TourCrudViewModel
                    {
                        TourId    = t.TourId,
                        TourName  = t.TourName,
                        Place     = t.Place,
                        Days      = t.Days,
                        Price     = t.Price,
                        Locations = t.Locations,
                        TourInfo  = t.TourInfo,
                        Pic       = t.Pic
                    })
                    .ToListAsync();
            }

            return View(tours);
        }

        // -----------------------------------------------------------------------
        // POST: /Booking/UpdateTour
        // Replaces: TourCrud.aspx SqlDataSource UpdateCommand
        // cr-dotnet-1034: Async EF Core update replacing synchronous SqlDataSource UpdateCommand.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateTour(TourCrudViewModel model)
        {
            // cr-dotnet-1034: Async EF Core update replacing synchronous SqlDataSource UpdateCommand.
            using (var db = new TourManagementDbContext())
            {
                var tour = await db.Tours.FindAsync(model.TourId);
                if (tour != null)
                {
                    tour.TourName  = model.TourName;
                    tour.Place     = model.Place;
                    tour.Days      = model.Days;
                    tour.Price     = model.Price;
                    tour.Locations = model.Locations;
                    tour.TourInfo  = model.TourInfo;
                    await db.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Tour updated successfully.";
            return RedirectToAction("TourCrud");
        }

        // -----------------------------------------------------------------------
        // POST: /Booking/DeleteTour
        // Replaces: TourCrud.aspx SqlDataSource DeleteCommand
        // cr-dotnet-1034: Async EF Core delete replacing synchronous SqlDataSource DeleteCommand.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteTour(int tourId)
        {
            // cr-dotnet-1034: Async EF Core delete replacing synchronous SqlDataSource DeleteCommand.
            using (var db = new TourManagementDbContext())
            {
                var tour = await db.Tours.FindAsync(tourId);
                if (tour != null)
                {
                    db.Tours.Remove(tour);
                    await db.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Tour deleted successfully.";
            return RedirectToAction("TourCrud");
        }

        // -----------------------------------------------------------------------
        // GET: /Booking/AllBooking
        // Replaces: allbooking.aspx (GridView with SqlDataSource for all bookings)
        // cr-dotnet-1034: Synchronous GridView data binding (allbooking.aspx lines 15, 33)
        // replaced with async Task-based EF Core query on Amazon RDS.
        // Uses ToListAsync() to free the request thread during database I/O.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult> AllBooking()
        {
            List<BookingViewModel> bookings;

            // cr-dotnet-1034: Async EF Core query replacing synchronous SqlDataSource +
            // GridView data binding. ToListAsync() releases the thread during RDS I/O.
            using (var db = new TourManagementDbContext())
            {
                bookings = await db.Bookings
                    .Select(b => new BookingViewModel
                    {
                        TourId    = b.TourId,
                        TourName  = b.TourName,
                        Place     = b.Place,
                        Email     = b.Email,
                        FirstName = b.FirstName
                    })
                    .ToListAsync();
            }

            return View(bookings);
        }

        // -----------------------------------------------------------------------
        // GET: /Booking/MyBooking
        // Replaces: mybooking.aspx (GridView with SqlDataSource for user's bookings)
        // cr-dotnet-1034: Synchronous GridView data binding (mybooking.aspx lines 12, 28)
        // replaced with async Task-based EF Core query on Amazon RDS.
        // Uses ToListAsync() to free the request thread during database I/O.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult> MyBooking()
        {
            List<MyBookingViewModel> bookings;

            // cr-dotnet-1034: Async EF Core query replacing synchronous SqlDataSource +
            // GridView data binding. ToListAsync() releases the thread during RDS I/O.
            using (var db = new TourManagementDbContext())
            {
                bookings = await db.Bookings
                    .Select(b => new MyBookingViewModel
                    {
                        TourId   = b.TourId,
                        TourName = b.TourName
                    })
                    .ToListAsync();
            }

            return View(bookings);
        }

        // -----------------------------------------------------------------------
        // POST: /Booking/DeleteBooking
        // Replaces: mybooking.aspx SqlDataSource DeleteCommand
        // cr-dotnet-1034: Async EF Core delete replacing synchronous SqlDataSource DeleteCommand.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteBooking(int tourId)
        {
            // cr-dotnet-1034: Async EF Core delete replacing synchronous SqlDataSource DeleteCommand.
            using (var db = new TourManagementDbContext())
            {
                var booking = await db.Bookings.FindAsync(tourId);
                if (booking != null)
                {
                    db.Bookings.Remove(booking);
                    await db.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Booking deleted successfully.";
            return RedirectToAction("MyBooking");
        }
    }
}
