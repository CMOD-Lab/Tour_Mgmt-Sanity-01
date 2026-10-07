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
    <div class="container">
        <h1>Admin Login</h1>
        <form method="post">
            @Html.AntiForgeryToken()
            @if (!string.IsNullOrEmpty(Model.ErrorMessage))
            {
                <div class="alert alert-danger">@Model.ErrorMessage</div>
            }
            <label for="name">Email</label><br />
            <input type="email" id="name" name="Email" /><br />
            <label for="password">Password</label><br />
            <input type="password" id="password" name="Password" /><br />
            <button type="submit">Login</button>
        </form>
    </div>
</body>
</html>
