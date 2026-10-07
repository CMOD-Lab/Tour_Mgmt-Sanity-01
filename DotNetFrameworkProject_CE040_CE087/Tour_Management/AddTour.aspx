@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026) *@
@page
@model Tour_Management.Pages.AddTourModel
@{
    ViewData["Title"] = "Add New Tour";
}

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
        @Html.AntiForgeryToken()
        <div class="page-header">
            <h1>Add New Tour</h1>
        </div>
        <div class="container">
            <div class="row">
                <div class="form-horizontal col-md-7">

                    @if (!string.IsNullOrEmpty(Model.StatusMessage))
                    {
                        <div class="alert">@Model.StatusMessage</div>
                    }

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="TourName">Name of Tour</label>
                        </div>
                        <div class="col-sm-6">
                            <input id="TourName" name="TourName" type="text" required="true" class="form-control"
                                   value="@Model.TourName" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="Place">Place</label>
                        </div>
                        <div class="col-sm-6">
                            <input id="Place" name="Place" type="text" required="true" class="form-control"
                                   value="@Model.Place" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="Days">Days</label>
                        </div>
                        <div class="col-sm-6">
                            <input id="Days" name="Days" type="text" required="true" class="form-control"
                                   value="@Model.Days" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="Locations">Locations</label>
                        </div>
                        <div class="col-sm-6">
                            <input id="Locations" name="Locations" type="text" required="true" class="form-control"
                                   value="@Model.Locations" />
                        </div>
                    </div>

                    <div class="col-md-5">
                        <p style="text-align:center; font-size:30px;">Image for Tour</p>
                        <input type="file" id="TourImage" name="TourImage" />
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="Price">Price</label>
                        </div>
                        <div class="col-sm-6">
                            <input id="Price" name="Price" type="text" required="true" class="form-control"
                                   value="@Model.Price" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="TourInfo">Tour Info</label>
                        </div>
                        <div class="col-sm-6">
                            <textarea id="TourInfo" name="TourInfo" required="true" class="form-control"
                                      maxlength="250">@Model.TourInfo</textarea>
                        </div>
                    </div>
                    <span asp-validation-for="TourInfo" class="text-danger"></span>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <button type="submit" style="background-color:#cc6600; color:black;">Register</button>
                        </div>
                        <div class="control-label col-sm-3">
                            <button type="reset" style="background-color:#cc6600; color:black;">Reset</button>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </form>
</body>
</html>
