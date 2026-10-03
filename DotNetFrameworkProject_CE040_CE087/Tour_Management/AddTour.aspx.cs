using System;
using System.Collections.Generic;
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
            // cz-dotnet-0055: Replaced ConfigurationManager.ConnectionStrings Web.config transform with environment variable
            string connectionString = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("DB_CONNECTION_STRING environment variable is not set.");
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

            // cz-dotnet-1032: Replaced local filesystem write (FileUpload1.SaveAs) with Amazon S3 upload.
            // Authenticated via IAM Roles for Service Accounts (IRSA) on EKS — no credentials embedded.
            // S3 bucket and region are read from environment variables S3_BUCKET_NAME and AWS_REGION.
            string s3BucketName = System.Environment.GetEnvironmentVariable("S3_BUCKET_NAME")
                ?? throw new InvalidOperationException("S3_BUCKET_NAME environment variable is not set.");
            string awsRegion = System.Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";
            string s3KeyPrefix = "tour-pics/";
            string fileName = Path.GetFileName(FileUpload1.FileName);

            using (var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(awsRegion)))
            using (var transferUtility = new TransferUtility(s3Client))
            using (Stream fileStream = FileUpload1.FileContent)
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = s3BucketName,
                    Key = s3KeyPrefix + fileName,
                    InputStream = fileStream,
                    ContentType = FileUpload1.PostedFile.ContentType
                };
                transferUtility.Upload(uploadRequest);
            }

            com.Parameters.AddWithValue("@pic", fileName);

            com.ExecuteNonQuery();
            Response.Write("ADD  Successful");
            //Response.Redirect("a.aspx");
            //Server.Transfer("a.aspx");
            conn.Close();
        }
    }
}
