//------------------------------------------------------------------------------
// userlogin.aspx.designer.cs - MIGRATED TO ASP.NET MVC
// Rule: cr-dotnet-0026 - Web Forms Usage
//
// This auto-generated designer file previously declared Web Forms server controls
// (System.Web.UI.HtmlControls / System.Web.UI.WebControls).
// All server controls have been replaced by Razor HTML helpers in Views/User/Login.cshtml.
//
// Controls removed and their MVC equivalents:
//   - HtmlForm form1              → @using (Html.BeginForm("Login", "User", FormMethod.Post))
//   - Label Label1 (Email)        → @Html.LabelFor(m => m.Email)
//   - TextBox txtEmail            → @Html.TextBoxFor(m => m.Email, ...)
//   - Label Label2 (Password)     → @Html.LabelFor(m => m.Password)
//   - TextBox txtPassword         → @Html.PasswordFor(m => m.Password, ...)
//   - Button Register             → @Html.ActionLink("Sign Up", "SignUp", "User", ...)
//   - Button Button1 (Login)      → <button type="submit" ...>Login</button>
//------------------------------------------------------------------------------

namespace Tour_Management
{
    // Designer file intentionally cleared — all Web Forms server control declarations
    // have been removed as part of the migration to ASP.NET MVC Razor Views.
    // The partial class is no longer needed since userlogin.aspx.cs no longer
    // inherits from System.Web.UI.Page.
}
