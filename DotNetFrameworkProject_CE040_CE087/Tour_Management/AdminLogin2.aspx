@page
@model Tour_Management.Pages.AdminLogin2Model
@{
    ViewData["Title"] = "Admin Login";
}

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>@ViewData["Title"]</title>
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

            @if (!string.IsNullOrEmpty(Model?.ErrorMessage))
            {
                <div class="alert alert-danger" style="color:red;">@Model.ErrorMessage</div>
            }

            <label for="name">Email</label><br />
            <input type="email" id="name" name="Email" asp-for="Email" /><br />

            <label for="password">Password</label><br />
            <input type="password" id="password" name="Password" asp-for="Password" /><br />

            <button type="submit" id="Button1">login</button>
        </div>
    </form>
</body>
</html>
