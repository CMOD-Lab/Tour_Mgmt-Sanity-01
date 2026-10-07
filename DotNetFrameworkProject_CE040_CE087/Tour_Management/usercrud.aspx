@* MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
   This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   - Removed: <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="usercrud.aspx.cs" Inherits="Tour_Management.usercrud" %> directive (line 1)
   - Removed: <head runat="server">, <form id="form1" runat="server"> Web Forms server-side attributes
   - Removed: <asp:GridView> data-bound server control (line 11) — replaced with Razor @foreach HTML table
     cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async Task-based EF Core OnGetAsync()
   - Removed: <asp:SqlDataSource> declarative data source (line 30) with ConnectionString="<%$ ConnectionStrings:dbconnection %>"
     cr-dotnet-1034: SqlDataSource synchronous data binding replaced with async EF Core queries via Amazon RDS
   - Removed: <asp:BoundField> column definitions
   - Removed: AutoGenerateEditButton="True", DataKeyNames="Email", DataSourceID="SqlDataSource1" GridView bindings
   - Replaced: <asp:GridView> with standard HTML <table> rendered via @foreach over @Model.Users
   - Replaced: <asp:SqlDataSource> SelectCommand with OnGetAsync() EF Core query populating @Model.Users
   - Replaced: <asp:SqlDataSource> UpdateCommand with OnPostUpdateAsync() Razor Page handler
   - cr-dotnet-1034: All data access converted to async EF Core patterns for Amazon RDS,
     preventing thread pool exhaustion under cloud load and enabling efficient auto-scaling.
*@
@page
@model Tour_Management.Pages.UserCrudModel
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>User Management</title>
</head>
<body>
    @* Replaces <form id="form1" runat="server"> Web Forms postback form *@
    <div>
        @* Replaces <asp:GridView> (line 11) with a standard HTML table rendered via Razor @foreach *@
        @* cr-dotnet-1034: Data now loaded asynchronously via EF Core OnGetAsync() *@
        <table style="background-color:white; border-color:#DEDFDE; border-style:None; border-width:1px; color:black; border-collapse:collapse;">
            <thead>
                <tr style="background-color:#6B696B; color:white; font-weight:bold;">
                    @* Replaces <asp:BoundField DataField="Email" HeaderText="Email" ReadOnly="True" /> *@
                    <th style="padding:4px;">Email</th>
                    @* Replaces <asp:BoundField DataField="FirstName" HeaderText="FirstName" /> *@
                    <th style="padding:4px;">FirstName</th>
                    @* Replaces <asp:BoundField DataField="LastName" HeaderText="LastName" /> *@
                    <th style="padding:4px;">LastName</th>
                    @* Replaces <asp:BoundField DataField="Gender" HeaderText="Gender" /> *@
                    <th style="padding:4px;">Gender</th>
                    @* Replaces <asp:BoundField DataField="Password" HeaderText="Password" /> *@
                    <th style="padding:4px;">Password</th>
                    @* Replaces <asp:BoundField DataField="City" HeaderText="City" /> *@
                    <th style="padding:4px;">City</th>
                    @* Replaces AutoGenerateEditButton="True" *@
                    <th style="padding:4px;">Actions</th>
                </tr>
            </thead>
            <tbody>
                @* Replaces <asp:GridView> row rendering — iterates over @Model.Users populated via async EF Core query *@
                @* cr-dotnet-1034: Replaces <asp:SqlDataSource> (line 30) synchronous data binding *@
                @if (Model.Users != null)
                {
                    foreach (var user in Model.Users)
                    {
                        <tr style="background-color:#F7F7DE; text-align:center;">
                            <td style="padding:4px;">@user.Email</td>
                            <td style="padding:4px;">
                                @* Replaces AutoGenerateEditButton — inline edit form *@
                                <form method="post" asp-page-handler="Update" style="display:inline;">
                                    <input type="hidden" name="email" value="@user.Email" />
                                    <input type="text" name="firstName" value="@user.FirstName" style="width:100px;" />
                                    <input type="text" name="lastName" value="@user.LastName" style="width:100px;" />
                                    <input type="text" name="gender" value="@user.Gender" style="width:70px;" />
                                    <input type="text" name="password" value="@user.Password" style="width:100px;" />
                                    <input type="text" name="city" value="@user.City" style="width:100px;" />
                                    <button type="submit" style="margin:2px; padding:4px 8px; cursor:pointer;">Update</button>
                                </form>
                            </td>
                            <td style="padding:4px;">@user.LastName</td>
                            <td style="padding:4px;">@user.Gender</td>
                            <td style="padding:4px;">@user.Password</td>
                            <td style="padding:4px;">@user.City</td>
                            <td style="padding:4px;"></td>
                        </tr>
                    }
                }
            </tbody>
        </table>
    </div>
</body>
</html>
