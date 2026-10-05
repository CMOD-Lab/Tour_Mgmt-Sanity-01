@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026) *@
@page "/AddTour"
@model Tour_Management.Pages.AddTourModel
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add New Tour</title>
    <style type="text/css">
        .page-header {
            text-align: center;
        }
        .form-horizontal {
            font-size: 30px;
            text-align: center;
        }
        .row .form-horizontal {
            border-right: 1px double;
        }
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
    <form method="post" enctype="multipart/form-data">
        <div class="page-header">
            <h1>Add New Tour</h1>
        </div>
        <div class="container">
            <div class="row">
                <div class="form-horizontal col-md-7">

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label asp-for="TourName">Name of Tour</label>
                        </div>
                        <div class="col-sm-6">
                            <input asp-for="TourName" required="true" style="color:black;" class="form-control" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label asp-for="Place">Place</label>
                        </div>
                        <div class="col-sm-6">
                            <input asp-for="Place" required="true" style="color:black;" class="form-control" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label asp-for="Days">Days</label>
                        </div>
                        <div class="col-sm-6">
                            <input asp-for="Days" required="true" style="color:black;" class="form-control" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label asp-for="Locations">Locations</label>
                        </div>
                        <div class="col-sm-6">
                            <input asp-for="Locations" required="true" style="color:black;" class="form-control" />
                        </div>
                    </div>

                    <div class="col-md-5">
                        <p style="text-align:center; font-size:30px;">Image for Tour</p>
                        <input type="file" name="TourImage" id="TourImage" />
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label asp-for="Price">Price</label>
                        </div>
                        <div class="col-sm-6">
                            <input asp-for="Price" required="true" style="color:black;" class="form-control" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label asp-for="TourInfo">Tour Info</label>
                        </div>
                        <div class="col-sm-6">
                            <textarea asp-for="TourInfo" required="true" style="color:black;" class="form-control" maxlength="250"></textarea>
                        </div>
                    </div>

                    <span asp-validation-for="TourInfo" class="text-danger"></span>

                    @if (!string.IsNullOrEmpty(Model.StatusMessage))
                    {
                        <div class="alert">@Model.StatusMessage</div>
                    }

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <button type="submit" style="background-color:#cc6600; color:black;" class="btn">Register</button>
                        </div>
                        <div class="control-label col-sm-3">
                            <button type="reset" style="background-color:#cc6600; color:black;" class="btn">Reset</button>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </form>
</body>
</html>
