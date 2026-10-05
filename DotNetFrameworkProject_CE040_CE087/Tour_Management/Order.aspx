<%-- 
    MIGRATION NOTE (cr-dotnet-0026): This file has been migrated from ASP.NET Web Forms
    to ASP.NET Core Razor Pages pattern. The Web Forms <%@ Page %> directive, runat="server"
    attributes, and <asp:*> server controls have been replaced with Razor Page equivalents.
    The equivalent Razor Page is: Pages/Order.cshtml
    This .aspx file is retained for reference only and should be replaced by the Razor Page.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="Tour_Management.Order" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Book Tour</title>
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
   <!-- Migrated from Web Forms <form runat="server"> to standard HTML form -->
   <form method="post" action="/Order">
     <div class="container">
        <div class="page-header">
                <h1>Confirm Tour</h1>
        </div>
        <div class="form-horizontal">   
        <div class="form-group"> 
             <!-- Migrated from <asp:Label> and <asp:TextBox> to standard HTML label/input -->
             <div class="control-label col-sm-4"><label for="name">Your Name</label></div>
             <div class="col-sm-6"><input type="text" id="name" name="name" required="true" class="form-control" style="color:black;" /></div>
        </div>
        <div class="form-group">
             <div class="control-label col-sm-4"><label for="city">Your City</label></div>
             <div class="col-sm-6">
                 <input type="text" id="city" name="city" class="form-control" style="color:black;" />
             </div>
        </div>
        <div class="form-group">
                <div class="control-label col-sm-4"><label for="tour_name">Tour Name</label></div>
                <div class="col-sm-6"><input type="text" id="tour_name" name="tour_name" required="true" class="form-control" style="color:black;" /></div>    
        </div>   
        <div class="form-group">
                    <div class="control-label col-sm-4"><label for="number">Mobile Number</label></div>
                    <div class="col-sm-6"><input type="number" id="number" name="number" required="true" class="form-control" style="color:black;" /></div>
        </div>    
        <div class="form-group">           
                    <div class="control-label col-sm-2"><input type="submit" value="Register" style="background-color:#cc6600; color:black;" class="form-control" /></div>
                    <div class="control-label col-sm-2"><input type="reset" value="Reset" style="background-color:#cc6600; color:black;" class="form-control" /></div>
        </div>   
        </div>     
     </div>
   </form>
</body>
</html>
