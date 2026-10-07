<%-- 
    MIGRATED: This Web Forms page has been migrated to ASP.NET Core Razor Pages.
    The equivalent Razor Page is located at: Pages/UserCrud.cshtml
    This file is retained for reference only and is no longer active.
    
    Migration: ASP.NET Web Forms -> ASP.NET Core Razor Pages (cr-dotnet-0026)
    The <%@ Page %> directive, runat="server" controls, asp:GridView, asp:SqlDataSource,
    and code-behind inheritance from System.Web.UI.Page have been replaced with a Razor Page model.

    cr-dotnet-1034: Synchronous GridView data binding replaced with async Task-based pattern.
    Original synchronous pattern (lines 11, 30 - now removed):
      Line 11: <asp:GridView ID="GridView1" runat="server" ... DataSourceID="SqlDataSource1"
               AutoGenerateEditButton="True" AllowSorting="True" ...>
      Line 30: <asp:SqlDataSource ID="SqlDataSource1" ...
               SelectCommand="Select top (select COUNT(*) from UserInfo) * From UserInfo
                              EXCEPT Select top (...) * From UserInfo"
               UpdateCommand="UPDATE [UserInfo] Set ... Where [Email]=@Email">
    
    Replaced with async Razor Page handlers in Pages/UserCrud.cshtml.cs:
      public async Task OnGetAsync()          -> uses await conn.QueryAsync<UserViewModel>()
      public async Task OnPostEditAsync(...)  -> uses await conn.ExecuteAsync() for UPDATE
    Connected to Amazon RDS via Dapper for cloud-native async data access,
    preventing thread pool exhaustion and enabling efficient auto-scaling.
--%>
