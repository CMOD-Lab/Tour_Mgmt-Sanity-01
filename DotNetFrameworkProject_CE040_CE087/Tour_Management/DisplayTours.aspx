@* cr-dotnet-1034: Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Synchronous GridView/SqlDataSource data binding replaced with async Task-based
   patterns using Entity Framework Core connected to Amazon RDS.
   Original: asp:GridView DataSourceID="SqlDataSource1" (synchronous, line 18 original)
   Original: asp:SqlDataSource SelectCommand="SELECT ... FROM [Tour]" (line 18 original)
   Replaced by: async OnGetAsync() in DisplayTours.aspx.cs using EF Core ToListAsync(). *@
@page
@model Tour_Management.Pages.DisplayToursModel
@{
    ViewData["Title"] = "Display Tours";
}

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Display Tours</title>
    <style type="text/css">
        table {
            border-collapse: collapse;
            width: 875px;
            margin-right: 130px;
            margin-top: 21px;
            margin-left: 13px;
        }

        table th {
            background-color: #6B696B;
            color: white;
            font-weight: bold;
            padding: 8px;
        }

        table td {
            background-color: #F7F7DE;
            padding: 8px;
            text-align: center;
            border: 1px solid #DEDFDE;
        }

        table tr:nth-child(even) td {
            background-color: white;
        }
    </style>
</head>
<body>
    <form method="get">
        &nbsp;&nbsp;&nbsp;
        @* cr-dotnet-1034: HTML table replaces asp:GridView — data bound via async EF Core query (line 47 original) *@
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
