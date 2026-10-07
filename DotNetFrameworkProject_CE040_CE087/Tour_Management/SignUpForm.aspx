<%--
    MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
    This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
    The original <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUpForm.aspx.cs" Inherits="Tour_Management.SignUpForm" %>
    directive and all Web Forms server controls (<asp:TextBox>, <asp:Label>, <asp:Button>,
    <asp:DropDownList>, runat="server" attributes, <form runat="server">) have been replaced
    by the equivalent ASP.NET Core Razor Page located at Pages/SignUpForm.cshtml.

    Key changes:
    - Removed: <%@ Page %> directive (Web Forms page registration)
    - Removed: runat="server" attributes on HTML elements
    - Removed: <asp:Label>, <asp:TextBox>, <asp:Button>, <asp:DropDownList> server controls
    - Removed: <form runat="server"> Web Forms postback form
    - Removed: OnClick="Register_Click" postback event handler
    - Replaced: Web Forms server controls with standard HTML form elements bound via Razor tag helpers
    - Replaced: Response.Write / Response.Redirect with Razor @Model.StatusMessage and RedirectToPage()
    - Replaced: System.Web.UI.Page base class with Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    - The new Razor Page is: Pages/SignUpForm.cshtml + Pages/SignUpForm.cshtml.cs
--%>
