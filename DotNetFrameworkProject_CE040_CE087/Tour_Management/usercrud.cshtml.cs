using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for UserCrud - migrated from ASP.NET Web Forms (usercrud.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    ///
    /// Cloud Readiness Fix (Rule: cr-dotnet-1034):
    /// Replaces synchronous asp:GridView data binding (AutoGenerateEditButton="True",
    /// DataSourceID="SqlDataSource1") and asp:SqlDataSource SelectCommand/UpdateCommand
    /// with async Task-based patterns using Entity Framework Core connected to Amazon RDS.
    /// This prevents thread pool exhaustion under load and enables efficient auto-scaling
    /// in cloud deployments.
    ///
    /// Key changes from original Web Forms implementation:
    ///   - Removed: asp:SqlDataSource synchronous SelectCommand/UpdateCommand
    ///   - Removed: asp:GridView synchronous DataBind() / AutoGenerateEditButton
    ///   - Added:   async OnGetAsync() using EF Core DbContext.UserInfos.ToListAsync()
    ///   - Added:   async OnPostUpdateAsync() using EF Core SaveChangesAsync()
    ///   - Added:   Amazon RDS connection via DB_CONNECTION_STRING environment variable
    ///
    /// Original SqlDataSource commands replaced:
    ///   SelectCommand: Select top (select COUNT(*) from UserInfo) * From UserInfo
    ///                  EXCEPT Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo
    ///   UpdateCommand: UPDATE [UserInfo] Set [Email]=@Email,[FirstName]=@FirstName,
    ///                  [LastName]=@LastName,[Gender]=@Gender,[Password]=@Password,
    ///                  [City]=@City Where [Email]=@Email
    /// </summary>
    public class UserCrudModel : PageModel
    {
        private readonly TourManagementDbContext _context;
        private readonly ILogger<UserCrudModel> _logger;

        public UserCrudModel(TourManagementDbContext context, ILogger<UserCrudModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<UserInfoViewModel> Users { get; private set; } = new List<UserInfoViewModel>();
        public string ErrorMessage { get; private set; }
        public string StatusMessage { get; private set; }
        public bool IsSuccess { get; private set; }
        public string EditingEmail { get; private set; }

        // Bound properties for the edit form
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string City { get; set; }

        /// <summary>
        /// Async GET handler - replaces synchronous asp:GridView data binding.
        /// Uses EF Core async query to prevent thread pool exhaustion under cloud load.
        /// Connects to Amazon RDS via DB_CONNECTION_STRING environment variable.
        /// Replaces original asp:SqlDataSource SelectCommand:
        ///   Select top (select COUNT(*) from UserInfo) * From UserInfo
        ///   EXCEPT Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadUsersAsync();
        }

        /// <summary>
        /// Handles POST Edit requests — sets the editing state for a user row.
        /// </summary>
        public async Task<IActionResult> OnPostEditAsync(string email)
        {
            EditingEmail = email;
            await LoadUsersAsync();
            return Page();
        }

        /// <summary>
        /// Handles POST CancelEdit requests — cancels the edit operation.
        /// </summary>
        public async Task<IActionResult> OnPostCancelEditAsync()
        {
            await LoadUsersAsync();
            return Page();
        }

        /// <summary>
        /// Async Update handler - replaces synchronous asp:SqlDataSource UpdateCommand
        /// and asp:GridView AutoGenerateEditButton="True" synchronous update.
        /// Uses EF Core async SaveChangesAsync() to prevent thread pool exhaustion.
        /// Replaces original UpdateCommand:
        ///   UPDATE [UserInfo] Set [Email]=@Email,[FirstName]=@FirstName,
        ///   [LastName]=@LastName,[Gender]=@Gender,[Password]=@Password,
        ///   [City]=@City Where [Email]=@Email
        /// </summary>
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            try
            {
                // Async EF Core update - prevents thread pool exhaustion under cloud load
                var user = await _context.UserInfos.FindAsync(Email);
                if (user != null)
                {
                    user.FirstName = FirstName;
                    user.LastName  = LastName;
                    user.Gender    = Gender;
                    user.Password  = Password;
                    user.City      = City;

                    await _context.SaveChangesAsync();
                    IsSuccess = true;
                    StatusMessage = "User updated successfully.";
                }
                else
                {
                    StatusMessage = $"User with email {Email} not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Email} in Amazon RDS", Email);
                StatusMessage = $"Update failed: {ex.Message}";
            }

            await LoadUsersAsync();
            return Page();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                // Async EF Core query - prevents thread pool exhaustion under load
                // Replaces synchronous asp:SqlDataSource SelectCommand:
                //   Select top (select COUNT(*) from UserInfo) * From UserInfo
                //   EXCEPT Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo
                var users = await _context.UserInfos
                    .AsNoTracking()
                    .ToListAsync();

                Users = new List<UserInfoViewModel>();
                foreach (var u in users)
                {
                    Users.Add(new UserInfoViewModel
                    {
                        Email     = u.Email,
                        FirstName = u.FirstName,
                        LastName  = u.LastName,
                        Gender    = u.Gender,
                        Password  = u.Password,
                        City      = u.City
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users from Amazon RDS");
                ErrorMessage = $"Unable to load users: {ex.Message}";
            }
        }
    }

    /// <summary>
    /// View model representing a single user record for the user CRUD page.
    /// </summary>
    public class UserInfoViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
    }
}
