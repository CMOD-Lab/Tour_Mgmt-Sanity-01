// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.RazorPages (ASP.NET Core MVC)
// cr-dotnet-1032: Replaced Server.MapPath / local filesystem file save with Amazon S3 upload
//                 using AWSSDK.S3, enabling stateless, scalable file operations across
//                 ephemeral cloud instances.
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
//                 Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") so that
//                 configuration is injected at runtime (AWS ECS/EB environment variable or
//                 AWS Systems Manager Parameter Store) rather than baked into build artifacts
//                 via Web.Debug.config / Web.Release.config XDT transformations.
using System;
using System.Data;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
// cr-dotnet-0010: IConfiguration used as fallback for local development only;
// in AWS cloud environments DB_CONNECTION_STRING environment variable takes precedence.
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Tour_Management.Pages
{
    // Migrated from System.Web.UI.Page to ASP.NET Core Razor Pages PageModel (cr-dotnet-0026)
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

        // cr-dotnet-0010: Retrieve the connection string from the DB_CONNECTION_STRING
        // environment variable (set in AWS ECS task definition, Elastic Beanstalk
        // environment properties, or injected from AWS Systems Manager Parameter Store).
        // Falls back to appsettings.json / Web.config connectionString for local development.
        // Web.Debug.config and Web.Release.config XDT transforms are no longer used.
        private string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection");
        }

        // cr-dotnet-1032: Upload a file stream to Amazon S3.
        // Bucket name and AWS region are read from environment variables
        // (S3_BUCKET_NAME, AWS_REGION) so no credentials or paths are
        // hard-coded, satisfying 12-factor app principle III (Config).
        private string UploadToS3(IFormFile file)
        {
            string bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME")
                ?? _configuration["AWS:S3BucketName"];
            string regionName = Environment.GetEnvironmentVariable("AWS_REGION")
                ?? _configuration["AWS:Region"]
                ?? "us-east-1";

            string keyName = $"Tour_pics/{Path.GetFileName(file.FileName)}";

            var region = RegionEndpoint.GetBySystemName(regionName);
            using (var s3Client = new AmazonS3Client(region))
            using (var transferUtility = new TransferUtility(s3Client))
            using (var stream = file.OpenReadStream())
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName  = bucketName,
                    Key         = keyName,
                    InputStream = stream,
                    ContentType = file.ContentType
                };
                transferUtility.Upload(uploadRequest);
            }

            // Return only the S3 object key (file name) to store in the database,
            // matching the original behaviour of storing FileUpload1.FileName.
            return Path.GetFileName(file.FileName);
        }

        // Replaces Web Forms Page_Load event handler
        public void OnGet()
        {
        }

        // Replaces Web Forms Register_Click event handler
        public IActionResult OnPost()
        {
            // Use Dapper with RDS Proxy-backed SqlConnection for cloud-native connection pooling.
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";

                string fileName = string.Empty;
                if (TourImage != null && TourImage.Length > 0)
                {
                    // cr-dotnet-1032: Replaced Server.MapPath("~/Tour_pics/") + local FileStream
                    // save with an Amazon S3 upload via TransferUtility.  The S3 bucket is
                    // configured through the S3_BUCKET_NAME environment variable, making file
                    // storage stateless and durable across ephemeral cloud instances.
                    fileName = UploadToS3(TourImage);
                }

                conn.Execute(insertQuery, new
                {
                    TOUR_NAME = TourName,
                    PLACE     = Place,
                    DAYS      = Days,
                    PRICE     = Price,
                    LOCATIONS = Locations,
                    TOUR_INFO = TourInfo,
                    pic       = fileName
                });

                StatusMessage = "ADD Successful";
            }

            return Page();
        }
    }
}
