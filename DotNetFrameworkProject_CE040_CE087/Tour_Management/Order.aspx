@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026) *@
@page
@model Tour_Management.Pages.OrderModel
@{
    ViewData["Title"] = "Confirm Tour";
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
   <div class="container">
        <div class="page-header">
                <h1>Confirm Tour</h1>
        </div>
        @if (!string.IsNullOrEmpty(Model.StatusMessage))
        {
            <div class="alert">@Model.StatusMessage</div>
        }
        <form method="post">
            @Html.AntiForgeryToken()
            <div class="form-horizontal">   
                <div class="form-group"> 
                     <div class="control-label col-sm-4"><label asp-for="BookingInput.FirstName">Your Name</label></div>
                     <div class="col-sm-6"><input asp-for="BookingInput.FirstName" required="true" class="form-control" style="color:black;" /></div>
                </div>
                <div class="form-group">
                     <div class="control-label col-sm-4"><label asp-for="BookingInput.Place">Your City</label></div>
                     <div class="col-sm-6">
                         <input asp-for="BookingInput.Place" class="form-control" style="color:black;" />
                     </div>
                </div>
                <div class="form-group">
                        <div class="control-label col-sm-4"><label asp-for="BookingInput.TourName">Tour Name</label></div>
                         <div class="col-sm-6"><input asp-for="BookingInput.TourName" required="true" class="form-control" style="color:black;" /></div>    
                </div>    
                <div class="form-group">
                            <div class="control-label col-sm-4"><label asp-for="BookingInput.Email">Mobile Number</label></div>
                            <div class="col-sm-6"><input asp-for="BookingInput.Email" required="true" class="form-control" style="color:black;" type="number" /></div>
                </div>    
                <div class="form-group">           
                            <div class="control-label col-sm-2"><button type="submit" style="background-color:#cc6600; color:black;" class="form-control">Register</button></div>
                            <div class="control-label col-sm-2"><button type="reset" style="background-color:#cc6600; color:black;" class="form-control">Reset</button></div>
                </div>   
            </div>     
        </form>
   </div>
</body>
</html>
