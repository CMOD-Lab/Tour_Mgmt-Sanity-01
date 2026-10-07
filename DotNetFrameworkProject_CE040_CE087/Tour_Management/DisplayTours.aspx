@page
@model Tour_Management.Pages.DisplayToursModel
@{
    ViewData["Title"] = "Display Tours";
    Layout = null;
}

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Display Tours</title>
    <style type="text/css">
        table {
            width: 875px;
            margin-right: 130px;
            margin-top: 21px;
            margin-left: 13px;
            border-collapse: collapse;
            background-color: #F7F7DE;
            border: 1px solid #DEDFDE;
            color: black;
        }
        table th {
            background-color: #6B696B;
            color: white;
            font-weight: bold;
            padding: 4px;
        }
        table td {
            padding: 4px;
            text-align: center;
            border: 1px solid #DEDFDE;
        }
        table tr:nth-child(even) {
            background-color: white;
        }
    </style>
</head>
<body>
    <form method="get">
        &nbsp;&nbsp;&nbsp;
        <table>
            <thead>
                <tr>
                    <th>TOUR_NAME</th>
                    <th>DAYS</th>
                    <th>LOCATIONS</th>
                    <th>TOUR_ID</th>
                    <th>pic</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                @foreach (var tour in Model.Tours)
                {
                    <tr>
                        <td>@tour.TourName</td>
                        <td>@tour.Days</td>
                        <td>@tour.Locations</td>
                        <td>@tour.TourId</td>
                        <td><img src="Tour_pics/@tour.Pic" style="width:200px;height:200px" /></td>
                        <td><a href="/Order">Book Now</a></td>
                    </tr>
                }
            </tbody>
        </table>
        &nbsp;
    </form>
</body>
</html>
