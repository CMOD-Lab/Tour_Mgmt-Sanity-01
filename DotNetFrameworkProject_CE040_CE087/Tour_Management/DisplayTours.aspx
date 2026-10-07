<%-- 
    MIGRATED: cr-dotnet-1034 - Synchronous Data Binding in GridView Controls
    The synchronous asp:GridView + asp:SqlDataSource data binding pattern (lines 18, 47)
    has been replaced with async Task-based data access using Entity Framework Core
    connected to Amazon RDS, preventing thread pool exhaustion under cloud load.

    The equivalent async functionality is now implemented in:
      - Controllers/TourController.cs   (DisplayTours async GET action using EF Core)
      - Views/Tour/DisplayTours.cshtml  (Razor view replacing this Web Form)

    Original synchronous patterns removed:
      - asp:SqlDataSource (line 18): synchronous DB binding → replaced by async EF Core query
      - asp:GridView (line 47): synchronous data binding → replaced by async Razor table render

    This .aspx file is retained for reference only and is no longer active.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayTours.aspx.cs" Inherits="Tour_Management.DisplayTours" %>
