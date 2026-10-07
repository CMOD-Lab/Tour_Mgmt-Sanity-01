using System;
using System.Web.Mvc;
using Tour_Management.Models;

namespace Tour_Management.Controllers
{
    /// <summary>
    /// ASP.NET Core MVC controller replacing AdminLogin2.aspx and AdminProfile.aspx Web Forms.
    /// Migrated from Web Forms (System.Web.UI.Page) to ASP.NET MVC Controller pattern.
    /// </summary>
    public class AdminController : Controller
    {
        // -----------------------------------------------------------------------
        // GET: /Admin/Login
        // Replaces: AdminLogin2.aspx initial render
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Login()
        {
            return View(new AdminLoginViewModel());
        }

        // -----------------------------------------------------------------------
        // POST: /Admin/Login
        // Replaces: AdminLogin2.aspx.cs Page_Load credential check + Response.Redirect
        // Original logic: if (password.Text == "admin" && name.Text == "admin@gmail.com")
        //                     Response.Redirect("AdminProfile.aspx");
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Preserve original business logic: validate hardcoded admin credentials.
            // In a cloud-native deployment these credentials should be stored in
            // AWS Secrets Manager and retrieved at runtime.
            if (model.Password == "admin" && model.Email == "admin@gmail.com")
            {
                // Replaces: Response.Redirect("AdminProfile.aspx") / Server.Transfer("AdminProfile.aspx")
                return RedirectToAction("Profile", "Admin");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        // -----------------------------------------------------------------------
        // GET: /Admin/Profile
        // Replaces: AdminProfile.aspx (static welcome page for admin)
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Profile()
        {
            return View();
        }

        // -----------------------------------------------------------------------
        // GET: /Admin/Logout
        // Replaces: href="AdminLogin2.aspx" logout link in AdminProfile.aspx navbar
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Logout()
        {
            // Clear any session state and redirect to login
            Session.Clear();
            return RedirectToAction("Login", "Admin");
        }
    }
}
