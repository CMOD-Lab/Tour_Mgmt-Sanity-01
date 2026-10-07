@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026) *@
@page
@model Tour_Management.Pages.AdminLogin2Model
@{
    ViewData["Title"] = "Admin Login";
}

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
        @Html.AntiForgeryToken()
        <div class="container">
            <h1>Admin Login</h1>

            @if (!string.IsNullOrEmpty(Model.ErrorMessage))
            {
                <div class="alert alert-danger">@Model.ErrorMessage</div>
            }

            <label for="Email">Email</label><br />
            <input id="Email" name="Email" type="email" value="@Model.Email" /><br />
            <label for="Password">Password</label><br />
            <input id="Password" name="Password" type="password" /><br />
            <button type="submit">Login</button>
        </div>
    </form>
</body>
</html>
