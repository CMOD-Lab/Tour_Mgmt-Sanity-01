<%--
    mybooking.aspx - cr-dotnet-1034: Synchronous Data Binding in GridView Controls
    The synchronous asp:GridView (line 12) and asp:SqlDataSource (line 28) controls
    have been replaced with async Task-based data access using Entity Framework Core
    connected to Amazon RDS, preventing thread pool exhaustion under cloud load.

    This Web Form has been replaced by:
      - Controller: BookingController.MyBookingAsync (GET) - async EF Core data access
      - View: Views/Booking/MyBooking.cshtml - Razor table replacing synchronous GridView
      - Model: Models/MyBookingViewModel

    Original synchronous patterns removed:
      - asp:GridView (line 12): synchronous data binding → replaced by async Razor table
      - asp:SqlDataSource (line 28): synchronous DB binding → replaced by async EF Core query

    This file is retained for backward compatibility only.
    All new requests should use the MVC route: /Booking/MyBooking
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mybooking.aspx.cs" Inherits="Tour_Management.mybooking" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        // Redirect to the ASP.NET MVC equivalent route with async EF Core data binding
        Response.RedirectPermanent("~/Booking/MyBooking");
    }
</script>
