<%--
    userlogin.aspx - MIGRATED TO ASP.NET MVC
    Rule: cr-dotnet-0026 - Web Forms Usage
    This Web Form has been replaced by:
      - Controller: UserController.Login (GET/POST)
      - View: Views/User/Login.cshtml
      - Model: Models/UserLoginViewModel
    
    This file is retained for backward compatibility only.
    All new requests should use the MVC route: /User/Login
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userlogin.aspx.cs" Inherits="Tour_Management.userlogin" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        // Redirect to the ASP.NET MVC equivalent route
        Response.RedirectPermanent("~/User/Login");
    }
</script>
