<%--
    MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
    This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
    The original <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TourCrud.aspx.cs" Inherits="Tour_Management.TourCrud" %>
    directive and all Web Forms server controls (<asp:GridView>, <asp:SqlDataSource>,
    <asp:BoundField>, <asp:TemplateField>, runat="server" attributes, <form runat="server">)
    have been replaced by the equivalent ASP.NET Core Razor Page located at Pages/TourCrud.cshtml.

    Key changes:
    - Removed: <%@ Page %> directive (Web Forms page registration)
    - Removed: runat="server" attributes on HTML elements
    - Removed: <asp:GridView> data-bound server control — replaced with Razor @foreach table rendering
    - Removed: <asp:SqlDataSource> declarative data source — replaced with PageModel data access via Dapper
    - Removed: <asp:BoundField>, <asp:TemplateField> column definitions — replaced with HTML <table> columns
    - Removed: <form id="form1" runat="server"> Web Forms postback form
    - Replaced: DataSourceID / ConnectionString="<%$ ConnectionStrings:dbconnection %>" with environment-variable-aware GetConnectionString()
    - Replaced: System.Web.UI.Page base class with Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    - The new Razor Page is: Pages/TourCrud.cshtml + Pages/TourCrud.cshtml.cs
--%>
