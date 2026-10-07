// MIGRATED: This code-behind has been migrated to ASP.NET Core MVC.
// The equivalent functionality is now in Controllers/OrderController.cs.
// This file is retained for reference only and is no longer active.
//
// Original Web Forms code-behind for Order.aspx
// Replaced by: OrderController.Order (GET) and OrderController.Order (POST)
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
//     public partial class Order : System.Web.UI.Page   // <-- Web Forms base class (removed)
//     {
//         protected void Page_Load(object sender, EventArgs e) { }
//
//         protected void btn_click(object sender, EventArgs e)
//         {
//             // ORIGINAL (cloud-incompatible — cr-dotnet-0010):
//             // SqlConnection conn = new SqlConnection(
//             //     ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
//             //
//             // REPLACED BY (in OrderController.cs — cr-dotnet-0010):
//             // string connStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//             //     ?? AwsSsmHelper.GetParameter("/tour-management/db-connection-string");
//         }
//     }
// }
