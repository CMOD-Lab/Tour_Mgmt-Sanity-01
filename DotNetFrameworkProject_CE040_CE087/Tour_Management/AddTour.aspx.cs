// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.RazorPages (ASP.NET Core MVC/Razor Pages)
// Fixed: cr-dotnet-1032 - Replaced Server.MapPath() local file access with Amazon S3 file operations
// Added: AWSSDK.S3 for cloud-native stateless file storage on AWS
// Fixed: cr-dotnet-0010 - Replaced Web.config transformation files (Web.Debug.config, Web.Release.config)
//        with environment variables and AWS Systems Manager Parameter Store for runtime configuration.
//        Configuration is injected at runtime rather than baked into build artifacts.
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;

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
        [Required]
        public string TourName { get; set; }

        [BindProperty]
        [Required]
        public string Place { get; set; }

        [BindProperty]
        [Required]
        public string Days { get; set; }

        [BindProperty]
        [Required]
        public string Locations { get; set; }

        [BindProperty]
        [Required]
        [MaxLength(250, ErrorMessage = "Characters less than 250")]
        public string TourInfo { get; set; }

        [BindProperty]
        [Required]
        public string Price { get; set; }

        public string StatusMessage { get; set; }

        // cr-dotnet-0010: Retrieve connection string at runtime from environment variable (highest priority),
        // then AWS Systems Manager Parameter Store (cloud-native secrets/config), then appsettings.json fallback.
        // This eliminates the need for Web.config transformation files (Web.Debug.config / Web.Release.config)
        // which bake configuration into build artifacts and are incompatible with cloud deployment pipelines.
        private string GetConnectionString()
        {
            // 1. Environment variable — injected by AWS ECS task definition, Elastic Beanstalk, or Lambda
            var envValue = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envValue))
                return envValue;

            // 2. AWS Systems Manager Parameter Store — runtime-configurable, no rebuild required
            //    Parameter path: /tour-mgmt/DB_CONNECTION_STRING
            //    IAM role on the compute resource must have ssm:GetParameter permission.
            try
            {
                using (var ssmClient = new AmazonSimpleSystemsManagementClient())
                {
                    var request = new GetParameterRequest
                    {
                        Name = "/tour-mgmt/DB_CONNECTION_STRING",
                        WithDecryption = true
                    };
                    var response = ssmClient.GetParameterAsync(request).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(response?.Parameter?.Value))
                        return response.Parameter.Value;
                }
            }
            catch
            {
                // SSM not available (e.g., local development) — fall through to appsettings.json
            }

            // 3. appsettings.json fallback for local development only
            return _configuration.GetConnectionString("dbconnection");
        }

        // cr-dotnet-0010: Retrieve S3 bucket name from environment variable or AWS SSM Parameter Store.
        // Eliminates Web.config transformation dependency for environment-specific S3 configuration.
        private string GetS3BucketName()
        {
            var envValue = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
            if (!string.IsNullOrEmpty(envValue))
                return envValue;

            try
            {
                using (var ssmClient = new AmazonSimpleSystemsManagementClient())
                {
                    var request = new GetParameterRequest { Name = "/tour-mgmt/S3_BUCKET_NAME" };
                    var response = ssmClient.GetParameterAsync(request).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(response?.Parameter?.Value))
                        return response.Parameter.Value;
                }
            }
            catch { /* fall through */ }

            return _configuration["AWS:S3BucketName"] ?? "tour-management-uploads";
        }

        // cr-dotnet-0010: Retrieve AWS region from environment variable or AWS SSM Parameter Store.
        // Eliminates Web.config transformation dependency for environment-specific region configuration.
        private string GetAwsRegion()
        {
            var envValue = Environment.GetEnvironmentVariable("AWS_REGION");
            if (!string.IsNullOrEmpty(envValue))
                return envValue;

            try
            {
                using (var ssmClient = new AmazonSimpleSystemsManagementClient())
                {
                    var request = new GetParameterRequest { Name = "/tour-mgmt/AWS_REGION" };
                    var response = ssmClient.GetParameterAsync(request).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(response?.Parameter?.Value))
                        return response.Parameter.Value;
                }
            }
            catch { /* fall through */ }

            return _configuration["AWS:Region"] ?? "us-east-1";
        }

        public void OnGet()
        {
            // Page load handler - no initialization required
        }

        // cr-dotnet-1032: Replaced Server.MapPath("~/Tour_pics/") local filesystem upload
        // with Amazon S3 PutObject upload for stateless, scalable cloud file storage.
        // Files are stored in S3 under the "Tour_pics/" prefix in the configured bucket.
        private async Task<string> UploadFileToS3Async(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            string bucketName = GetS3BucketName();
            string awsRegion = GetAwsRegion();
            string s3Key = $"Tour_pics/{file.FileName}";

            // IAM role-based credentials are used automatically when running on AWS (EC2/ECS/Lambda).
            // For local development, configure AWS credentials via environment variables or ~/.aws/credentials.
            var regionEndpoint = RegionEndpoint.GetBySystemName(awsRegion);
            using (var s3Client = new AmazonS3Client(regionEndpoint))
            using (var transferUtility = new TransferUtility(s3Client))
            using (var stream = file.OpenReadStream())
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = s3Key,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    // Make the uploaded file publicly readable so it can be served as a tour image
                    CannedACL = S3CannedACL.PublicRead
                };

                await transferUtility.UploadAsync(uploadRequest);
            }

            return file.FileName;
        }

        public IActionResult OnPost(IFormFile TourImage)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";

            // cr-dotnet-1032: Upload tour image to Amazon S3 instead of local filesystem via Server.MapPath
            string fileName = string.Empty;
            if (TourImage != null && TourImage.Length > 0)
            {
                // Upload to S3 asynchronously; GetAwaiter().GetResult() bridges sync OnPost with async upload
                fileName = UploadFileToS3Async(TourImage).GetAwaiter().GetResult();
            }

            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
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
