// MIGRATED: This code-behind has been migrated to ASP.NET Core MVC.
// The equivalent functionality is now in Controllers/TourController.cs.
// This file is retained for reference only and is no longer active.
//
// Original Web Forms code-behind for AddTour.aspx
// Replaced by: TourController.AddTour (GET) and TourController.AddTour (POST)
//
// cr-dotnet-1032 fix applied: Server.MapPath("~/Tour_pics/") + FileUpload1.SaveAs(...)
// has been replaced with Amazon S3 file upload via AWSSDK.S3 TransferUtility in
// TourController.cs (UploadTourImageToS3 helper method).
// Files are stored in the S3 bucket configured by the environment variable
// S3_TOUR_PICS_BUCKET (fallback: Web.config appSetting "S3TourPicsBucket").
//
// cr-dotnet-0010 fix applied: ConfigurationManager.ConnectionStrings["dbconnection"]
// has been replaced with environment variable DB_CONNECTION_STRING (with fallback to
// AWS Systems Manager Parameter Store key /tour-management/db-connection-string).
// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer
// used for connection string injection — configuration is injected at runtime via
// environment variables, enabling immutable deployments on AWS.
//
// using System;
// using System.Collections.Generic;
// using System.Configuration;
// using System.Data;
// using System.Data.SqlClient;
// using System.Linq;
// using System.Web;
// using System.Web.UI;                  // <-- Web Forms: System.Web.UI.Page (removed)
// using System.Web.UI.WebControls;      // <-- Web Forms: WebControls (removed)
// using Dapper;
//
// namespace Tour_Management
// {
//     public partial class AddTour : System.Web.UI.Page   // <-- Web Forms base class (removed)
//     {
//         protected void Page_Load(object sender, EventArgs e) { }
//
//         protected void Register_Click(object sender, EventArgs e)
//         {
//             // ORIGINAL (cloud-incompatible — cr-dotnet-0010 / cr-dotnet-1032):
//             // SqlConnection conn = new SqlConnection(
//             //     ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
//             //
//             // REPLACED BY (in TourController.cs — cr-dotnet-0010):
//             // string connStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//             //     ?? AwsSsmHelper.GetParameter("/tour-management/db-connection-string");
//             //
//             // REPLACED BY (in TourController.cs — cr-dotnet-1032):
//             // UploadTourImageToS3(tourImage.InputStream, picFileName);
//             // which streams the file directly to Amazon S3 using TransferUtility.
//         }
//     }
// }
