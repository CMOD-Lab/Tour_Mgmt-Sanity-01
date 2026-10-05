@* cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
   Replaced synchronous GridView/SqlDataSource data binding (lines 18, 47) with async
   Task-based Razor Page pattern using Entity Framework Core connected to Amazon RDS.
   - Line 18 (original GridView control): Replaced with async Razor foreach table rendering
   - Line 47 (original SqlDataSource control): Replaced with async EF Core OnGetAsync()
   - Prevents thread pool exhaustion under cloud load; enables efficient AWS auto-scaling
*@
@page
@model Tour_Management.Pages.DisplayToursModel
@using Tour_Management.Models
@{
    ViewData["Title"] = "Display Tours";
}

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Display Tours</title>
    <style type="text/css">
        .tours-list li {
            display: grid;
        }

        table {
            width: 875px;
            margin-right: 130px;
            margin-top: 21px;
            margin-left: 13px;
            border-collapse: collapse;
            background-color: white;
            border: 1px solid #DEDFDE;
            color: black;
        }

        th {
            background-color: #6B696B;
            color: white;
            font-weight: bold;
            padding: 4px;
        }

        td {
            padding: 4px;
            background-color: #F7F7DE;
            text-align: center;
        }

        tr:nth-child(even) td {
            background-color: white;
        }
    </style>
</head>
<body>
    <form method="get">
        &nbsp;&nbsp;&nbsp;
        @*
            cr-dotnet-1034 (Line 18): Replaced <asp:GridView runat="server"> synchronous server control
            with async Razor foreach table. Data is loaded via async Task OnGetAsync() in the PageModel
            using Entity Framework Core ToListAsync() connected to Amazon RDS.
        *@
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
                @*
                    cr-dotnet-1034 (Line 47): Replaced <asp:SqlDataSource> synchronous data binding
                    with async EF Core data loaded in OnGetAsync() and rendered here via Razor foreach.
                *@
                @if (Model.Tours != null)
                {
                    @foreach (var tour in Model.Tours)
                    {
                        <tr>
                            <td>@tour.TourName</td>
                            <td>@tour.Days</td>
                            <td>@tour.Locations</td>
                            <td>@tour.TourId</td>
                            <td>
                                <img src="Tour_pics/@tour.Pic" style="width:200px;height:200px" alt="@tour.TourName" />
                            </td>
                            <td>
                                <a href="/Order?tourId=@tour.TourId">Book Now</a>
                            </td>
                        </tr>
                    }
                }
            </tbody>
        </table>
        &nbsp;
    </form>
</body>
</html>
