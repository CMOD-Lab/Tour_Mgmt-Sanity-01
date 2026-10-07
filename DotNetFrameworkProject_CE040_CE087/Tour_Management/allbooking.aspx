<%-- 
    MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
    This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
    The original <%@ Page %> directive and all Web Forms markup below have been superseded
    by the Razor Page at Pages/AllBooking.cshtml and its PageModel in allbooking.aspx.cs.

    Original Web Forms patterns replaced:
    - <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="allbooking.aspx.cs" Inherits="Tour_Management.allbooking" %>
      Replaced by: @page / @model directives in Pages/AllBooking.cshtml
    - <%@ Register assembly="System.Web.DataVisualization..." %> — removed (Web Forms only)
    - <form id="form1" runat="server"> — replaced with standard HTML <form method="post">
    - <asp:GridView> server control — replaced with HTML <table> rendered via @foreach in Razor view
    - <asp:SqlDataSource> with ConnectionString="<%$ ConnectionStrings:dbconnection %>"
      Replaced by: Dapper query in AllBookingModel.OnGet() using RDS_CONNECTION_STRING env variable
    - <asp:BoundField> column definitions — replaced with explicit <th>/<td> in Razor view

    See: Pages/AllBooking.cshtml and allbooking.aspx.cs (AllBookingModel PageModel)
--%>
