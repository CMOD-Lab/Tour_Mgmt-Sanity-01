<%-- 
    cr-dotnet-1034: Synchronous GridView/SqlDataSource data binding replaced with
    async Task-based patterns using Entity Framework Core connected to Amazon RDS.
    Original: asp:GridView DataSourceID="SqlDataSource1" (synchronous, line 15 original)
    Original: asp:SqlDataSource SelectCommand="SELECT * FROM [booking]" (line 33 original)
    Replaced by: async OnGetAsync() in Pages/AllBooking.cshtml.cs using EF Core ToListAsync().

    MIGRATED: This Web Forms page has been migrated to ASP.NET Core Razor Pages.
    The equivalent Razor Page is located at Pages/AllBooking.cshtml.
    This file is retained for reference only and is no longer active.
    See Pages/AllBooking.cshtml and Pages/AllBooking.cshtml.cs for the migrated async EF Core implementation.
--%>
