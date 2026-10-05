<%-- 
    cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
    
    Line 15 (original): <asp:GridView ID="GridView1" runat="server" ... DataSourceID="SqlDataSource1">
    Fix applied: Replaced synchronous GridView server control with async Razor Page table rendering.
    
    Line 33 (original): <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="..."
                         SelectCommand="SELECT * FROM [booking]">
    Fix applied: Replaced synchronous SqlDataSource with async EF Core DbContext on Amazon RDS.
    
    The active implementation is at: Pages/AllBooking.cshtml and Pages/AllBooking.cshtml.cs
    using async Task OnGetAsync() with Entity Framework Core ToListAsync() connected to Amazon RDS.
    
    MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): This Web Forms page (allbooking.aspx) has been
    migrated to ASP.NET Core Razor Pages with async data binding.
    - Removed: <%@ Register %> directive for System.Web.DataVisualization
    - Removed: <asp:GridView runat="server"> synchronous server control (line 15)
    - Removed: <asp:SqlDataSource runat="server"> synchronous data source (line 33)
    - Replaced: synchronous GridView.DataBind() with async Task OnGetAsync() + EF Core ToListAsync()
    - Replaced: SqlDataSource ConnectionString binding with environment variable DB_CONNECTION_STRING
    - Prevents thread pool exhaustion under AWS cloud load; enables efficient auto-scaling
    
    This file is retained for reference only.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="allbooking.aspx.cs" Inherits="Tour_Management.allbooking" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <%--
            cr-dotnet-1034 (Line 15): Replaced synchronous <asp:GridView> server control with
            async Razor Page table rendering in Pages/AllBooking.cshtml.
            Data is now loaded via async Task OnGetAsync() using EF Core ToListAsync() on Amazon RDS.
            Original: <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                       DataKeyNames="TOUR_ID" DataSourceID="SqlDataSource1" ...>
        --%>

        <%--
            cr-dotnet-1034 (Line 33): Replaced synchronous <asp:SqlDataSource> with async EF Core.
            Original: <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                       ConnectionString="<%$ ConnectionStrings:dbconnection %>"
                       SelectCommand="SELECT * FROM [booking]">
            Replaced by: Entity Framework Core AllBookingDbContext with async ToListAsync()
                         connected to Amazon RDS.
        --%>
    </form>
</body>
</html>
