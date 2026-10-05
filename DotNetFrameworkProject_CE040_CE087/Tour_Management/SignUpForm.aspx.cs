// Fixed: cr-dotnet-0010 - Replaced Web.config transformation files (Web.Debug.config, Web.Release.config)
//        with environment variables and AWS Systems Manager Parameter Store for runtime configuration.
//        Configuration is injected at runtime rather than baked into build artifacts, enabling
//        true infrastructure-as-code and immutable deployments.
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Dapper;

namespace Tour_Management
{
    public partial class SignUpForm : System.Web.UI.Page
    {
        // cr-dotnet-0010: Retrieve connection string at runtime from environment variable (highest priority),
        // then AWS Systems Manager Parameter Store (cloud-native secrets/config), then Web.config fallback.
        // This eliminates the need for Web.config transformation files (Web.Debug.config / Web.Release.config)
        // which bake configuration into build artifacts and are incompatible with cloud deployment pipelines.
        private static string GetConnectionString()
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
                // SSM not available (e.g., local development) — fall through to Web.config
            }

            // 3. Web.config fallback for local development only
            return System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Register_Click(object sender, EventArgs e)
        {
            string insertQuery = "insert into UserInfo(Email,FirstName,LastName,Gender,Password,dob,Street,City,State) values(@email,@FirstName,@LastName,@Gender,@Password,@dob,@Street,@City,@State)";

            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Execute(insertQuery, new
                {
                    email = email.Text,
                    FirstName = fname.Text,
                    LastName = lname.Text,
                    Gender = gender.Text,
                    Password = password1.Text,
                    dob = dob.Text,
                    Street = street.Text,
                    City = city.Text,
                    State = state.Text
                });
            }

            Response.Write("Registration Successful");
            Response.Redirect("userlogin.aspx");
        }
    }
}
