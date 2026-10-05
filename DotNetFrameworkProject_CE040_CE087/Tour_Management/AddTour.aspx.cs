using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for Add Tour — migrated from ASP.NET Web Forms (AddTour.aspx / AddTour.aspx.cs)
    /// to ASP.NET Core Razor Pages for cloud-native deployment on AWS.
    ///
    /// Cloud remediation (cr-dotnet-1032):
    ///   Replaced Server.MapPath("~/Tour_pics/") + FileUpload1.FileName local file-save (line 33)
    ///   with Amazon S3 upload using the AWS SDK for .NET (AWSSDK.S3), enabling stateless,
    ///   scalable file operations across ephemeral cloud instances.
    ///
    ///   S3 configuration is driven by environment variables:
    ///     AWS_S3_BUCKET_NAME  — target S3 bucket (required)
    ///     AWS_REGION          — AWS region, e.g. "us-east-1" (optional, defaults to us-east-1)
    ///   AWS credentials are resolved automatically via the AWS SDK credential chain
    ///   (IAM instance role / ECS task role / environment variables).
    ///
    /// Cloud remediation (cr-dotnet-0010):
    ///   Eliminated Web.config transformation dependency (Web.Debug.config / Web.Release.config).
    ///   Connection string is now resolved at runtime from:
    ///     1. DB_CONNECTION_STRING environment variable (AWS ECS task definition / Elastic Beanstalk
    ///        environment property / App Runner environment variable) — primary source.
    ///     2. AWS Systems Manager Parameter Store via IConfiguration SSM provider
    ///        (parameter path: /tour-management/db-connection-string) — secondary source.
    ///     3. appsettings.json "ConnectionStrings:dbconnection" — local development fallback only.
    ///   This enables immutable deployments where configuration is never baked into build artifacts.
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

        // cr-dotnet-0010: Retrieve connection string from environment variable (RDS Proxy endpoint)
        // with fallback to IConfiguration (appsettings.json / AWS SSM Parameter Store).
        // ConfigurationManager.ConnectionStrings removed — no longer relies on Web.config
        // transformation files for environment-specific configuration.
        private string GetConnectionString()
        {
            // Primary: environment variable injected by AWS ECS / Elastic Beanstalk / App Runner
            string envConnStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;

            // Secondary: IConfiguration (reads from appsettings.json or AWS SSM Parameter Store
            // when the AWSSDK.Extensions.NETCore.Setup + Amazon.Extensions.Configuration.SystemsManager
            // packages are configured in Program.cs / Startup.cs)
            return _configuration.GetConnectionString("dbconnection");
        }

        /// <summary>
        /// Uploads the tour image to Amazon S3 and returns the stored file name (S3 object key).
        /// Replaces the original Server.MapPath("~/Tour_pics/") + FileUpload1.FileName local save.
        ///
        /// S3 object key format: Tour_pics/{fileName}
        /// Bucket is read from the AWS_S3_BUCKET_NAME environment variable.
        /// AWS region is read from the AWS_REGION environment variable (default: us-east-1).
        /// </summary>
        private string UploadToS3(IFormFile file)
        {
            string bucketName = Environment.GetEnvironmentVariable("AWS_S3_BUCKET_NAME")
                                ?? _configuration["AWS:S3BucketName"];

            if (string.IsNullOrEmpty(bucketName))
                throw new InvalidOperationException(
                    "S3 bucket name is not configured. Set the AWS_S3_BUCKET_NAME environment variable.");

            string regionName = Environment.GetEnvironmentVariable("AWS_REGION")
                                ?? _configuration["AWS:Region"]
                                ?? "us-east-1";

            RegionEndpoint region = RegionEndpoint.GetBySystemName(regionName);

            // S3 object key mirrors the original Tour_pics virtual directory
            string s3Key = $"Tour_pics/{file.FileName}";

            using (IAmazonS3 s3Client = new AmazonS3Client(region))
            using (TransferUtility transferUtility = new TransferUtility(s3Client))
            using (Stream fileStream = file.OpenReadStream())
            {
                TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = s3Key,
                    InputStream = fileStream,
                    ContentType = file.ContentType,
                    // Make the uploaded image publicly readable so it can be served via S3 URL
                    CannedACL = S3CannedACL.PublicRead
                };

                transferUtility.Upload(uploadRequest);
            }

            // Return only the file name (consistent with original @pic column value)
            return file.FileName;
        }

        public void OnGet()
        {
            // Initial page load — no action required.
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string picFileName = string.Empty;

            // Upload tour image to Amazon S3 (replaces Server.MapPath local file save)
            if (TourImage != null && TourImage.Length > 0)
            {
                picFileName = UploadToS3(TourImage);
            }

            // Use Dapper with RDS Proxy-compatible SqlConnection (connection pooling handled by RDS Proxy)
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
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
                    pic = picFileName
                });
            }

            StatusMessage = "ADD Successful";
            return Page();
        }
    }
}
