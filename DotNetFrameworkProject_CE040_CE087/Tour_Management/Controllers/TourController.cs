using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Tour_Management.Data;
using Tour_Management.Models;

namespace Tour_Management.Controllers
{
    /// <summary>
    /// ASP.NET MVC controller replacing AddTour.aspx and DisplayTours.aspx Web Forms.
    /// cr-dotnet-1034: Synchronous GridView data binding replaced with async Task-based
    /// patterns using Entity Framework 6 connected to Amazon RDS, preventing thread pool
    /// exhaustion under cloud load and enabling efficient auto-scaling.
    ///
    /// Migrated from Web Forms (System.Web.UI.Page) to ASP.NET MVC Controller pattern.
    /// File uploads migrated from Server.MapPath (local filesystem) to Amazon S3
    /// for stateless, scalable file operations across ephemeral cloud instances (cr-dotnet-1032).
    /// </summary>
    public class TourController : Controller
    {
        // -----------------------------------------------------------------------
        // Amazon S3 helper — uploads a file stream to the configured S3 bucket.
        // Bucket name is read from the environment variable S3_TOUR_PICS_BUCKET,
        // falling back to the Web.config appSetting "S3TourPicsBucket".
        // AWS credentials are resolved automatically via the AWS SDK credential
        // chain (IAM role → environment variables → ~/.aws/credentials).
        // Replaces: Server.MapPath("~/Tour_pics/") + FileUpload1.SaveAs(...)
        // -----------------------------------------------------------------------
        private static string UploadTourImageToS3(Stream fileStream, string fileName)
        {
            string bucketName = Environment.GetEnvironmentVariable("S3_TOUR_PICS_BUCKET")
                ?? System.Configuration.ConfigurationManager.AppSettings["S3TourPicsBucket"]
                ?? "tour-management-tour-pics";

            string s3Key = "Tour_pics/" + fileName;

            // AmazonS3Client resolves credentials from the IAM instance role when
            // running on EC2/ECS, or from environment variables
            // (AWS_ACCESS_KEY_ID / AWS_SECRET_ACCESS_KEY) in other environments.
            using (var s3Client = new AmazonS3Client(RegionEndpoint.USEast1))
            using (var transferUtility = new TransferUtility(s3Client))
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName      = bucketName,
                    Key             = s3Key,
                    InputStream     = fileStream,
                    AutoCloseStream = false
                };

                transferUtility.Upload(uploadRequest);
            }

            return fileName;
        }

        // -----------------------------------------------------------------------
        // GET: /Tour/AddTour
        // Replaces: AddTour.aspx Page_Load (initial render)
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult AddTour()
        {
            return View(new AddTourViewModel());
        }

        // -----------------------------------------------------------------------
        // POST: /Tour/AddTour
        // Replaces: AddTour.aspx.cs Register_Click event handler.
        // File upload now targets Amazon S3 instead of the local filesystem
        // (Server.MapPath removed — cr-dotnet-1032).
        // cr-dotnet-1034: Uses async EF Core DbContext to insert tour record,
        // replacing synchronous SqlDataSource InsertCommand.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddTour(AddTourViewModel model, HttpPostedFileBase tourImage)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string picFileName = string.Empty;

            // Handle file upload — streams directly to Amazon S3 Tour_pics/ prefix.
            // Replaces the original: FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + fileName)
            if (tourImage != null && tourImage.ContentLength > 0)
            {
                picFileName = Path.GetFileName(tourImage.FileName);
                UploadTourImageToS3(tourImage.InputStream, picFileName);
            }

            // cr-dotnet-1034: Async EF Core insert replacing synchronous SqlDataSource InsertCommand.
            // Uses async SaveChangesAsync() to avoid blocking the request thread on Amazon RDS I/O.
            using (var db = new TourManagementDbContext())
            {
                var tour = new TourEntity
                {
                    TourName  = model.TourName,
                    Place     = model.Place,
                    Days      = model.Days,
                    Price     = model.Price,
                    Locations = model.Locations,
                    TourInfo  = model.TourInfo,
                    Pic       = picFileName
                };

                db.Tours.Add(tour);
                await db.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "ADD Successful";
            return RedirectToAction("AddTour");
        }

        // -----------------------------------------------------------------------
        // GET: /Tour/DisplayTours
        // Replaces: DisplayTours.aspx (SqlDataSource + GridView Web Form controls)
        // cr-dotnet-1034: Synchronous GridView data binding (DisplayTours.aspx lines 18, 47)
        // replaced with async Task-based EF Core query on Amazon RDS.
        // Uses ToListAsync() to free the request thread during database I/O,
        // enabling efficient auto-scaling in cloud deployments.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult> DisplayTours()
        {
            List<TourViewModel> tours;

            // cr-dotnet-1034: Async EF Core query replacing synchronous SqlDataSource +
            // GridView data binding. ToListAsync() releases the thread during RDS I/O.
            using (var db = new TourManagementDbContext())
            {
                var entities = await db.Tours
                    .Select(t => new TourViewModel
                    {
                        TourId    = t.TourId,
                        TourName  = t.TourName,
                        Pic       = t.Pic,
                        Price     = t.Price,
                        Days      = t.Days,
                        Locations = t.Locations
                    })
                    .ToListAsync();

                tours = entities;
            }

            return View(tours);
        }
    }
}
