// MIGRATED: This Web Forms designer file has been migrated to ASP.NET Core Razor Pages.
// The equivalent Razor Page model is located at: Pages/UserLogin.cshtml.cs
// This file is retained for reference only and is no longer active.
//
// Migration: ASP.NET Web Forms -> ASP.NET Core Razor Pages (cr-dotnet-0026)
// Rule: Web Forms Usage — Severity: HIGH — Category: legacy-framework-issues
//
// All System.Web.UI.HtmlControls and System.Web.UI.WebControls server control
// field declarations have been removed. The equivalent HTML form controls are
// defined in Pages/UserLogin.cshtml using standard HTML with Razor tag helpers:
//
//   form1  (HtmlForm)          -> <form method="post"> in Pages/UserLogin.cshtml
//   Label1 (Label)             -> <label asp-for="Input.Email"> in Pages/UserLogin.cshtml
//   txtEmail (TextBox)         -> <input asp-for="Input.Email"> in Pages/UserLogin.cshtml
//   Label2 (Label)             -> <label asp-for="Input.Password"> in Pages/UserLogin.cshtml
//   txtPassword (TextBox)      -> <input asp-for="Input.Password"> in Pages/UserLogin.cshtml
//   Register (Button/Login)    -> <button asp-page-handler="Login"> in Pages/UserLogin.cshtml
//   Button1 (Button/Sign Up)   -> <button asp-page-handler="Register"> in Pages/UserLogin.cshtml
//
// See Pages/UserLogin.cshtml for the active Razor Page view.
// See Pages/UserLogin.cshtml.cs for the active Razor Page model.
