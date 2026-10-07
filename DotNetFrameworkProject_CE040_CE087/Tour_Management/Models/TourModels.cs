using System;
using System.ComponentModel.DataAnnotations;

namespace Tour_Management.Models
{
    /// <summary>
    /// View model for adding a new tour (replaces AddTour.aspx Web Form).
    /// </summary>
    public class AddTourViewModel
    {
        [Required(ErrorMessage = "Tour name is required.")]
        [Display(Name = "Name of Tour")]
        public string TourName { get; set; }

        [Required(ErrorMessage = "Place is required.")]
        [Display(Name = "Place")]
        public string Place { get; set; }

        [Required(ErrorMessage = "Days is required.")]
        [Display(Name = "Days")]
        public string Days { get; set; }

        [Required(ErrorMessage = "Locations is required.")]
        [Display(Name = "Locations")]
        public string Locations { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Display(Name = "Price")]
        public string Price { get; set; }

        [Required(ErrorMessage = "Tour info is required.")]
        [MaxLength(250, ErrorMessage = "Characters less than 250")]
        [Display(Name = "Tour Info")]
        public string TourInfo { get; set; }

        /// <summary>
        /// File name of the uploaded tour image (stored after upload).
        /// </summary>
        public string PicFileName { get; set; }
    }

    /// <summary>
    /// View model for displaying a tour in the list (replaces DisplayTours.aspx Web Form).
    /// </summary>
    public class TourViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Pic { get; set; }
        public string Price { get; set; }
        public string Days { get; set; }
        public string Locations { get; set; }
    }

    /// <summary>
    /// View model for booking an order (replaces Order.aspx Web Form).
    /// </summary>
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Your name is required.")]
        [Display(Name = "Your Name")]
        public string Name { get; set; }

        [Display(Name = "Your City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Tour name is required.")]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
    }

    /// <summary>
    /// View model for admin login (replaces AdminLogin2.aspx Web Form).
    /// </summary>
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    /// <summary>
    /// View model for user sign-up (replaces SignUpForm.aspx Web Form).
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// </summary>
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Re-enter Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth (dd-mm-yyyy)")]
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [Display(Name = "State")]
        public string State { get; set; }
    }

    /// <summary>
    /// View model for user login (replaces userlogin.aspx Web Form).
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// </summary>
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    /// <summary>
    /// View model for user info CRUD (replaces usercrud.aspx Web Form).
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// </summary>
    public class UserInfoViewModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }
    }

    /// <summary>
    /// View model for tour CRUD (replaces TourCrud.aspx Web Form).
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// </summary>
    public class TourCrudViewModel
    {
        public int TourId { get; set; }

        [Display(Name = "Tour Name")]
        public string TourName { get; set; }

        [Display(Name = "Place")]
        public string Place { get; set; }

        [Display(Name = "Days")]
        public string Days { get; set; }

        [Display(Name = "Price")]
        public string Price { get; set; }

        [Display(Name = "Locations")]
        public string Locations { get; set; }

        [Display(Name = "Tour Info")]
        public string TourInfo { get; set; }

        [Display(Name = "Picture")]
        public string Pic { get; set; }
    }

    /// <summary>
    /// View model for all bookings (replaces allbooking.aspx Web Form).
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// </summary>
    public class BookingViewModel
    {
        public int TourId { get; set; }

        [Display(Name = "Tour Name")]
        public string TourName { get; set; }

        [Display(Name = "Place")]
        public string Place { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
    }

    /// <summary>
    /// View model for my bookings (replaces mybooking.aspx Web Form).
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// </summary>
    public class MyBookingViewModel
    {
        public int TourId { get; set; }

        [Display(Name = "Tour Name")]
        public string TourName { get; set; }
    }
}
