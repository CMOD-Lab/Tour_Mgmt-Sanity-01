using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tour_Management.Data;
using Tour_Management.Models;

namespace Tour_Management.Controllers
{
    /// <summary>
    /// ASP.NET MVC controller replacing SignUpForm.aspx, userlogin.aspx, and usercrud.aspx Web Forms.
    /// cr-dotnet-1034: Synchronous GridView data binding replaced with async Task-based
    /// patterns using Entity Framework 6 connected to Amazon RDS, preventing thread pool
    /// exhaustion under cloud load and enabling efficient auto-scaling.
    ///
    /// Migrated from Web Forms (System.Web.UI.Page) to ASP.NET MVC Controller pattern.
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// </summary>
    public class UserController : Controller
    {
        // -----------------------------------------------------------------------
        // GET: /User/SignUp
        // Replaces: SignUpForm.aspx Page_Load (initial render)
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        // -----------------------------------------------------------------------
        // POST: /User/SignUp
        // Replaces: SignUpForm.aspx.cs Register_Click event handler
        // Original logic: INSERT into UserInfo, then Response.Redirect("userlogin.aspx")
        // cr-dotnet-1034: Async EF Core insert replacing synchronous SqlDataSource InsertCommand.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // cr-dotnet-1034: Async EF Core insert replacing synchronous SqlDataSource InsertCommand.
            // SaveChangesAsync() releases the thread during Amazon RDS I/O.
            using (var db = new TourManagementDbContext())
            {
                var user = new UserInfoEntity
                {
                    Email       = model.Email,
                    FirstName   = model.FirstName,
                    LastName    = model.LastName,
                    Gender      = model.Gender,
                    Password    = model.Password,
                    DateOfBirth = model.DateOfBirth,
                    Street      = model.Street,
                    City        = model.City,
                    State       = model.State
                };

                db.UserInfos.Add(user);
                await db.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Registration Successful";
            // Replaces: Response.Redirect("userlogin.aspx")
            return RedirectToAction("Login", "User");
        }

        // -----------------------------------------------------------------------
        // GET: /User/Login
        // Replaces: userlogin.aspx Page_Load (initial render)
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Login()
        {
            return View(new UserLoginViewModel());
        }

        // -----------------------------------------------------------------------
        // POST: /User/Login
        // Replaces: userlogin.aspx.cs Btn_Submit event handler
        // Original logic: SELECT password from UserInfo, compare, then Response.Redirect("MainProfilePage.aspx")
        // cr-dotnet-1034: Async EF Core query replacing synchronous SqlDataSource SelectCommand.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // cr-dotnet-1034: Async EF Core query replacing synchronous SqlDataSource SelectCommand.
            // FirstOrDefaultAsync() releases the thread during Amazon RDS I/O.
            string password;
            using (var db = new TourManagementDbContext())
            {
                var user = await db.UserInfos
                    .Where(u => u.Email == model.Email && u.Password == model.Password)
                    .Select(u => u.Password)
                    .FirstOrDefaultAsync();

                password = user ?? string.Empty;
            }

            if (password == model.Password)
            {
                // Replaces: Response.Redirect("MainProfilePage.aspx")
                return RedirectToAction("MainProfilePage", "Home");
            }

            ModelState.AddModelError(string.Empty, "Password is not correct.");
            return View(model);
        }

        // -----------------------------------------------------------------------
        // GET: /User/Register  (redirect to SignUp for backward compatibility)
        // Replaces: Btn_reg event handler in userlogin.aspx.cs
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Register()
        {
            return RedirectToAction("SignUp", "User");
        }

        // -----------------------------------------------------------------------
        // GET: /User/UserCrud
        // Replaces: usercrud.aspx (GridView with SqlDataSource for user management)
        // cr-dotnet-1034: Synchronous GridView data binding (usercrud.aspx lines 11, 30)
        // replaced with async Task-based EF Core query on Amazon RDS.
        // Uses ToListAsync() to free the request thread during database I/O.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult> UserCrud()
        {
            List<UserInfoViewModel> users;

            // cr-dotnet-1034: Async EF Core query replacing synchronous SqlDataSource +
            // GridView data binding. ToListAsync() releases the thread during RDS I/O.
            // Preserve original SQL logic: select the last user record from UserInfo.
            using (var db = new TourManagementDbContext())
            {
                int totalCount = await db.UserInfos.CountAsync();
                int skipCount  = totalCount > 1 ? totalCount - 1 : 0;

                users = await db.UserInfos
                    .OrderBy(u => u.Email)
                    .Skip(skipCount)
                    .Take(1)
                    .Select(u => new UserInfoViewModel
                    {
                        Email     = u.Email,
                        FirstName = u.FirstName,
                        LastName  = u.LastName,
                        Gender    = u.Gender,
                        Password  = u.Password,
                        City      = u.City
                    })
                    .ToListAsync();
            }

            return View(users);
        }

        // -----------------------------------------------------------------------
        // POST: /User/UpdateUser
        // Replaces: usercrud.aspx SqlDataSource UpdateCommand
        // cr-dotnet-1034: Async EF Core update replacing synchronous SqlDataSource UpdateCommand.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUser(UserInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserCrud");
            }

            // cr-dotnet-1034: Async EF Core update replacing synchronous SqlDataSource UpdateCommand.
            // SaveChangesAsync() releases the thread during Amazon RDS I/O.
            using (var db = new TourManagementDbContext())
            {
                var user = await db.UserInfos.FindAsync(model.Email);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName  = model.LastName;
                    user.Gender    = model.Gender;
                    user.Password  = model.Password;
                    user.City      = model.City;
                    await db.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction("UserCrud");
        }
    }
}
