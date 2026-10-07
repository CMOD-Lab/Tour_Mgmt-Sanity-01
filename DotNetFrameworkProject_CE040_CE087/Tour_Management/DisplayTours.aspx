<%-- 
    MIGRATED: This Web Forms page has been migrated to ASP.NET Core Razor Pages.
    The equivalent Razor Page is located at: Pages/DisplayTours.cshtml (see DisplayTours.aspx.cs)
    This file is retained for reference only and is no longer active.
    
    Migration: ASP.NET Web Forms -> ASP.NET Core Razor Pages (cr-dotnet-0026)
    The <%@ Page %> directive, runat="server" controls, asp:GridView, asp:SqlDataSource,
    and code-behind inheritance from System.Web.UI.Page have been replaced with a Razor Page model.

    cr-dotnet-1034: Synchronous GridView data binding (asp:GridView DataSourceID="SqlDataSource1"
    with synchronous SqlDataSource SelectCommand) replaced with async Task OnGetAsync() pattern
    in DisplayTours.aspx.cs using Dapper QueryAsync<TourItem>() connected to Amazon RDS,
    preventing thread pool exhaustion and enabling efficient auto-scaling in cloud deployments.

    Original synchronous pattern (lines 18, 47 - now removed):
      Line 18: <asp:SqlDataSource ... SelectCommand="SELECT [TOUR_NAME], [pic], [PRICE], [DAYS], [LOCATIONS], [TOUR_ID] FROM [Tour]">
      Line 47: <asp:GridView ID="GridView1" runat="server" ... DataSourceID="SqlDataSource1" ...>
    
    Replaced with async Razor Page model in DisplayTours.aspx.cs:
      public async Task OnGetAsync() { ... await conn.QueryAsync<TourItem>(...) ... }
--%>
