<%--
    usercrud.aspx - cr-dotnet-1034: Synchronous Data Binding in GridView Controls
    The synchronous asp:GridView (line 11) and asp:SqlDataSource (line 30) controls
    have been replaced with async Task-based data access using Entity Framework Core
    connected to Amazon RDS, preventing thread pool exhaustion under cloud load.

    This Web Form has been replaced by:
      - Controller: UserController.UserCrudAsync (GET) - async EF Core data access
      - View: Views/User/UserCrud.cshtml - Razor table replacing synchronous GridView
      - Model: Models/UserInfoViewModel

    Original synchronous patterns removed:
      - asp:GridView (line 11): synchronous data binding → replaced by async Razor table
      - asp:SqlDataSource (line 30): synchronous DB binding → replaced by async EF Core query

    This file is retained for backward compatibility only.
    All new requests should use the MVC route: /User/UserCrud
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="usercrud.aspx.cs" Inherits="Tour_Management.usercrud" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        // Redirect to the ASP.NET MVC equivalent route with async EF Core data binding
        Response.RedirectPermanent("~/User/UserCrud");
    }
</script>
