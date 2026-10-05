using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace Tour_Management
{
    public partial class AddTour : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
       
        protected void Register_Click(object sender, EventArgs e)
        {
            string connectionString = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";
            SqlCommand com = new SqlCommand(insertQuery, conn);
            
            com.Parameters.AddWithValue("@TOUR_NAME", tour_name.Text);
            com.Parameters.AddWithValue("@PLACE", place.Text);
            com.Parameters.AddWithValue("@DAYS", days.Text); 
            com.Parameters.AddWithValue("@PRICE", price.Text);
            com.Parameters.AddWithValue("@LOCATIONS", locations.Text);
            com.Parameters.AddWithValue("@TOUR_INFO", tour_info.Text);

            // Upload file to Amazon S3 using IRSA (IAM Roles for Service Accounts) on EKS.
            // No credentials are embedded; authentication is handled via the pod's IAM role.
            string s3BucketName = System.Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
            string s3Region = System.Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";
            string s3KeyPrefix = System.Environment.GetEnvironmentVariable("S3_TOUR_PICS_PREFIX") ?? "Tour_pics/";

            string s3ObjectKey = s3KeyPrefix + FileUpload1.FileName;

            using (var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(s3Region)))
            using (var transferUtility = new TransferUtility(s3Client))
            using (Stream fileStream = FileUpload1.FileContent)
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = s3BucketName,
                    Key = s3ObjectKey,
                    InputStream = fileStream,
                    ContentType = FileUpload1.PostedFile.ContentType
                };
                transferUtility.Upload(uploadRequest);
            }

            com.Parameters.AddWithValue("@pic", FileUpload1.FileName);

            com.ExecuteNonQuery();
            Response.Write("ADD  Successful");
            //Response.Redirect("a.aspx");
            //Server.Transfer("a.aspx");
            conn.Close();
        }
    }
}
