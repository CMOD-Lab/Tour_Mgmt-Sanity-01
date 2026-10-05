// cr-dotnet-1034: Designer file updated — synchronous GridView and SqlDataSource
// control declarations removed. Data binding is now handled via async Entity Framework Core
// queries in the Razor Page model (Pages/TourCrud.cshtml.cs).
// The GridView and SqlDataSource Web Forms controls are no longer used.
//
// Original auto-generated declarations removed:
//   - protected global::System.Web.UI.WebControls.GridView GridView1;   (line 33 original)
//   - protected global::System.Web.UI.WebControls.SqlDataSource SqlDataSource1;
//   - protected global::System.Web.UI.HtmlControls.HtmlForm form1;
//
// These are replaced by the async EF Core PageModel in Pages/TourCrud.cshtml.cs.

namespace Tour_Management
{
    // cr-dotnet-1034: Partial class retained for compatibility; Web Forms GridView
    // and SqlDataSource fields removed — async EF Core data access is used instead.
    public partial class TourCrud
    {
        // GridView1 (System.Web.UI.WebControls.GridView) removed — replaced by
        // async EF Core query in TourCrudModel.OnGetAsync() (Pages/TourCrud.cshtml.cs).
        // SqlDataSource1 (System.Web.UI.WebControls.SqlDataSource) removed — replaced by
        // TourCrudDbContext (Entity Framework Core DbContext) in Pages/TourCrud.cshtml.cs.
    }
}
