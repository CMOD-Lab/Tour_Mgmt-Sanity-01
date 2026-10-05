// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// This designer file previously declared synchronous GridView and SqlDataSource controls.
// All synchronous GridView data binding has been replaced with async Task-based Razor Page
// rendering using Entity Framework Core ToListAsync() connected to Amazon RDS.
// The active implementation is in Pages/UserCrud.cshtml and Pages/UserCrud.cshtml.cs
// using async Task OnGetAsync() with EF Core DbContext.

namespace Tour_Management
{
    // cr-dotnet-1034: This designer class previously declared synchronous GridView and SqlDataSource controls.
    // Line 33 (original): protected global::System.Web.UI.WebControls.GridView GridView1;
    // The GridView has been replaced with async Razor foreach table rendering in Pages/UserCrud.cshtml.
    // SqlDataSource synchronous data binding replaced with EF Core async ToListAsync() on Amazon RDS.
    public partial class usercrud
    {
        /// <summary>
        /// form1 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.HtmlControls.HtmlForm form1;

        // cr-dotnet-1034 (Line 33): Removed synchronous GridView1 control declaration.
        // Original: protected global::System.Web.UI.WebControls.GridView GridView1;
        // Replaced by: async Razor foreach table in Pages/UserCrud.cshtml
        // Data loaded via: async Task OnGetAsync() using EF Core ToListAsync() on Amazon RDS

        // cr-dotnet-1034: Removed synchronous SqlDataSource1 control declaration.
        // Original: protected global::System.Web.UI.WebControls.SqlDataSource SqlDataSource1;
        // Replaced by: Entity Framework Core UserCrudDbContext with async ToListAsync() and update
    }
}
