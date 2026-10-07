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
            // cz-dotnet-0055: Replaced Web.config XDT transform-based connection string with environment variable
            SqlConnection conn = new SqlConnection(System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
            conn.Open();
            string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";
            SqlCommand com = new SqlCommand(insertQuery, conn);
            
            com.Parameters.AddWithValue("@TOUR_NAME", tour_name.Text);
            com.Parameters.AddWithValue("@PLACE", place.Text);
            com.Parameters.AddWithValue("@DAYS", days.Text); 
            com.Parameters.AddWithValue("@PRICE", price.Text);
            com.Parameters.AddWithValue("@LOCATIONS", locations.Text);
            com.Parameters.AddWithValue("@TOUR_INFO", tour_info.Text);

            // cz-dotnet-1032: Replaced local filesystem write (FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/")))
            // with Amazon S3 upload via AWSSDK. Bucket name and AWS region are read from environment variables
            // (S3_BUCKET_NAME, AWS_REGION). Authentication uses IRSA (IAM Roles for Service Accounts) on EKS —
            // no credentials are embedded in the container image.
            string s3BucketName = System.Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
            string awsRegion = System.Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";
            string s3KeyPrefix = "Tour_pics/";
            string fileName = Path.GetFileName(FileUpload1.FileName);
            string s3ObjectKey = s3KeyPrefix + fileName;

            using (var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(awsRegion)))
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

            com.Parameters.AddWithValue("@pic", fileName);

            com.ExecuteNonQuery();
            Response.Write("ADD  Successful");
            //Response.Redirect("a.aspx");
            //Server.Transfer("a.aspx");
            conn.Close();
        }
    }
}
