# ASP.NET Web Forms to .NET 8 Migration Analysis Report
## Tour Management Application

**Analysis Date:** 2025-01-30  
**Current Framework:** ASP.NET Web Forms 4.7.2  
**Target Framework:** .NET 8  
**Module:** Tour_Management  

---

## Executive Summary

The Tour Management application is a classic ASP.NET Web Forms application targeting .NET Framework 4.7.2. The application provides tour management functionality including user registration/login, tour browsing, booking management, and admin capabilities.

**Total Issues Found: 42**
- Critical: 12
- High: 14
- Medium: 10
- Low: 6

**Migration Complexity: Complex**  
**Estimated Effort: 80-120 hours**  
**Compatibility Score: 18/100**

---

## Project Inventory

### Web Forms Pages (11 .aspx files)
| Page | Code-Behind | Complexity | Purpose |
|------|-------------|------------|---------|
| AddTour.aspx | AddTour.aspx.cs | Medium | Admin: Add new tour with file upload |
| AdminLogin2.aspx | AdminLogin2.aspx.cs | Simple | Admin login (hardcoded credentials) |
| AdminProfile.aspx | AdminProfile.aspx.cs | Simple | Admin dashboard/navigation |
| allbooking.aspx | allbooking.aspx.cs | Medium | Admin: View all bookings (GridView + SqlDataSource) |
| DisplayTours.aspx | DisplayTours.aspx.cs | Medium | User: Browse tours (GridView + SqlDataSource) |
| MainProfilePage.aspx | MainProfilePage.aspx.cs | Simple | User home/dashboard |
| mybooking.aspx | mybooking.aspx.cs | Medium | User: View/delete own bookings |
| Order.aspx | Order.aspx.cs | Medium | User: Book a tour |
| SignUpForm.aspx | SignUpForm.aspx.cs | Medium | User registration |
| TourCrud.aspx | TourCrud.aspx.cs | Medium | Admin: CRUD tours (GridView + SqlDataSource) |
| usercrud.aspx | usercrud.aspx.cs | Medium | User: View/edit own profile |
| userlogin.aspx | userlogin.aspx.cs | Medium | User login |

### No Master Pages Found
### No User Controls (.ascx) Found
### No Global.asax Found

---

## Detailed Issue Findings

### CRITICAL Issues

#### ISSUE-001: System.Web Namespace Dependencies (All Code-Behind Files)
- **Severity:** Critical
- **Category:** webforms-migration / deprecated-api
- **Files Affected:** All 12 .aspx.cs files
- **Breaking Change:** Yes
- **Description:** All code-behind files import `System.Web`, `System.Web.UI`, and `System.Web.UI.WebControls` namespaces which do not exist in .NET 8. The entire System.Web assembly is .NET Framework-only.
- **Code Snippet:**
  ```csharp
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  ```
- **Impact:** Complete rewrite of all page classes required. No direct migration path exists.
- **Recommendation:** Replace with ASP.NET Core Razor Pages. Each `System.Web.UI.Page` class becomes a `PageModel` class. Remove all `System.Web.*` using statements.
- **Effort:** High

#### ISSUE-002: Web Forms Page Lifecycle (Page_Load Events)
- **Severity:** Critical
- **Category:** webforms-migration
- **Files Affected:** AddTour.aspx.cs (line 14), AdminLogin2.aspx.cs (line 10), AdminProfile.aspx.cs (line 10), allbooking.aspx.cs (line 10), DisplayTours.aspx.cs (line 11), MainProfilePage.aspx.cs (line 10), mybooking.aspx.cs (line 10), Order.aspx.cs (line 10), SignUpForm.aspx.cs (line 10), TourCrud.aspx.cs (line 13), usercrud.aspx.cs (line 10), userlogin.aspx.cs (line 10)
- **Breaking Change:** Yes
- **Description:** All pages use the Web Forms `Page_Load` event lifecycle pattern which does not exist in .NET 8. The `Page.IsPostBack` pattern (TourCrud.aspx.cs line 15) is also Web Forms-specific.
- **Code Snippet:**
  ```csharp
  protected void Page_Load(object sender, EventArgs e)
  {
      if (!Page.IsPostBack)
      {
          refreshdata();
      }
  }
  ```
- **Impact:** All page initialization logic must be rewritten using Razor Pages `OnGet`/`OnPost` handlers or MVC controller actions.
- **Recommendation:** Convert `Page_Load` to `OnGet()` in Razor Pages PageModel. Replace `Page.IsPostBack` check with separate `OnGet` and `OnPost` handlers.
- **Effort:** High

#### ISSUE-003: ASP.NET Web Forms Server Controls (.aspx markup)
- **Severity:** Critical
- **Category:** webforms-migration
- **Files Affected:** All 11 .aspx files
- **Breaking Change:** Yes
- **Description:** All .aspx pages use Web Forms server controls (`<asp:TextBox>`, `<asp:Button>`, `<asp:Label>`, `<asp:GridView>`, `<asp:FileUpload>`, `<asp:SqlDataSource>`, `<asp:RegularExpressionValidator>`, `<asp:HyperLink>`, `<asp:DropDownList>`, `<asp:BoundField>`, `<asp:TemplateField>`) which are not available in .NET 8.
- **Code Snippet:**
  ```html
  <asp:TextBox id="tour_name" required="true" ForeColor="Black" class="form-control" runat="server"/>
  <asp:Button BackColor="#cc6600" ID="Register" runat="server" Text="Register" OnClick="Register_Click" />
  <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" ...>
  ```
- **Impact:** All .aspx markup must be completely rewritten as Razor Pages (.cshtml) using HTML helpers, Tag Helpers, or plain HTML.
- **Recommendation:** Convert all server controls to HTML equivalents with Tag Helpers. Replace `<asp:GridView>` with HTML `<table>` or a modern component. Replace `<asp:TextBox>` with `<input asp-for="...">`.
- **Effort:** High

#### ISSUE-004: SqlDataSource Server Control (Data-Bound Controls)
- **Severity:** Critical
- **Category:** webforms-migration / deprecated-api
- **Files Affected:** TourCrud.aspx (line 12), DisplayTours.aspx (line 13), allbooking.aspx (line 22), mybooking.aspx (line 22), usercrud.aspx (line 22)
- **Breaking Change:** Yes
- **Description:** Five pages use `<asp:SqlDataSource>` for declarative data binding directly in markup. This control does not exist in .NET 8 and represents a tightly coupled data access pattern.
- **Code Snippet:**
  ```html
  <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
      ConnectionString="<%$ ConnectionStrings:dbconnection %>" 
      SelectCommand="SELECT * FROM [Tour]"
      UpdateCommand="UPDATE [Tour] Set [TOUR_NAME]=@TOUR_NAME..."
      DeleteCommand="Delete from [Tour] Where [TOUR_ID]=@TOUR_ID">
  </asp:SqlDataSource>
  ```
- **Impact:** All data access must be moved to code-behind (PageModel) using EF Core or ADO.NET. The declarative data binding pattern is completely incompatible.
- **Recommendation:** Replace with EF Core DbContext and repository pattern. Move all SQL queries to service/repository layer. Use model binding in Razor Pages.
- **Effort:** High

#### ISSUE-005: Raw ADO.NET with SqlConnection/SqlCommand (Direct Database Access)
- **Severity:** Critical
- **Category:** deprecated-api / security
- **Files Affected:** AddTour.aspx.cs (lines 20-35), userlogin.aspx.cs (lines 22-30), SignUpForm.aspx.cs (lines 18-32), Order.aspx.cs (lines 16-28), TourCrud.aspx.cs (lines 18-22)
- **Breaking Change:** Yes
- **Description:** Multiple code-behind files use raw ADO.NET `SqlConnection` and `SqlCommand` directly. While `System.Data.SqlClient` has a .NET 8 equivalent (`Microsoft.Data.SqlClient`), the pattern should be replaced with EF Core per migration rules.
- **Code Snippet:**
  ```csharp
  SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
  conn.Open();
  string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";
  SqlCommand com = new SqlCommand(insertQuery, conn);
  ```
- **Impact:** Data access layer must be completely redesigned using EF Core with proper repository pattern.
- **Recommendation:** Replace with EF Core DbContext. Create entity classes for `Tour`, `UserInfo`, and `Booking`. Implement repository interfaces and implementations.
- **Effort:** High

#### ISSUE-006: ConfigurationManager Usage
- **Severity:** Critical
- **Category:** deprecated-api / breaking-change
- **Files Affected:** AddTour.aspx.cs (line 21), userlogin.aspx.cs (line 23), SignUpForm.aspx.cs (line 19), Order.aspx.cs (line 17), TourCrud.aspx.cs (line 19)
- **Breaking Change:** Yes
- **Description:** `System.Configuration.ConfigurationManager` is used to read connection strings. While a compatibility NuGet package exists, the proper .NET 8 approach is `IConfiguration` with `appsettings.json`.
- **Code Snippet:**
  ```csharp
  ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString
  ```
- **Impact:** Configuration system must be migrated from Web.config to appsettings.json with IConfiguration injection.
- **Recommendation:** Replace with `IConfiguration` injected via DI. Move connection string to `appsettings.json`. Use `builder.Configuration.GetConnectionString("dbconnection")` in Program.cs.
- **Effort:** Medium

#### ISSUE-007: Web.config Configuration File
- **Severity:** Critical
- **Category:** webforms-migration / breaking-change
- **Files Affected:** Web.config (entire file)
- **Breaking Change:** Yes
- **Description:** The entire Web.config configuration system is .NET Framework-specific and not supported in .NET 8. This includes `<system.web>`, `<system.webServer>`, `<compilation>`, `<httpRuntime>`, `<httpHandlers>`, `<connectionStrings>`, and `<appSettings>` sections.
- **Code Snippet:**
  ```xml
  <system.web>
    <compilation debug="true" targetFramework="4.7.2">
    <httpRuntime targetFramework="4.7.2"/>
  </system.web>
  <connectionStrings>
    <add name="dbconnection" connectionString="Data Source=(LocalDB)\MSSQLLocalDB;..."/>
  </connectionStrings>
  ```
- **Impact:** Complete configuration migration required. All settings must be moved to `appsettings.json`.
- **Recommendation:** Create `appsettings.json` with connection strings and app settings. Create `appsettings.Development.json` for development-specific settings. Configure in `Program.cs`.
- **Effort:** Medium

#### ISSUE-008: Legacy .csproj Format (Non-SDK Style)
- **Severity:** Critical
- **category:** breaking-change / project-configuration
- **Files Affected:** Tour_Management.csproj (entire file)
- **Breaking Change:** Yes
- **Description:** The project file uses the old non-SDK-style MSBuild format with `ToolsVersion="15.0"` and `ProjectTypeGuids` for Web Application. .NET 8 requires SDK-style project files.
- **Code Snippet:**
  ```xml
  <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <ProjectTypeGuids>{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}</ProjectTypeGuids>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
  ```
- **Impact:** Project file must be completely rewritten in SDK-style format targeting `net8.0`.
- **Recommendation:** Replace with `<Project Sdk="Microsoft.NET.Sdk.Web">` format. Set `<TargetFramework>net8.0</TargetFramework>`. Use `PackageReference` instead of `packages.config`.
- **Effort:** Medium

#### ISSUE-009: Server.MapPath Usage (File Upload)
- **Severity:** Critical
- **Category:** deprecated-api / breaking-change
- **Files Affected:** AddTour.aspx.cs (line 31)
- **Breaking Change:** Yes
- **Description:** `Server.MapPath()` is a `System.Web.HttpServerUtility` method that does not exist in .NET 8. Used for resolving physical file paths for file uploads.
- **Code Snippet:**
  ```csharp
  FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
  ```
- **Impact:** File upload mechanism must be completely rewritten using `IWebHostEnvironment.WebRootPath` or `IWebHostEnvironment.ContentRootPath`.
- **Recommendation:** Inject `IWebHostEnvironment` and use `_env.WebRootPath` to resolve paths. Use `IFormFile` for file uploads in Razor Pages.
- **Effort:** Medium

#### ISSUE-010: Response.Write Usage
- **Severity:** Critical
- **Category:** deprecated-api / breaking-change
- **Files Affected:** AddTour.aspx.cs (line 36), userlogin.aspx.cs (lines 31, 40), SignUpForm.aspx.cs (line 33), Order.aspx.cs (line 25)
- **Breaking Change:** Yes
- **Description:** `Response.Write()` is a `System.Web.HttpResponse` method not available in .NET 8. Used for inline feedback messages.
- **Code Snippet:**
  ```csharp
  Response.Write("ADD  Successful");
  Response.Write("Password is correct");
  Response.Write("Registration Successful");
  ```
- **Impact:** All inline response writing must be replaced with proper model-based feedback (TempData, ViewData, or model properties).
- **Recommendation:** Use `TempData["Message"]` or model properties to pass messages to the view. Display messages in Razor markup using `@TempData["Message"]`.
- **Effort:** Low

#### ISSUE-011: Response.Redirect + Server.Transfer Pattern
- **Severity:** Critical
- **Category:** deprecated-api / breaking-change
- **Files Affected:** AdminLogin2.aspx.cs (lines 14-15), userlogin.aspx.cs (lines 33-34), SignUpForm.aspx.cs (lines 34-35), Order.aspx.cs (lines 26-27)
- **Breaking Change:** Yes
- **Description:** Multiple files use both `Response.Redirect()` AND `Server.Transfer()` on consecutive lines, which is logically incorrect (code after `Response.Redirect` is unreachable). `Server.Transfer()` does not exist in .NET 8.
- **Code Snippet:**
  ```csharp
  Response.Redirect("MainProfilePage.aspx");
  Server.Transfer("MainProfilePage.aspx");  // Unreachable - Server.Transfer not in .NET 8
  ```
- **Impact:** Navigation logic must be rewritten. `Server.Transfer` has no direct equivalent in ASP.NET Core.
- **Recommendation:** Use `return RedirectToPage("/MainProfilePage")` in Razor Pages. Remove all `Server.Transfer` calls. Fix the logical error of calling both methods.
- **Effort:** Low

#### ISSUE-012: Hardcoded Admin Credentials (Security Critical)
- **Severity:** Critical
- **Category:** security
- **Files Affected:** AdminLogin2.aspx.cs (lines 12-15)
- **Breaking Change:** No (logic issue, not framework)
- **Description:** Admin authentication uses hardcoded credentials compared directly in Page_Load, which is a severe security vulnerability. Additionally, the login check runs on every page load, not just on form submission.
- **Code Snippet:**
  ```csharp
  protected void Page_Load(object sender, EventArgs e)
  {
      if (password.Text == "admin" && name.Text == "admin@gmail.com")
      {
          Response.Redirect("AdminProfile.aspx");
      }
  }
  ```
- **Impact:** Security breach risk. Must be replaced with proper authentication.
- **Recommendation:** Implement ASP.NET Core Identity with role-based authorization. Use `[Authorize(Roles = "Admin")]` attribute. Store hashed passwords in database.
- **Effort:** High

---

### HIGH Issues

#### ISSUE-013: SQL Injection Vulnerability (userlogin.aspx.cs)
- **Severity:** High
- **Category:** security
- **Files Affected:** userlogin.aspx.cs (lines 25-26)
- **Breaking Change:** No (security issue)
- **Description:** The login query uses string concatenation to build SQL, creating a SQL injection vulnerability. Passwords are stored and compared in plain text.
- **Code Snippet:**
  ```csharp
  string checkPasswordQuery = "select password from Userinfo where password='" + txtPassword.Text + "' and email = '" + txtEmail.Text + "'";
  ```
- **Impact:** Critical security vulnerability allowing database compromise.
- **Recommendation:** Use parameterized queries (already done in other files) or EF Core. Hash passwords using BCrypt or ASP.NET Core Identity's password hasher.
- **Effort:** Medium

#### ISSUE-014: Plain Text Password Storage
- **Severity:** High
- **Category:** security
- **Files Affected:** SignUpForm.aspx.cs (line 27), usercrud.aspx (line 12 - Password column displayed), userlogin.aspx.cs (line 25)
- **Breaking Change:** No (security issue)
- **Description:** Passwords are stored in plain text in the database and even displayed in the user CRUD grid view.
- **Code Snippet:**
  ```csharp
  com.Parameters.AddWithValue("@Password", password1.Text);
  // In usercrud.aspx:
  <asp:BoundField DataField="Password" HeaderText="Password" SortExpression="Password" />
  ```
- **Impact:** Complete exposure of all user passwords if database is compromised.
- **Recommendation:** Use ASP.NET Core Identity's `IPasswordHasher<T>` to hash passwords before storage. Never display passwords in UI.
- **Effort:** Medium

#### ISSUE-015: No Authentication/Authorization Mechanism
- **Severity:** High
- **Category:** security / webforms-migration
- **Files Affected:** All pages
- **Breaking Change:** Yes
- **Description:** There is no Forms Authentication, session-based authentication, or any authorization mechanism protecting admin pages or user-specific pages. Any user can navigate directly to AdminProfile.aspx, AddTour.aspx, TourCrud.aspx, etc.
- **Impact:** Unauthorized access to all admin functionality.
- **Recommendation:** Implement ASP.NET Core Identity. Add `[Authorize]` and `[Authorize(Roles = "Admin")]` attributes to Razor Pages. Configure authentication middleware in Program.cs.
- **Effort:** High

#### ISSUE-016: System.Web.DataVisualization Chart Control
- **Severity:** High
- **Category:** deprecated-api / breaking-change
- **Files Affected:** allbooking.aspx (line 2), Web.config (lines 4-8, 12-16, 20-22)
- **Breaking Change:** Yes
- **Description:** The `allbooking.aspx` page registers `System.Web.DataVisualization.Charting` assembly. Web.config also configures the `ChartHttpHandler`. This assembly is .NET Framework-only.
- **Code Snippet:**
  ```html
  <%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" 
      namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>
  ```
- **Impact:** Chart functionality must be replaced with a .NET 8 compatible charting library.
- **Recommendation:** Replace with a JavaScript charting library (Chart.js, ApexCharts) or a .NET 8 compatible server-side library. Remove all `System.Web.DataVisualization` references.
- **Effort:** Medium

#### ISSUE-017: Microsoft.CodeDom.Providers.DotNetCompilerPlatform Package
- **Severity:** High
- **Category:** package-compatibility
- **Files Affected:** packages.config (line 2), Tour_Management.csproj (lines 1, 47-49)
- **Breaking Change:** Yes
- **Description:** The `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` package (version 2.0.1) is a .NET Framework-specific package for Roslyn compiler support in Web Forms. It is not needed or compatible with .NET 8.
- **Code Snippet:**
  ```xml
  <package id="Microsoft.CodeDom.Providers.DotNetCompilerPlatform" version="2.0.1" targetFramework="net472" />
  ```
- **Impact:** Package must be removed. The Roslyn compiler is built into .NET 8 SDK.
- **Recommendation:** Remove from packages.config and csproj. Delete the packages.config file entirely and use PackageReference format.
- **Effort:** Low

#### ISSUE-018: System.Web.DynamicData Reference
- **Severity:** High
- **Category:** deprecated-api / breaking-change
- **Files Affected:** Tour_Management.csproj (line 36)
- **Breaking Change:** Yes
- **Description:** The project references `System.Web.DynamicData` which is a .NET Framework-only assembly for Dynamic Data scaffolding.
- **Code Snippet:**
  ```xml
  <Reference Include="System.Web.DynamicData" />
  ```
- **Impact:** Must be removed. No equivalent in .NET 8.
- **Recommendation:** Remove this reference. Dynamic Data functionality should be replaced with EF Core scaffolding or custom Razor Pages.
- **Effort:** Low

#### ISSUE-019: System.Web.Entity Reference
- **Severity:** High
- **Category:** deprecated-api / breaking-change
- **Files Affected:** Tour_Management.csproj (line 37)
- **Breaking Change:** Yes
- **Description:** The project references `System.Web.Entity` which is a .NET Framework-only assembly.
- **Code Snippet:**
  ```xml
  <Reference Include="System.Web.Entity" />
  ```
- **Impact:** Must be removed. Use EF Core instead.
- **Recommendation:** Remove this reference. Replace with `Microsoft.EntityFrameworkCore` NuGet package.
- **Effort:** Low

#### ISSUE-020: System.Web.ApplicationServices Reference
- **Severity:** High
- **Category:** deprecated-api / breaking-change
- **Files Affected:** Tour_Management.csproj (line 38)
- **Breaking Change:** Yes
- **Description:** `System.Web.ApplicationServices` is a .NET Framework-only assembly providing membership, roles, and profile services.
- **Code Snippet:**
  ```xml
  <Reference Include="System.Web.ApplicationServices" />
  ```
- **Impact:** Must be removed. Replace with ASP.NET Core Identity.
- **Recommendation:** Remove this reference. Implement ASP.NET Core Identity for membership and role management.
- **Effort:** Low

#### ISSUE-021: System.Web.Extensions Reference
- **Severity:** High
- **Category:** deprecated-api / breaking-change
- **Files Affected:** Tour_Management.csproj (line 42)
- **Breaking Change:** Yes
- **Description:** `System.Web.Extensions` provides UpdatePanel, ScriptManager, and other AJAX Web Forms controls. Not available in .NET 8.
- **Code Snippet:**
  ```xml
  <Reference Include="System.Web.Extensions" />
  ```
- **Impact:** Must be removed. AJAX functionality must be reimplemented.
- **Recommendation:** Remove this reference. Use JavaScript fetch API or Blazor for AJAX-like functionality.
- **Effort:** Low

#### ISSUE-022: System.Web.Services Reference
- **Severity:** High
- **Category:** deprecated-api / breaking-change
- **Files Affected:** Tour_Management.csproj (line 47)
- **Breaking Change:** Yes
- **Description:** `System.Web.Services` provides ASMX web services which are not supported in .NET 8.
- **Code Snippet:**
  ```xml
  <Reference Include="System.Web.Services" />
  ```
- **Impact:** Must be removed. Any web services must be reimplemented as ASP.NET Core Web API.
- **Recommendation:** Remove this reference. Use `Microsoft.AspNetCore.Mvc` for API endpoints.
- **Effort:** Low

#### ISSUE-023: System.EnterpriseServices Reference
- **Severity:** High
- **Category:** deprecated-api / breaking-change
- **Files Affected:** Tour_Management.csproj (line 48)
- **Breaking Change:** Yes
- **Description:** `System.EnterpriseServices` provides COM+ services which are Windows-only and not supported in .NET 8 cross-platform scenarios.
- **Code Snippet:**
  ```xml
  <Reference Include="System.EnterpriseServices" />
  ```
- **Impact:** Must be removed. COM+ services are not available in .NET 8.
- **Recommendation:** Remove this reference. Replace COM+ functionality with modern .NET 8 equivalents.
- **Effort:** Low

#### ISSUE-024: System.Web.DataVisualization Reference
- **Severity:** High
- **Category:** deprecated-api / breaking-change
- **Files Affected:** Tour_Management.csproj (line 35)
- **Breaking Change:** Yes
- **Description:** `System.Web.DataVisualization` is the charting assembly for Web Forms, not available in .NET 8.
- **Code Snippet:**
  ```xml
  <Reference Include="System.Web.DataVisualization" />
  ```
- **Impact:** Must be removed. Replace with JavaScript charting library.
- **Recommendation:** Remove this reference. Use Chart.js or similar JavaScript library for charts.
- **Effort:** Low

#### ISSUE-025: Hardcoded Absolute File Path in Connection String
- **Severity:** High
- **Category:** configuration / security
- **Files Affected:** Web.config (line 27)
- **Breaking Change:** No (configuration issue)
- **Description:** The connection string contains a hardcoded absolute path to the developer's local machine (`C:\Users\gajer\source\repos\...`). This will not work in any other environment.
- **Code Snippet:**
  ```xml
  connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gajer\source\repos\Tour_Management\Tour_Management\App_Data\tourdb.mdf;Integrated Security=True"
  ```
- **Impact:** Application will fail to connect to database in any environment other than the original developer's machine.
- **Recommendation:** Use a proper SQL Server connection string with server name and database name. Store in `appsettings.json` with environment-specific overrides. Use `|DataDirectory|` substitution or environment variables.
- **Effort:** Low

#### ISSUE-026: No Session Management for User Authentication
- **Severity:** High
- **Category:** security / webforms-migration
- **Files Affected:** userlogin.aspx.cs (line 30 - commented out session code)
- **Breaking Change:** Yes
- **Description:** The session-based user tracking code is commented out (`//Session["New"] = txtEmail.Text;`). There is no mechanism to track which user is logged in, making user-specific pages (mybooking, usercrud) non-functional.
- **Code Snippet:**
  ```csharp
  //Session["New"] = txtEmail.Text;
  ```
- **Impact:** User-specific functionality is broken. Any user can see any user's data.
- **Recommendation:** Implement ASP.NET Core Identity with claims-based authentication. Use `User.Identity.Name` to identify the current user.
- **Effort:** High

---

### MEDIUM Issues

#### ISSUE-027: FileUpload Server Control
- **Severity:** Medium
- **Category:** webforms-migration
- **Files Affected:** AddTour.aspx (line 47), AddTour.aspx.cs (lines 31-33)
- **Breaking Change:** Yes
- **Description:** `<asp:FileUpload>` server control is Web Forms-specific. The file saving uses `Server.MapPath()` which is also incompatible.
- **Code Snippet:**
  ```html
  <asp:FileUpload ID="FileUpload1" runat="server"/>
  ```
  ```csharp
  FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
  ```
- **Recommendation:** Use `<input type="file" asp-for="UploadedFile">` with `IFormFile` in Razor Pages. Use `IWebHostEnvironment.WebRootPath` for path resolution.
- **Effort:** Medium

#### ISSUE-028: RegularExpressionValidator Server Control
- **Severity:** Medium
- **Category:** webforms-migration
- **Files Affected:** AddTour.aspx (line 63)
- **Breaking Change:** Yes
- **Description:** `<asp:RegularExpressionValidator>` is a Web Forms validation control with no equivalent in .NET 8.
- **Code Snippet:**
  ```html
  <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="tour_info" 
      ValidationExpression="^[\s\S]{0,250}$" runat="server" ErrorMessage="Characters less than 250">
  ```
- **Recommendation:** Use Data Annotations (`[MaxLength(250)]`) on the model property, or FluentValidation. Display validation errors using `<span asp-validation-for="...">` Tag Helper.
- **Effort:** Low

#### ISSUE-029: DropDownList Server Control
- **Severity:** Medium
- **Category:** webforms-migration
- **Files Affected:** SignUpForm.aspx (lines 30-34)
- **Breaking Change:** Yes
- **Description:** `<asp:DropDownList>` with `<asp:ListItem>` is a Web Forms server control.
- **Code Snippet:**
  ```html
  <asp:DropDownList ID="gender" runat="server" Width="361px" ForeColor="Black" class="form-control">
      <asp:ListItem Text="Male"></asp:ListItem>
      <asp:ListItem Text="Female"></asp:ListItem>
  </asp:DropDownList>
  ```
- **Recommendation:** Replace with HTML `<select asp-for="Gender" asp-items="...">` Tag Helper with a `SelectList` in the PageModel.
- **Effort:** Low

#### ISSUE-030: HyperLink Server Control
- **Severity:** Medium
- **Category:** webforms-migration
- **Files Affected:** DisplayTours.aspx (lines 24-26)
- **Breaking Change:** Yes
- **Description:** `<asp:HyperLink>` is a Web Forms server control.
- **Code Snippet:**
  ```html
  <asp:HyperLink ID="HyperLink1" href="Order.aspx" runat="server">Book Now</asp:HyperLink>
  ```
- **Recommendation:** Replace with standard HTML `<a href="/Order">Book Now</a>` or `<a asp-page="/Order">Book Now</a>` Tag Helper.
- **Effort:** Low

#### ISSUE-031: Label Server Control
- **Severity:** Medium
- **Category:** webforms-migration
- **Files Affected:** Multiple .aspx files
- **Breaking Change:** Yes
- **Description:** `<asp:Label>` server controls are used throughout all forms.
- **Code Snippet:**
  ```html
  <asp:Label ID="Label1" runat="server" Text="Email"/>
  ```
- **Recommendation:** Replace with HTML `<label asp-for="Email">Email</label>` Tag Helper or plain `<label>` elements.
- **Effort:** Low

#### ISSUE-032: GridView with AutoGenerateEditButton/DeleteButton
- **Severity:** Medium
- **Category:** webforms-migration
- **Files Affected:** TourCrud.aspx (line 7), mybooking.aspx (line 7), usercrud.aspx (line 7)
- **Breaking Change:** Yes
- **Description:** GridView with auto-generated edit/delete buttons is a Web Forms-specific pattern that generates postback-based CRUD operations.
- **Code Snippet:**
  ```html
  <asp:GridView ID="GridView1" runat="server" AutoGenerateDeleteButton="True" AutoGenerateEditButton="True" 
      DataKeyNames="TOUR_ID" DataSourceID="SqlDataSource1" ...>
  ```
- **Recommendation:** Replace with an HTML table bound to a model collection. Add edit/delete links using `<a asp-page-handler="Delete" asp-route-id="@item.Id">Delete</a>`.
- **Effort:** Medium

#### ISSUE-033: Inline SQL in Markup (SqlDataSource)
- **Severity:** Medium
- **Category:** security / architecture
- **Files Affected:** usercrud.aspx (lines 22-26)
- **Breaking Change:** Yes
- **Description:** Complex SQL query embedded directly in .aspx markup using SqlDataSource, including a subquery pattern.
- **Code Snippet:**
  ```html
  SelectCommand="Select top (select COUNT(*) from UserInfo) * From UserInfo
  EXCEPT
  Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo"
  ```
- **Recommendation:** Move all SQL to repository layer. This query appears to select the last inserted row - replace with proper EF Core query using `OrderByDescending().FirstOrDefault()`.
- **Effort:** Medium

#### ISSUE-034: No Input Validation on Server Side
- **Severity:** Medium
- **Category:** security
- **Files Affected:** AddTour.aspx.cs, SignUpForm.aspx.cs, Order.aspx.cs
- **Breaking Change:** No
- **Description:** No server-side validation is performed on form inputs before database operations. Only client-side HTML5 `required` attributes are used.
- **Recommendation:** Add Data Annotations validation attributes to ViewModels/DTOs. Use FluentValidation for complex rules. Add `ModelState.IsValid` checks in page handlers.
- **Effort:** Medium

#### ISSUE-035: No Error Handling / Exception Management
- **Severity:** Medium
- **Category:** code-quality
- **Files Affected:** All code-behind files with database operations
- **Breaking Change:** No
- **Description:** No try-catch blocks exist around database operations. Any database error will result in an unhandled exception.
- **Code Snippet:**
  ```csharp
  SqlConnection conn = new SqlConnection(...);
  conn.Open();
  // No try-catch, no finally block to close connection
  com.ExecuteNonQuery();
  conn.Close(); // Never reached if exception occurs
  ```
- **Recommendation:** Wrap all database operations in try-catch-finally blocks. Use `using` statements for disposable resources. Implement global exception handling middleware in .NET 8.
- **Effort:** Medium

#### ISSUE-036: Database Connection Not Properly Disposed
- **Severity:** Medium
- **Category:** code-quality / resource-management
- **Files Affected:** AddTour.aspx.cs, userlogin.aspx.cs, SignUpForm.aspx.cs, Order.aspx.cs, TourCrud.aspx.cs
- **Breaking Change:** No
- **Description:** `SqlConnection` objects are opened but not wrapped in `using` statements. If an exception occurs, connections are never closed, leading to connection pool exhaustion.
- **Code Snippet:**
  ```csharp
  SqlConnection conn = new SqlConnection(...);
  conn.Open();
  // ... operations ...
  conn.Close(); // Not reached if exception thrown
  ```
- **Recommendation:** Use `using` statements or `await using` for async operations. EF Core handles connection management automatically.
- **Effort:** Low

---

### LOW Issues

#### ISSUE-037: Designer Files (.aspx.designer.cs)
- **Severity:** Low
- **Category:** webforms-migration
- **Files Affected:** All 11 .aspx.designer.cs files
- **Breaking Change:** Yes
- **Description:** Auto-generated designer files that declare server control fields are Web Forms-specific and have no equivalent in .NET 8.
- **Recommendation:** Delete all .designer.cs files. In Razor Pages, controls are accessed through model binding, not field declarations.
- **Effort:** Low

#### ISSUE-038: AssemblyInfo.cs Properties File
- **Severity:** Low
- **Category:** project-configuration
- **Files Affected:** Properties/AssemblyInfo.cs
- **Breaking Change:** No
- **Description:** The `AssemblyInfo.cs` file with assembly attributes is the old-style approach. SDK-style projects auto-generate assembly info.
- **Recommendation:** Remove `AssemblyInfo.cs` or keep only custom attributes. SDK-style projects generate standard assembly attributes automatically.
- **Effort:** Low

#### ISSUE-039: Web.Debug.config and Web.Release.config Transform Files
- **Severity:** Low
- **Category:** webforms-migration / configuration
- **Files Affected:** Web.Debug.config, Web.Release.config
- **Breaking Change:** Yes
- **Description:** Web.config transform files are .NET Framework-specific. .NET 8 uses environment-specific appsettings files.
- **Recommendation:** Replace with `appsettings.Development.json` and `appsettings.Production.json`. Use environment variables for sensitive configuration.
- **Effort:** Low

#### ISSUE-040: Inline CSS Styles in .aspx Pages
- **Severity:** Low
- **Category:** code-quality / maintainability
- **Files Affected:** Multiple .aspx files
- **Breaking Change:** No
- **Description:** All styling is done with inline `<style>` blocks in individual pages. No shared CSS files or master page for consistent styling.
- **Recommendation:** Create a shared layout page (`_Layout.cshtml`) with Bootstrap 5. Move common styles to `wwwroot/css/site.css`. Use Bootstrap classes consistently.
- **Effort:** Medium

#### ISSUE-041: No Master Page / Shared Layout
- **Severity:** Low
- **Category:** architecture / maintainability
- **Files Affected:** All .aspx files
- **Breaking Change:** No
- **Description:** No master page is used. Navigation menus are duplicated across AdminProfile.aspx and MainProfilePage.aspx with inline HTML.
- **Recommendation:** Create `Pages/Shared/_Layout.cshtml` in Razor Pages. Move navigation to layout. Use `@RenderBody()` for page content.
- **Effort:** Medium

#### ISSUE-042: Unreachable Code After Response.Redirect
- **Severity:** Low
- **Category:** code-quality
- **Files Affected:** AdminLogin2.aspx.cs (line 15), userlogin.aspx.cs (line 34), SignUpForm.aspx.cs (lines 34-35), Order.aspx.cs (line 27)
- **Breaking Change:** No
- **Description:** Code after `Response.Redirect()` calls is unreachable because `Response.Redirect` throws a `ThreadAbortException` in Web Forms (or ends response). The `Server.Transfer` calls after `Response.Redirect` are never executed.
- **Code Snippet:**
  ```csharp
  Response.Redirect("MainProfilePage.aspx");
  Server.Transfer("MainProfilePage.aspx"); // Unreachable
  conn.Close(); // Unreachable
  ```
- **Recommendation:** Remove all `Server.Transfer` calls. In .NET 8, use `return RedirectToPage(...)` which properly returns from the method.
- **Effort:** Low

---

## Migration Roadmap

### Phase 1: Project Setup (Week 1) - 8-12 hours
1. Create new .NET 8 solution with clean architecture (4 projects)
2. Set up SDK-style .csproj files for all projects
3. Configure `appsettings.json` with connection strings
4. Set up `Program.cs` with middleware pipeline
5. Install required NuGet packages (EF Core 8, Identity, AutoMapper, FluentValidation)

### Phase 2: Domain & Infrastructure Layer (Week 1-2) - 16-20 hours
1. Create domain entities: `Tour`, `UserInfo`, `Booking`
2. Create EF Core DbContext with entity configurations
3. Implement repository interfaces and implementations
4. Create and run EF Core migrations
5. Replace LocalDB MDF file with proper SQL Server connection

### Phase 3: Application Layer (Week 2) - 12-16 hours
1. Create DTOs for all entities
2. Implement service classes with business logic
3. Configure AutoMapper profiles
4. Add FluentValidation validators
5. Implement file upload service

### Phase 4: Security Implementation (Week 2-3) - 16-20 hours
1. Implement ASP.NET Core Identity
2. Create user registration with password hashing
3. Implement login with cookie authentication
4. Add role-based authorization (Admin/User roles)
5. Protect admin pages with `[Authorize(Roles = "Admin")]`

### Phase 5: Web Layer - Razor Pages (Week 3-4) - 24-32 hours
1. Create `_Layout.cshtml` with navigation
2. Convert all 11 .aspx pages to Razor Pages (.cshtml + .cshtml.cs)
3. Replace all server controls with HTML/Tag Helpers
4. Implement model binding and validation
5. Add file upload with IFormFile
6. Replace GridView with HTML tables

### Phase 6: Testing & Documentation (Week 4) - 8-12 hours
1. Write unit tests for services
2. Write integration tests for repositories
3. Create migration documentation
4. Update README

---

## Page Migration Mapping

| Web Forms Page | Razor Page Target | Complexity |
|----------------|-------------------|------------|
| userlogin.aspx | Pages/Account/Login.cshtml | Medium |
| SignUpForm.aspx | Pages/Account/Register.cshtml | Medium |
| MainProfilePage.aspx | Pages/Index.cshtml | Simple |
| AdminLogin2.aspx | Pages/Admin/Login.cshtml (or use Identity) | Simple |
| AdminProfile.aspx | Pages/Admin/Index.cshtml | Simple |
| AddTour.aspx | Pages/Tours/Create.cshtml | Medium |
| TourCrud.aspx | Pages/Tours/Index.cshtml | Medium |
| DisplayTours.aspx | Pages/Tours/Browse.cshtml | Medium |
| Order.aspx | Pages/Bookings/Create.cshtml | Medium |
| allbooking.aspx | Pages/Admin/Bookings/Index.cshtml | Medium |
| mybooking.aspx | Pages/Bookings/Index.cshtml | Medium |
| usercrud.aspx | Pages/Account/Profile.cshtml | Medium |

---

## Required .NET 8 Package References

```xml
<!-- Infrastructure Project -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />

<!-- Application Project -->
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="FluentValidation" Version="11.9.0" />

<!-- Web Project -->
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
```

## Packages to REMOVE
- `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` (2.0.1) - Not needed in .NET 8
- All `System.Web.*` assembly references
- `System.EnterpriseServices` reference

---

## Summary Statistics

| Category | Count |
|----------|-------|
| .aspx Web Forms pages | 11 |
| .aspx.cs code-behind files | 11 |
| .aspx.designer.cs files | 11 |
| Master pages (.master) | 0 |
| User controls (.ascx) | 0 |
| Global.asax | 0 |
| System.Web references in code | 12 files |
| SqlDataSource controls | 5 pages |
| GridView controls | 5 pages |
| Direct ADO.NET usage | 5 files |
| Security vulnerabilities | 4 |
| Breaking changes | 35 |
