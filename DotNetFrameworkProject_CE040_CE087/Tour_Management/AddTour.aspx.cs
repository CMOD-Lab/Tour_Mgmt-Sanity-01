// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Replaced: System.Web.UI.Page inheritance with Razor PageModel
// Replaced: Web Forms code-behind pattern with ASP.NET Core Razor Page model
// cr-dotnet-1032: Replaced Server.MapPath / local filesystem file upload with Amazon S3 (AWS SDK for .NET)
// File uploads are now stored in an S3 bucket configured via the AWS_S3_BUCKET_NAME environment variable.
// AWS credentials are resolved automatically via the AWS SDK credential chain
// (IAM role, environment variables, ~/.aws/credentials, etc.).
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
// environment variable and AWS Systems Manager Parameter Store resolution.
// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer used.
// Configuration is injected at runtime via:
//   1. RDS_CONNECTION_STRING environment variable (highest priority)
//   2. AWS SSM Parameter Store key /tour-management/dbconnection (injected as SSM_DBCONNECTION env var)
// This enables immutable deployments and true infrastructure-as-code on AWS.
using System;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Tour_Management.Pages
{
    public class AddTourModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AddTourModel> _logger;

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

        public AddTourModel(IConfiguration configuration, ILogger<AddTourModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // cr-dotnet-0010: Retrieve the connection string from environment variables or
        // AWS Systems Manager Parameter Store — NOT from Web.config / ConfigurationManager.
        // Priority order:
        //   1. RDS_CONNECTION_STRING environment variable (set in ECS task definition / Elastic Beanstalk env)
        //   2. SSM_DBCONNECTION environment variable (injected from /tour-management/dbconnection SSM key)
        //   3. appsettings.json "dbconnection" entry (local development fallback only)
        // This replaces the Web.config <connectionStrings> entry and eliminates the need for
        // Web.Debug.config / Web.Release.config build-time transformations.
        private string GetConnectionString()
        {
            // 1. Check environment variable first (highest priority — set at runtime in AWS)
            string envConnStr = Environment.GetEnvironmentVariable("RDS_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;

            // 2. Fall back to AWS SSM Parameter Store via environment variable indirection.
            //    The SSM parameter value is injected as an environment variable by the
            //    ECS task definition or Elastic Beanstalk configuration using the SSM integration.
            //    Parameter Store key: /tour-management/dbconnection
            string ssmInjected = Environment.GetEnvironmentVariable("SSM_DBCONNECTION");
            if (!string.IsNullOrEmpty(ssmInjected))
                return ssmInjected;

            // 3. Local development fallback via appsettings.json (not used in cloud deployments)
            return _configuration.GetConnectionString("dbconnection");
        }

        // cr-dotnet-1032: Upload the tour image to Amazon S3 instead of the local filesystem.
        // The target S3 bucket is read from the AWS_S3_BUCKET_NAME environment variable
        // (falls back to the "AWS:S3BucketName" appsettings key).
        // Files are stored under the "Tour_pics/" prefix, mirroring the original folder structure.
        // Returns the S3 object key (file name) stored in the database.
        private string UploadImageToS3(IFormFile file)
        {
            string bucketName = Environment.GetEnvironmentVariable("AWS_S3_BUCKET_NAME")
                                ?? _configuration["AWS:S3BucketName"];

            if (string.IsNullOrEmpty(bucketName))
                throw new InvalidOperationException(
                    "S3 bucket name is not configured. Set the AWS_S3_BUCKET_NAME environment variable or the AWS:S3BucketName configuration key.");

            string awsRegion = Environment.GetEnvironmentVariable("AWS_REGION")
                               ?? _configuration["AWS:Region"]
                               ?? "us-east-1";

            string s3Key = $"Tour_pics/{file.FileName}";

            using (var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(awsRegion)))
            using (var transferUtility = new TransferUtility(s3Client))
            using (var stream = file.OpenReadStream())
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName  = bucketName,
                    Key         = s3Key,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    // Make the uploaded object publicly readable so it can be served as a tour image.
                    CannedACL   = S3CannedACL.PublicRead
                };

                transferUtility.Upload(uploadRequest);
            }

            _logger.LogInformation("Tour image '{FileName}' uploaded to S3 bucket '{Bucket}' with key '{Key}'.",
                file.FileName, bucketName, s3Key);

            // Return only the file name; the full S3 URL can be constructed at display time.
            return file.FileName;
        }

        public void OnGet()
        {
            // Page load - no action required
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string picFileName = string.Empty;

                // cr-dotnet-1032: Upload tour image to Amazon S3 (replaces Server.MapPath / local disk save).
                if (TourImage != null && TourImage.Length > 0)
                {
                    picFileName = UploadImageToS3(TourImage);
                }

                // Use Dapper with RDS Proxy connection string for cloud-native connection pooling.
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";
                    conn.Execute(insertQuery, new
                    {
                        TOUR_NAME = TourName,
                        PLACE     = Place,
                        DAYS      = Days,
                        PRICE     = Price,
                        LOCATIONS = Locations,
                        TOUR_INFO = TourInfo,
                        pic       = picFileName
                    });
                }

                StatusMessage = "ADD Successful";
                _logger.LogInformation("Tour '{TourName}' added successfully.", TourName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tour '{TourName}'.", TourName);
                StatusMessage = "An error occurred while adding the tour.";
            }

            return Page();
        }
    }
}
