<%--
    MIGRATION NOTE (cr-dotnet-0026): This Web Forms page has been migrated to ASP.NET Core Razor Pages.
    The Razor Pages equivalent is located at: Pages/UserLogin.cshtml

    Original Web Forms violations addressed:
      - Line 1: <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userlogin.aspx.cs"
                Inherits="Tour_Management.userlogin" %>
                → Replaced with @page / @model directives in Pages/UserLogin.cshtml (cr-dotnet-0026)
      - <asp:Label runat="server"> → replaced with standard HTML <label> elements
      - <asp:TextBox runat="server"> → replaced with standard HTML <input> elements
      - <asp:Button runat="server" OnClick="Btn_Submit"> → replaced with <input type="submit">
      - <form runat="server"> → replaced with standard HTML form in Pages/UserLogin.cshtml
      - runat="server" attributes → removed throughout
      - <head runat="server"> → replaced with standard <head>

    This file is retained for reference only. The active implementation is in Pages/UserLogin.cshtml.
--%>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>User Login - Migrated</title>
</head>
<body>
    <!--
        This page has been migrated to ASP.NET Core Razor Pages.
        See Pages/UserLogin.cshtml for the active implementation.

        Migration summary:
        - <%@ Page %> directive removed (Web Forms - cr-dotnet-0026)
        - <asp:Label>, <asp:TextBox>, <asp:Button> server controls replaced with HTML equivalents
        - runat="server" attributes removed
        - Btn_Submit event handler migrated to OnPost() in Pages/UserLogin.cshtml.cs
        - Btn_reg event handler migrated to OnPostRegister() in Pages/UserLogin.cshtml.cs
        - Connection string migrated to environment variable DB_CONNECTION_STRING
        - SQL injection vulnerability fixed with parameterized queries via Dapper
    -->
</body>
</html>
