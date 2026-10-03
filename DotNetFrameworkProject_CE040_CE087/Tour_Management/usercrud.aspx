<%-- 
    MIGRATED TO ASP.NET CORE RAZOR PAGES
    This Web Forms page (usercrud.aspx) has been migrated to ASP.NET Core Razor Pages.
    The new implementation is located at: usercrud.cshtml / usercrud.cshtml.cs

    Migration performed as part of cloud readiness remediation (Rule: cr-dotnet-1034).
    The Web Forms Page directive, server controls (asp:GridView, asp:SqlDataSource,
    asp:BoundField), and code-behind inheritance from System.Web.UI.Page have been
    replaced with a stateless Razor Page model following cloud-native patterns.

    The synchronous asp:GridView data binding (AutoGenerateEditButton="True",
    DataSourceID="SqlDataSource1") and asp:SqlDataSource commands have been replaced
    with async Task-based data retrieval using Entity Framework Core connected to
    Amazon RDS, preventing thread pool exhaustion under load and enabling efficient
    auto-scaling in cloud deployments.

    Original synchronous controls replaced:
      Line 11: <asp:GridView ID="GridView1" ... AutoGenerateEditButton="True"
               DataSourceID="SqlDataSource1" ...>
               → Replaced with async EF Core query in UserCrudModel.OnGetAsync()
                 and update handler UserCrudModel.OnPostUpdateAsync() (usercrud.cshtml.cs)

      Line 30: <asp:SqlDataSource ID="SqlDataSource1" ...
               SelectCommand="Select top (select COUNT(*) from UserInfo) * From UserInfo
               EXCEPT Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo"
               UpdateCommand="UPDATE [UserInfo] Set [Email]=@Email,[FirstName]=@FirstName,
               [LastName]=@LastName,[Gender]=@Gender,[Password]=@Password,[City]=@City
               Where [Email]=@Email">
               → Replaced with async EF Core DbContext operations in usercrud.cshtml.cs
--%>
