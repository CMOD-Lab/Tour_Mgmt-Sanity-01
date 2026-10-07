@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026) *@
@page
@model Tour_Management.Pages.OrderModel
@{
    ViewData["Title"] = "Confirm Tour";
}

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Confirm Tour</title>
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
        <div class="page-header">
            <h1>Confirm Tour</h1>
        </div>
        <div class="form-horizontal">
            <form method="post">
                @Html.AntiForgeryToken()
                @if (!string.IsNullOrEmpty(Model.StatusMessage))
                {
                    <div class="alert">@Model.StatusMessage</div>
                }
                <div class="form-group">
                    <div class="control-label col-sm-4"><label for="name">Your Name</label></div>
                    <div class="col-sm-6">
                        <input type="text" id="name" name="Name" required="true" class="form-control" style="color:black;" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="control-label col-sm-4"><label for="city">Your City</label></div>
                    <div class="col-sm-6">
                        <input type="text" id="city" name="City" class="form-control" style="color:black;" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="control-label col-sm-4"><label for="tour_name">Tour Name</label></div>
                    <div class="col-sm-6">
                        <input type="text" id="tour_name" name="TourName" required="true" class="form-control" style="color:black;" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="control-label col-sm-4"><label for="number">Mobile Number</label></div>
                    <div class="col-sm-6">
                        <input type="number" id="number" name="Number" required="true" class="form-control" style="color:black;" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="control-label col-sm-2">
                        <button type="submit" style="background-color:#cc6600; color:black;" class="form-control">Register</button>
                    </div>
                    <div class="control-label col-sm-2">
                        <button type="reset" style="background-color:#cc6600; color:black;" class="form-control">Reset</button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</body>
</html>
