<%-- 
    MIGRATED: This ASP.NET Web Forms page has been migrated to ASP.NET Core Razor Pages.
    The equivalent Razor Page is located at: Pages/UserLogin.cshtml

    Migration: ASP.NET Web Forms -> ASP.NET Core Razor Pages (cr-dotnet-0026)
    Rule: Web Forms Usage — Severity: HIGH — Category: legacy-framework-issues

    Changes applied:
      - Removed: <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userlogin.aspx.cs"
                          Inherits="Tour_Management.userlogin" %>
                 -> Replaced with @page / @model directives in Pages/UserLogin.cshtml
      - Removed: <head runat="server"> -> Standard <head> in Pages/UserLogin.cshtml
      - Removed: <div ... runat="server"> -> Standard <div> in Pages/UserLogin.cshtml
      - Removed: <form id="form1" runat="server"> -> <form method="post"> in Pages/UserLogin.cshtml
      - Removed: <asp:Label> server controls -> <label asp-for="..."> tag helpers
      - Removed: <asp:TextBox> server controls -> <input asp-for="..."> tag helpers
      - Removed: <asp:Button OnClick="Btn_Submit"> -> <button asp-page-handler="Login">
      - Removed: <asp:Button OnClick="Btn_reg">    -> <button asp-page-handler="Register">

    See Pages/UserLogin.cshtml for the active Razor Page view.
    See Pages/UserLogin.cshtml.cs for the active Razor Page model.
--%>
