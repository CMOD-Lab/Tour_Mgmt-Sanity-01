<%-- 
    MIGRATED TO ASP.NET CORE RAZOR PAGES
    This Web Forms page (DisplayTours.aspx) has been migrated to ASP.NET Core Razor Pages.
    The new implementation is located at: DisplayTours.cshtml / DisplayTours.cshtml.cs

    Migration performed as part of cloud readiness remediation (Rule: cr-dotnet-1034).
    The Web Forms Page directive, server controls (asp:GridView, asp:SqlDataSource,
    asp:BoundField, asp:TemplateField), and code-behind inheritance from
    System.Web.UI.Page have been replaced with a stateless Razor Page model
    following cloud-native patterns.

    The synchronous asp:GridView data binding (DataSourceID="SqlDataSource1") and
    asp:SqlDataSource SelectCommand (SELECT [TOUR_NAME], [pic], [PRICE], [DAYS],
    [LOCATIONS], [TOUR_ID] FROM [Tour]) have been replaced with async Task-based
    data retrieval using Entity Framework Core connected to Amazon RDS, preventing
    thread pool exhaustion under load and enabling efficient auto-scaling.

    Original synchronous controls replaced:
      Line 18: <asp:SqlDataSource ID="SqlDataSource1" ... SelectCommand="SELECT ... FROM [Tour]">
      Line 47: <asp:GridView ID="GridView1" ... DataSourceID="SqlDataSource1" ...>
--%>
