@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026) *@
@page "/AdminLogin2"
@model Tour_Management.Pages.AdminLogin2Model
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Admin Login</title>
    <style>
        .container {
            text-align: center;
            background-color: black;
            width: 100%;
            font-size: 30px;
            color: white;
            padding-bottom: 150px;
            opacity: 0.8;
        }
    </style>
</head>
<body>
    <form method="post">
        <div class="container">
            <h1>Admin Login</h1>
            <label asp-for="Email">Email</label><br />
            <input asp-for="Email" type="text" /><br />
            <label asp-for="Password">password</label><br />
            <input asp-for="Password" type="password" /><br />

            @if (!string.IsNullOrEmpty(Model.ErrorMessage))
            {
                <span style="color:red;">@Model.ErrorMessage</span><br />
            }

            <button type="submit">login</button>
        </div>
    </form>
</body>
</html>
