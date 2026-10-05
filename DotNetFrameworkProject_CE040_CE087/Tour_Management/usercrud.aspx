<%--
    cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core

    Line 11 (original): <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                         AutoGenerateEditButton="True" DataSourceID="SqlDataSource1" ...>
    Fix applied: Replaced synchronous GridView server control with async Razor Page table rendering.

    Line 30 (original): <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                         ConnectionString="<%$ ConnectionStrings:dbconnection %>"
                         SelectCommand="Select top ... * From UserInfo EXCEPT ..."
                         UpdateCommand="UPDATE [UserInfo] Set ...">
    Fix applied: Replaced synchronous SqlDataSource with async EF Core DbContext on Amazon RDS.

    MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): This Web Forms page (usercrud.aspx) has been
    migrated to ASP.NET Core Razor Pages with async data binding.
    - Removed: <asp:GridView AutoGenerateEditButton="True"> synchronous server control (line 11)
    - Removed: <asp:SqlDataSource runat="server"> synchronous data source (line 30)
    - Replaced: synchronous GridView.DataBind() with async Task OnGetAsync() + EF Core ToListAsync()
    - Replaced: SqlDataSource UpdateCommand with async OnPostUpdateAsync() handler
    - Replaced: SqlDataSource ConnectionString binding with environment variable DB_CONNECTION_STRING
    - Prevents thread pool exhaustion under AWS cloud load; enables efficient auto-scaling

    The active implementation is at: Pages/UserCrud.cshtml and Pages/UserCrud.cshtml.cs
    This file is retained for reference only.
--%>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>User Management - Migrated to Async EF Core</title>
</head>
<body>
    <%--
        cr-dotnet-1034 (Line 11): Replaced synchronous <asp:GridView AutoGenerateEditButton="True">
        server control with async Razor Page table rendering in Pages/UserCrud.cshtml.
        Data is now loaded via async Task OnGetAsync() using EF Core ToListAsync() on Amazon RDS.
        Edit/Update action migrated to async OnPostUpdateAsync() handler.
        Original: <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                   AutoGenerateEditButton="True" DataSourceID="SqlDataSource1" ...>
    --%>

    <%--
        cr-dotnet-1034 (Line 30): Replaced synchronous <asp:SqlDataSource> with async EF Core.
        Original: <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                   ConnectionString="<%$ ConnectionStrings:dbconnection %>"
                   SelectCommand="Select top (select COUNT(*) from UserInfo) * From UserInfo
                   EXCEPT Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo"
                   UpdateCommand="UPDATE [UserInfo] Set [Email]=@Email,[FirstName]=@FirstName,
                   [LastName]=@LastName,[Gender]=@Gender,[Password]=@Password,[City]=@City
                   Where [Email]=@Email">
        Replaced by: Entity Framework Core UserCrudDbContext with async ToListAsync()
                     and async SaveChangesAsync() connected to Amazon RDS.
    --%>
</body>
</html>
