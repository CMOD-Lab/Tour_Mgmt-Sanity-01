using System.Web.Mvc;

namespace Tour_Management.Controllers
{
    /// <summary>
    /// ASP.NET Core MVC controller replacing MainProfilePage.aspx Web Form.
    /// Migrated from Web Forms (System.Web.UI.Page) to ASP.NET MVC Controller pattern.
    /// </summary>
    public class HomeController : Controller
    {
        // -----------------------------------------------------------------------
        // GET: /Home/MainProfilePage
        // Replaces: MainProfilePage.aspx (user home/welcome page with navigation)
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult MainProfilePage()
        {
            return View();
        }

        // -----------------------------------------------------------------------
        // GET: /Home/Index  (default route)
        // -----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("MainProfilePage");
        }
    }
}
