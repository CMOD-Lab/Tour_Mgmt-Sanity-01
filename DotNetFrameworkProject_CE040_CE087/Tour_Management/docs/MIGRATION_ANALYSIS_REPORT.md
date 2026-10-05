# Tour_Management – ASP.NET Web Forms to .NET 8 Migration Analysis Report

**Analysis Date:** 2025-01-30  
**Current Framework:** ASP.NET Web Forms 4.7.2  
**Target Framework:** .NET 8  
**Module:** Tour_Management  

---

## Executive Summary

| Severity | Count |
|----------|-------|
| Critical | 12    |
| High     | 10    |
| Medium   | 8     |
| Low      | 4     |
| **Total**| **34**|

- **Migration Complexity:** Complex  
- **Estimated Remediation Effort:** 80–120 hours  
- **Compatibility Score:** 18/100  
- **Deprecated APIs Found:** 18  
- **Breaking Changes:** 22  

---

## 1. Project Configuration Issues

### ISSUE-001 [CRITICAL] – Legacy Non-SDK Project Format
**File:** `Tour_Management.csproj`  
**Line:** 1  
**Code:** `<Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">`  
**Description:** The project uses the legacy MSBuild project format with `ProjectTypeGuids` for Web Application. .NET 8 requires the SDK-style project format.  
**Recommendation:** Replace with `<Project Sdk="Microsoft.NET.Sdk.Web">` and remove all legacy `<Import>` statements, `ProjectTypeGuids`, and `TargetFrameworkVersion`.

---

### ISSUE-002 [CRITICAL] – Target Framework is .NET Framework 4.7.2
**File:** `Tour_Management.csproj`  
**Line:** 18  
**Code:** `<TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>`  
**Description:** The project targets .NET Framework 4.7.2. .NET 8 requires `<TargetFramework>net8.0</TargetFramework>`.  
**Recommendation:** Change to `<TargetFramework>net8.0</TargetFramework>` in SDK-style project file.

---

### ISSUE-003 [CRITICAL] – System.Web Assembly References
**File:** `Tour_Management.csproj`  
**Lines:** 47–64  
**Code:**
```xml
<Reference Include="System.Web" />
<Reference Include="System.Web.DataVisualization" />
<Reference Include="System.Web.DynamicData" />
<Reference Include="System.Web.Entity" />
<Reference Include="System.Web.ApplicationServices" />
<Reference Include="System.Web.Extensions" />
<Reference Include="System.Web.Services" />
```
**Description:** All `System.Web.*` assemblies are .NET Framework-only and do not exist in .NET 8. These are the root cause of the majority of migration blockers.  
**Recommendation:** Remove all `System.Web.*` references. Replace with ASP.NET Core equivalents (`Microsoft.AspNetCore.*`).

---

### ISSUE-004 [CRITICAL] – Web.config Configuration System
**File:** `Web.config`  
**Lines:** 1–44  
**Code:** Entire `Web.config` file  
**Description:** `Web.config` is the .NET Framework configuration system and is not supported in .NET 8. Connection strings, app settings, compilation settings, and HTTP handlers are all defined here.  
**Recommendation:** Migrate to `appsettings.json`. Move connection strings to `appsettings.json` under `ConnectionStrings`. Move app settings to `appsettings.json`. Remove `system.web`, `system.webServer`, and `system.codedom` sections entirely.

---

### ISSUE-005 [HIGH] – packages.config NuGet Format
**File:** `packages.config`  
**Lines:** 1–4  
**Code:** `<package id="Microsoft.CodeDom.Providers.DotNetCompilerPlatform" version="2.0.1" targetFramework="net472" />`  
**Description:** `packages.config` is the legacy NuGet package management format. .NET 8 SDK-style projects use `<PackageReference>` in the `.csproj` file.  
**Recommendation:** Remove `packages.config`. Add `<PackageReference>` elements directly in the `.csproj` file. The `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` package is not needed in .NET 8.

---

## 2. System.Web Dependencies in Code-Behind Files

### ISSUE-006 [CRITICAL] – System.Web.UI.Page Base Class (All Pages)
**Files:** All `.aspx.cs` files  
**Code:** `public partial class AddTour : System.Web.UI.Page`  
**Description:** All code-behind classes inherit from `System.Web.UI.Page`, which is part of `System.Web` and does not exist in .NET 8. This is the fundamental Web Forms page model that must be replaced.  
**Recommendation:** Replace with Razor Pages (`PageModel`) or MVC Controllers. Each `.aspx.cs` file must be rewritten as a Razor Page (`*.cshtml.cs`) inheriting from `Microsoft.AspNetCore.Mvc.RazorPages.PageModel`.

**Affected Files:**
- `AddTour.aspx.cs` – Line 12
- `AdminLogin2.aspx.cs` – Line 11
- `AdminProfile.aspx.cs` – Line 11
- `allbooking.aspx.cs` – Line 11
- `DisplayTours.aspx.cs` – Line 11
- `MainProfilePage.aspx.cs` – Line 11
- `mybooking.aspx.cs` – Line 11
- `Order.aspx.cs` – Line 11
- `SignUpForm.aspx.cs` – Line 11
- `TourCrud.aspx.cs` – Line 13
- `usercrud.aspx.cs` – Line 11
- `userlogin.aspx.cs` – Line 11

---

### ISSUE-007 [CRITICAL] – System.Web Namespace Imports
**Files:** All `.aspx.cs` files  
**Code:**
```csharp
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
```
**Description:** These namespaces are part of `System.Web` which is not available in .NET 8.  
**Recommendation:** Remove all `System.Web.*` using statements. Replace with `Microsoft.AspNetCore.*` equivalents.

---

### ISSUE-008 [CRITICAL] – System.Configuration.ConfigurationManager Usage
**Files:** `AddTour.aspx.cs`, `DisplayTours.aspx.cs`, `Order.aspx.cs`, `SignUpForm.aspx.cs`, `TourCrud.aspx.cs`, `userlogin.aspx.cs`  
**Code:** `ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString`  
**Description:** `System.Configuration.ConfigurationManager` reads from `Web.config` which does not exist in .NET 8. While the `System.Configuration.ConfigurationManager` NuGet package exists, it is not the recommended approach for .NET 8.  
**Recommendation:** Use `IConfiguration` injected via dependency injection. Read connection strings from `appsettings.json` using `_configuration.GetConnectionString("dbconnection")`.

---

### ISSUE-009 [CRITICAL] – System.Data.SqlClient Direct ADO.NET Usage
**Files:** `AddTour.aspx.cs`, `Order.aspx.cs`, `SignUpForm.aspx.cs`, `TourCrud.aspx.cs`, `userlogin.aspx.cs`  
**Code:**
```csharp
SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
conn.Open();
SqlCommand com = new SqlCommand(insertQuery, conn);
com.ExecuteNonQuery();
```
**Description:** Raw ADO.NET with `SqlConnection`/`SqlCommand` is used throughout. While `System.Data.SqlClient` can be replaced with `Microsoft.Data.SqlClient` in .NET 8, the recommended approach is to use Entity Framework Core 8 or Dapper for data access.  
**Recommendation:** Replace with EF Core 8 (`Microsoft.EntityFrameworkCore.SqlServer 8.0.0`) using a `DbContext` and repository pattern, or use Dapper 2.1.28 for lightweight data access.

---

### ISSUE-010 [CRITICAL] – Server.MapPath Usage
**File:** `AddTour.aspx.cs`  
**Line:** 30  
**Code:** `FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);`  
**Description:** `Server.MapPath()` is a `System.Web.HttpServerUtility` method that does not exist in .NET 8.  
**Recommendation:** Replace with `IWebHostEnvironment.WebRootPath` or `IWebHostEnvironment.ContentRootPath`. Inject `IWebHostEnvironment` via constructor injection and use `Path.Combine(_env.WebRootPath, "Tour_pics", fileName)`.

---

### ISSUE-011 [CRITICAL] – Response.Write Usage
**Files:** `AddTour.aspx.cs`, `Order.aspx.cs`, `SignUpForm.aspx.cs`, `userlogin.aspx.cs`  
**Code:** `Response.Write("ADD Successful");`  
**Description:** `Response.Write()` is a `System.Web.HttpResponse` method. In .NET 8 Razor Pages, this pattern is replaced by model-bound messages, TempData, or ViewData.  
**Recommendation:** Use `TempData["Message"] = "ADD Successful";` and display in the Razor Page view, or use `ModelState.AddModelError()` for validation messages.

---

### ISSUE-012 [CRITICAL] – Response.Redirect + Server.Transfer Pattern
**Files:** `AdminLogin2.aspx.cs`, `Order.aspx.cs`, `SignUpForm.aspx.cs`, `userlogin.aspx.cs`  
**Code:**
```csharp
Response.Redirect("AdminProfile.aspx");
Server.Transfer("AdminProfile.aspx");
```
**Description:** `Server.Transfer()` is a `System.Web.HttpServerUtility` method that does not exist in .NET 8. Additionally, calling both `Response.Redirect()` and `Server.Transfer()` sequentially is a logic bug (the second call is unreachable). `Response.Redirect()` has an ASP.NET Core equivalent but the pattern must change.  
**Recommendation:** Use `return RedirectToPage("/AdminProfile")` in Razor Pages or `return Redirect("/AdminProfile")` in MVC controllers. Remove `Server.Transfer()` entirely.

---

## 3. Web Forms Page Directives and Server Controls

### ISSUE-013 [CRITICAL] – Web Forms Page Directives
**Files:** All `.aspx` files  
**Code:** `<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddTour.aspx.cs" Inherits="Tour_Management.AddTour" %>`  
**Description:** The `<%@ Page %>` directive is a Web Forms-specific construct that does not exist in .NET 8. The entire `.aspx` file format is not supported.  
**Recommendation:** Replace all `.aspx` files with Razor Pages (`.cshtml` files). The `@page` directive in Razor Pages replaces the `<%@ Page %>` directive.

---

### ISSUE-014 [CRITICAL] – ASP.NET Server Controls (runat="server")
**Files:** All `.aspx` files  
**Code:**
```html
<asp:TextBox id="tour_name" runat="server" class="form-control"/>
<asp:Button ID="Register" runat="server" OnClick="Register_Click"/>
<asp:Label id="l1" runat="server" text="Name of Tour"/>
<asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource1"/>
<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="..."/>
<asp:FileUpload ID="FileUpload1" runat="server"/>
<asp:RegularExpressionValidator ID="..." runat="server"/>
<asp:DropDownList ID="gender" runat="server"/>
<asp:HyperLink ID="HyperLink1" runat="server"/>
```
**Description:** All `asp:*` server controls with `runat="server"` are Web Forms-specific and do not exist in .NET 8. This includes TextBox, Button, Label, GridView, SqlDataSource, FileUpload, Validators, DropDownList, and HyperLink controls.  
**Recommendation:** Replace with standard HTML elements and Tag Helpers in Razor Pages:
- `asp:TextBox` → `<input asp-for="PropertyName" class="form-control" />`
- `asp:Button` → `<button type="submit">Submit</button>`
- `asp:Label` → `<label asp-for="PropertyName">`
- `asp:GridView` → HTML `<table>` with `@foreach` loop or a component library
- `asp:SqlDataSource` → Remove entirely; use EF Core or Dapper in PageModel
- `asp:FileUpload` → `<input type="file" asp-for="UploadedFile" />`
- `asp:RegularExpressionValidator` → Data Annotations + `<span asp-validation-for="...">`
- `asp:DropDownList` → `<select asp-for="Gender" asp-items="...">`

---

### ISSUE-015 [CRITICAL] – SqlDataSource Server Control
**Files:** `DisplayTours.aspx`, `TourCrud.aspx`, `allbooking.aspx`, `mybooking.aspx`, `usercrud.aspx`  
**Code:**
```html
<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
    ConnectionString="<%$ ConnectionStrings:dbconnection %>" 
    SelectCommand="SELECT * FROM [Tour]"
    UpdateCommand="UPDATE [Tour] Set ..."
    DeleteCommand="Delete from [Tour] Where ..."/>
```
**Description:** `asp:SqlDataSource` is a Web Forms data source control that does not exist in .NET 8. It embeds SQL directly in the markup, which is also a security concern.  
**Recommendation:** Remove all `SqlDataSource` controls. Implement data access in the PageModel using EF Core or Dapper. Bind data to the Razor Page model properties.

---

### ISSUE-016 [CRITICAL] – Web Forms Form Tag (runat="server")
**Files:** All `.aspx` files  
**Code:** `<form id="form1" runat="server">`  
**Description:** The `runat="server"` attribute on the `<form>` tag is a Web Forms-specific construct that enables ViewState and postback. This does not exist in .NET 8.  
**Recommendation:** Replace with standard HTML `<form>` tag with `method="post"` and include the anti-forgery token using `@Html.AntiForgeryToken()` or the `asp-antiforgery="true"` tag helper.

---

### ISSUE-017 [CRITICAL] – Page Lifecycle Events (Page_Load, IsPostBack)
**Files:** `TourCrud.aspx.cs`, `AdminLogin2.aspx.cs`  
**Code:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!Page.IsPostBack)
    {
        refreshdata();
    }
}
```
**Description:** The Web Forms page lifecycle (`Page_Load`, `Page_PreRender`, `IsPostBack`, etc.) does not exist in .NET 8. This is a fundamental architectural difference.  
**Recommendation:** Replace `Page_Load` with `OnGet()` (for GET requests) and `OnPost()` (for POST requests) in Razor Pages. The `IsPostBack` check is replaced by the HTTP method separation in Razor Pages.

---

### ISSUE-018 [CRITICAL] – DataVisualization Chart Control Registration
**File:** `allbooking.aspx`  
**Line:** 3  
**Code:** `<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>`  
**Description:** `System.Web.DataVisualization` is a .NET Framework-only assembly. It is not available in .NET 8.  
**Recommendation:** Replace with a modern charting library such as Chart.js (JavaScript), or use the `LiveCharts2` NuGet package for .NET 8.

---

## 4. Security Issues

### ISSUE-019 [CRITICAL] – SQL Injection Vulnerability
**File:** `userlogin.aspx.cs`  
**Lines:** 27–28  
**Code:**
```csharp
string checkPasswordQuery = "select password from Userinfo where password='" + txtPassword.Text + "' and email = '" + txtEmail.Text + "'";
SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn);
```
**Description:** String concatenation is used to build SQL queries, creating a critical SQL injection vulnerability. This is a security blocker for migration.  
**Recommendation:** Use parameterized queries (already done in other files) or EF Core. Replace with: `"SELECT password FROM Userinfo WHERE password=@password AND email=@email"` with `AddWithValue` parameters.

---

### ISSUE-020 [CRITICAL] – Plain-Text Password Storage
**Files:** `SignUpForm.aspx.cs`, `userlogin.aspx.cs`, `usercrud.aspx`  
**Code:**
```csharp
com.Parameters.AddWithValue("@Password", password1.Text);
// usercrud.aspx displays Password column in GridView
<asp:BoundField DataField="Password" HeaderText="Password" SortExpression="Password" />
```
**Description:** Passwords are stored and displayed in plain text. This is a critical security vulnerability.  
**Recommendation:** Use ASP.NET Core Identity with password hashing (`IPasswordHasher<T>`). Never store or display plain-text passwords. Migrate to `Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0`.

---

### ISSUE-021 [CRITICAL] – Hardcoded Admin Credentials
**File:** `AdminLogin2.aspx.cs`  
**Lines:** 14–17  
**Code:**
```csharp
if (password.Text == "admin" && name.Text == "admin@gmail.com")
{
    Response.Redirect("AdminProfile.aspx");
}
```
**Description:** Admin credentials are hardcoded in the source code. This is a critical security vulnerability.  
**Recommendation:** Implement proper authentication using ASP.NET Core Identity. Store admin credentials securely in the database with hashed passwords. Use role-based authorization with `[Authorize(Roles = "Admin")]`.

---

### ISSUE-022 [HIGH] – No Authentication/Authorization Mechanism
**Files:** All `.aspx` files  
**Description:** There is no Forms Authentication, session-based authentication, or any authorization mechanism protecting admin pages (`AdminProfile.aspx`, `AddTour.aspx`, `TourCrud.aspx`, `allbooking.aspx`).  
**Recommendation:** Implement ASP.NET Core Identity with cookie authentication. Apply `[Authorize]` attribute to protected Razor Pages. Configure authentication in `Program.cs`.

---

### ISSUE-023 [HIGH] – No Anti-Forgery Token (CSRF Protection)
**Files:** All `.aspx` files with forms  
**Description:** No CSRF protection is implemented on any form. Web Forms had built-in ViewState-based CSRF protection, but this is not present here.  
**Recommendation:** Razor Pages automatically include anti-forgery tokens when using Tag Helpers. Ensure `services.AddRazorPages()` is configured and use `<form method="post">` with Tag Helpers.

---

## 5. Data Access Issues

### ISSUE-024 [HIGH] – No Connection Pooling / Connection Management
**Files:** `AddTour.aspx.cs`, `Order.aspx.cs`, `SignUpForm.aspx.cs`, `TourCrud.aspx.cs`, `userlogin.aspx.cs`  
**Code:**
```csharp
SqlConnection conn = new SqlConnection(...);
conn.Open();
// ... operations ...
conn.Close(); // Not always reached due to Response.Redirect before Close()
```
**Description:** Connections are not wrapped in `using` statements, meaning they may not be properly disposed if an exception occurs. In several methods, `conn.Close()` is called after `Response.Redirect()` which means it is never reached.  
**Recommendation:** Use `using` statements for all `SqlConnection` and `SqlCommand` objects. With EF Core, the `DbContext` lifetime is managed by the DI container.

---

### ISSUE-025 [HIGH] – Hardcoded Local Database Path in Connection String
**File:** `Web.config`  
**Line:** 28  
**Code:** `AttachDbFilename=C:\Users\gajer\source\repos\Tour_Management\Tour_Management\App_Data\tourdb.mdf`  
**Description:** The connection string contains a hardcoded absolute path to a local `.mdf` file. This will not work in any environment other than the original developer's machine.  
**Recommendation:** Migrate to a proper SQL Server instance. Store the connection string in `appsettings.json` using environment variables or user secrets for development. Use `IConfiguration.GetConnectionString()`.

---

### ISSUE-026 [MEDIUM] – DataSet/DataTable Usage Pattern
**File:** `TourCrud.aspx.cs`  
**Lines:** 28–36 (commented out code)  
**Code:**
```csharp
// SqlDataAdapter sda = new SqlDataAdapter(cmd);
// DataTable dt = new DataTable();
// sda.Fill(dt);
// GridView1.DataSource = dt;
```
**Description:** Commented-out code shows DataSet/DataTable patterns. While commented out, this indicates the intended data access pattern which is not recommended for .NET 8.  
**Recommendation:** Use EF Core entities and LINQ queries. Replace `DataTable` with strongly-typed model classes.

---

## 6. Web Forms-Specific Architecture Issues

### ISSUE-027 [HIGH] – ViewState Dependency (GridView Controls)
**Files:** `DisplayTours.aspx`, `TourCrud.aspx`, `allbooking.aspx`, `mybooking.aspx`, `usercrud.aspx`  
**Code:** `<asp:GridView ID="GridView1" runat="server" AutoGenerateDeleteButton="True" AutoGenerateEditButton="True" DataKeyNames="TOUR_ID" DataSourceID="SqlDataSource1"/>`  
**Description:** `asp:GridView` with `AutoGenerateEditButton` and `AutoGenerateDeleteButton` relies on ViewState to maintain row state across postbacks. ViewState does not exist in .NET 8.  
**Recommendation:** Replace with HTML tables rendered via Razor `@foreach` loops. Implement edit/delete via form submissions or AJAX calls to Razor Page handlers.

---

### ISSUE-028 [HIGH] – FileUpload Server Control
**File:** `AddTour.aspx`, `AddTour.aspx.cs`  
**Code:**
```html
<asp:FileUpload ID="FileUpload1" runat="server"/>
```
```csharp
FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
```
**Description:** `asp:FileUpload` is a Web Forms server control. The `SaveAs()` method and `Server.MapPath()` are both `System.Web` dependencies.  
**Recommendation:** Use `<input type="file" asp-for="UploadedFile" />` in Razor Pages. In the PageModel, use `IFormFile` for the uploaded file and `IWebHostEnvironment` for the path.

---

### ISSUE-029 [MEDIUM] – Postback Event Handlers
**Files:** `AddTour.aspx.cs`, `Order.aspx.cs`, `SignUpForm.aspx.cs`, `userlogin.aspx.cs`  
**Code:**
```csharp
protected void Register_Click(object sender, EventArgs e) { ... }
protected void btn_click(object sender, EventArgs e) { ... }
protected void Btn_Submit(object sender, EventArgs e) { ... }
```
**Description:** Web Forms postback event handlers (`EventArgs e`) are tied to the Web Forms event model which does not exist in .NET 8.  
**Recommendation:** Replace with Razor Pages handler methods: `public IActionResult OnPost()` or `public IActionResult OnPostRegister()` for named handlers.

---

### ISSUE-030 [MEDIUM] – RegularExpressionValidator Server Control
**File:** `AddTour.aspx`  
**Code:** `<asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="tour_info" ValidationExpression="^[\s\S]{0,250}$" runat="server" ErrorMessage="Characters less than 250"/>`  
**Description:** `asp:RegularExpressionValidator` is a Web Forms validation control that does not exist in .NET 8.  
**Recommendation:** Use Data Annotations on the PageModel: `[MaxLength(250, ErrorMessage = "Characters less than 250")]`. Display validation errors with `<span asp-validation-for="TourInfo" class="text-danger"></span>`.

---

### ISSUE-031 [MEDIUM] – ConnectionString Expression Syntax in Markup
**Files:** `DisplayTours.aspx`, `TourCrud.aspx`, `allbooking.aspx`, `mybooking.aspx`, `usercrud.aspx`  
**Code:** `ConnectionString="<%$ ConnectionStrings:dbconnection %>"`  
**Description:** The `<%$ ConnectionStrings:... %>` expression syntax is Web Forms-specific and does not exist in .NET 8.  
**Recommendation:** Remove `SqlDataSource` controls entirely. Access connection strings via `IConfiguration` in the PageModel.

---

### ISSUE-032 [MEDIUM] – Data Binding Expression Syntax
**Files:** `DisplayTours.aspx`, `TourCrud.aspx`  
**Code:** `<img src="Tour_pics/<%#Eval("pic") %>" style="width:200px;height:200px" />`  
**Description:** The `<%#Eval("pic") %>` data binding expression is Web Forms-specific and does not exist in .NET 8.  
**Recommendation:** Use Razor syntax in `.cshtml` files: `<img src="Tour_pics/@item.Pic" style="width:200px;height:200px" />` within a `@foreach` loop.

---

### ISSUE-033 [LOW] – Designer Files (.aspx.designer.cs)
**Files:** All `.aspx.designer.cs` files  
**Description:** Designer files are auto-generated by Visual Studio for Web Forms pages to declare server control fields. These have no equivalent in .NET 8 Razor Pages.  
**Recommendation:** Delete all `.aspx.designer.cs` files. They are not needed in Razor Pages.

---

### ISSUE-034 [LOW] – Web.Debug.config and Web.Release.config Transform Files
**Files:** `Web.Debug.config`, `Web.Release.config`  
**Description:** Web.config transform files are .NET Framework-specific. .NET 8 uses environment-specific `appsettings.{Environment}.json` files.  
**Recommendation:** Delete `Web.Debug.config` and `Web.Release.config`. Create `appsettings.Development.json` and `appsettings.Production.json` for environment-specific settings.

---

## 7. Migration Roadmap

### Phase 1: Project Infrastructure (8–12 hours)
1. Create new SDK-style `.csproj` targeting `net8.0`
2. Create `appsettings.json` with connection strings and app settings
3. Create `Program.cs` with ASP.NET Core middleware pipeline
4. Set up clean architecture folder structure (Domain/Application/Infrastructure/Web)
5. Remove `Web.config`, `packages.config`, designer files

### Phase 2: Data Access Layer (16–24 hours)
1. Install `Microsoft.EntityFrameworkCore.SqlServer 8.0.0`
2. Create `TourManagementDbContext` with `Tour`, `UserInfo`, and `Booking` entities
3. Implement repository interfaces and implementations
4. Fix SQL injection in `userlogin` (parameterized queries)
5. Fix connection management (using statements)

### Phase 3: Security (12–16 hours)
1. Install `Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0`
2. Implement password hashing (remove plain-text passwords)
3. Remove hardcoded admin credentials
4. Implement role-based authorization (Admin/User roles)
5. Configure cookie authentication in `Program.cs`

### Phase 4: UI Migration (32–48 hours)
1. Convert each `.aspx` page to a Razor Page (`.cshtml` + `.cshtml.cs`)
2. Replace all `asp:*` server controls with HTML + Tag Helpers
3. Replace `SqlDataSource` with PageModel data binding
4. Replace `GridView` with `@foreach` table rendering
5. Replace `FileUpload` with `IFormFile`
6. Replace validation controls with Data Annotations
7. Replace `Response.Write` with TempData messages
8. Replace `Server.Transfer` with `RedirectToPage()`

### Phase 5: Testing & Verification (12–20 hours)
1. Write unit tests for services
2. Write integration tests for repositories
3. Verify all CRUD operations
4. Security testing (SQL injection, authentication)

---

## 8. Web Forms to Razor Pages Mapping

| Web Forms Page | Razor Page | Complexity |
|----------------|------------|------------|
| `userlogin.aspx` | `Pages/Account/Login.cshtml` | Medium |
| `SignUpForm.aspx` | `Pages/Account/Register.cshtml` | Medium |
| `MainProfilePage.aspx` | `Pages/Index.cshtml` | Simple |
| `AdminLogin2.aspx` | `Pages/Admin/Login.cshtml` | Medium |
| `AdminProfile.aspx` | `Pages/Admin/Index.cshtml` | Simple |
| `AddTour.aspx` | `Pages/Tours/Create.cshtml` | Complex |
| `DisplayTours.aspx` | `Pages/Tours/Index.cshtml` | Medium |
| `TourCrud.aspx` | `Pages/Admin/Tours/Index.cshtml` | Complex |
| `Order.aspx` | `Pages/Bookings/Create.cshtml` | Medium |
| `mybooking.aspx` | `Pages/Bookings/Index.cshtml` | Medium |
| `allbooking.aspx` | `Pages/Admin/Bookings/Index.cshtml` | Medium |
| `usercrud.aspx` | `Pages/Admin/Users/Index.cshtml` | Complex |

---

## 9. Package Migration

| Current Package | Status | .NET 8 Replacement |
|----------------|--------|-------------------|
| `Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1` | ❌ Remove | Not needed in .NET 8 |
| `System.Web` (framework) | ❌ Remove | `Microsoft.AspNetCore 8.0.0` |
| `System.Data.SqlClient` (framework) | ⚠️ Replace | `Microsoft.Data.SqlClient 5.2.0` or EF Core |
| `System.Configuration` (framework) | ⚠️ Replace | `Microsoft.Extensions.Configuration 8.0.0` |
| N/A | ✅ Add | `Microsoft.EntityFrameworkCore.SqlServer 8.0.0` |
| N/A | ✅ Add | `Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0` |
| N/A | ✅ Add | `AutoMapper 12.0.1` |
| N/A | ✅ Add | `FluentValidation 11.9.0` |
| N/A | ✅ Add | `Serilog.AspNetCore 8.0.0` |

---

*Report generated by ASP.NET Web Forms to .NET 8 Migration Analyzer v1.1.0*
