using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for AddTour - migrated from ASP.NET Web Forms (AddTour.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    ///
    /// cr-dotnet-1032: Server.MapPath("~/Tour_pics/") replaced with Amazon S3 PutObjectRequest.
    /// File uploads are now stored in an S3 bucket configured via the S3_BUCKET_NAME
    /// environment variable, enabling stateless, scalable file operations across
    /// ephemeral cloud instances.
    /// </summary>
    public class AddTourModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public AddTourModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string TourName { get; set; }

        [BindProperty]
        public string Place { get; set; }

        [BindProperty]
        public string Days { get; set; }

        [BindProperty]
        public string Locations { get; set; }

        [BindProperty]
        public string Price { get; set; }

        [BindProperty]
        public string TourInfo { get; set; }

        [BindProperty]
        public IFormFile TourImage { get; set; }

        public string StatusMessage { get; set; }

        public void OnGet()
        {
            // Page initialization - no action required on GET
        }

        public IActionResult OnPostRegister()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Use connection string from environment variable (cloud-native pattern)
            // or fall back to configuration for local development
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection");

            // cr-dotnet-1032 FIX: Replace Server.MapPath("~/Tour_pics/") with Amazon S3 upload.
            // Original Web Forms code (AddTour.aspx.cs, line 33):
            //   FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
            // Replaced with: Amazon S3 PutObjectRequest using AWSSDK.S3, enabling stateless
            // file access and persistence across ephemeral cloud instances.
            string fileName = string.Empty;
            if (TourImage != null && TourImage.Length > 0)
            {
                fileName = TourImage.FileName;

                // Retrieve S3 configuration from environment variables (12-factor app principle)
                string bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME")
                    ?? _configuration["AWS:S3BucketName"]
                    ?? "tour-management-pics";

                string awsRegion = Environment.GetEnvironmentVariable("AWS_REGION")
                    ?? _configuration["AWS:Region"]
                    ?? "us-east-1";

                // S3 object key: store under "Tour_pics/" prefix to mirror original folder structure
                string s3Key = $"Tour_pics/{fileName}";

                // Upload file to Amazon S3 using AWS SDK for .NET
                // AWS credentials are resolved automatically via the AWS credential provider chain:
                //   1. Environment variables (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY)
                //   2. IAM instance profile / ECS task role (recommended for cloud deployments)
                //   3. AWS credentials file (~/.aws/credentials)
                using (var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(awsRegion)))
                using (var inputStream = TourImage.OpenReadStream())
                {
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = s3Key,
                        InputStream = inputStream,
                        ContentType = TourImage.ContentType,
                        // Make the object publicly readable so tour images can be served via S3 URL
                        CannedACL = S3CannedACL.PublicRead
                    };

                    // Execute the S3 upload synchronously (Web Forms compatibility)
                    s3Client.PutObjectAsync(putRequest).GetAwaiter().GetResult();
                }
            }

            // Use Dapper with Amazon RDS Proxy connection string for cloud-native connection pooling
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) " +
                                     "values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";

                conn.Execute(insertQuery, new
                {
                    TOUR_NAME = TourName,
                    PLACE = Place,
                    DAYS = Days,
                    PRICE = Price,
                    LOCATIONS = Locations,
                    TOUR_INFO = TourInfo,
                    pic = fileName
                });
            }

            StatusMessage = "ADD Successful";
            return Page();
        }
    }
}
