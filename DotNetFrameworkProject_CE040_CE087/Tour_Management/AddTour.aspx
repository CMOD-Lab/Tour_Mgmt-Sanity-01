@page
@model Tour_Management.Pages.AddTourModel
@{
    ViewData["Title"] = "Add New Tour";
}

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>@ViewData["Title"]</title>
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
                            <label for="tour_name">Name of Tour</label>
                        </div>
                        <div class="col-sm-6">
                            <input type="text" id="tour_name" name="TourName" required="true"
                                   class="form-control" style="color:black;" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="place">Place</label>
                        </div>
                        <div class="col-sm-6">
                            <input type="text" id="place" name="Place" required="true"
                                   class="form-control" style="color:black;" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="days">Days</label>
                        </div>
                        <div class="col-sm-6">
                            <input type="text" id="days" name="Days" required="true"
                                   class="form-control" style="color:black;" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="locations">Locations</label>
                        </div>
                        <div class="col-sm-6">
                            <input type="text" id="locations" name="Locations" required="true"
                                   class="form-control" style="color:black;" />
                        </div>
                    </div>

                    <div class="col-md-5">
                        <p style="text-align:center; font-size:30px;">Image for Tour</p>
                        <input type="file" id="FileUpload1" name="TourImage" />
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="price">Price</label>
                        </div>
                        <div class="col-sm-6">
                            <input type="text" id="price" name="Price" required="true"
                                   class="form-control" style="color:black;" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <label for="tour_info">Tour Info</label>
                        </div>
                        <div class="col-sm-6">
                            <textarea id="tour_info" name="TourInfo" required="true"
                                      class="form-control" style="color:black;"
                                      maxlength="250"></textarea>
                        </div>
                    </div>
                    <span asp-validation-for="TourInfo" class="text-danger"></span>

                    @if (!string.IsNullOrEmpty(Model?.StatusMessage))
                    {
                        <div class="alert alert-info">@Model.StatusMessage</div>
                    }

                    <div class="form-group">
                        <div class="control-label col-sm-3">
                            <button type="submit" style="background-color:#cc6600; color:black;"
                                    class="btn">Register</button>
                        </div>
                        <div class="control-label col-sm-3">
                            <button type="reset" style="background-color:#cc6600; color:black;"
                                    class="btn">Reset</button>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </form>
</body>
</html>
