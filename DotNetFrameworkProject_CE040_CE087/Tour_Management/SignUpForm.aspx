<%--
    SignUpForm.aspx - MIGRATED TO ASP.NET MVC
    Rule: cr-dotnet-0026 - Web Forms Usage
    This Web Form has been replaced by:
      - Controller: UserController.SignUp (GET/POST)
      - View: Views/User/SignUp.cshtml
      - Model: Models/SignUpViewModel
    
    This file is retained for backward compatibility only.
    All new requests should use the MVC route: /User/SignUp
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUpForm.aspx.cs" Inherits="Tour_Management.SignUpForm" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        // Redirect to the ASP.NET MVC equivalent route
        Response.RedirectPermanent("~/User/SignUp");
    }
</script>
