<%-- 
    MIGRATED: This Web Forms page has been migrated to ASP.NET Core Razor Pages.
    The equivalent Razor Page is located at: Pages/AllBooking.cshtml
    This file is retained for reference only and is no longer active.
    
    Migration: ASP.NET Web Forms -> ASP.NET Core Razor Pages (cr-dotnet-0026)
    The <%@ Page %> directive, runat="server" controls, asp:GridView, asp:SqlDataSource,
    and code-behind inheritance from System.Web.UI.Page have been replaced with a Razor Page model.

    cr-dotnet-1034: Synchronous GridView data binding replaced with async Task-based pattern.
    Original synchronous pattern (lines 15, 33 - now removed):
      Line 15: <asp:GridView ID="GridView1" runat="server" ... DataSourceID="SqlDataSource1" ...>
      Line 33: <asp:SqlDataSource ID="SqlDataSource1" ... SelectCommand="SELECT * FROM [booking]">
    
    Replaced with async Razor Page handler in Pages/AllBooking.cshtml.cs:
      public async Task OnGetAsync() -> uses await conn.QueryAsync<BookingViewModel>()
    Connected to Amazon RDS via Dapper for cloud-native async data access,
    preventing thread pool exhaustion and enabling efficient auto-scaling.
--%>
