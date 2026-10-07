using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tour_Management.Data
{
    /// <summary>
    /// EF entity for the Tour table.
    /// cr-dotnet-1034: Replaces synchronous SqlDataSource + GridView binding with
    /// async EF DbSet queries in TourController and BookingController.
    /// </summary>
    [Table("Tour")]
    public class TourEntity
    {
        [Key]
        [Column("TOUR_ID")]
        public int TourId { get; set; }

        [Column("TOUR_NAME")]
        public string TourName { get; set; }

        [Column("PLACE")]
        public string Place { get; set; }

        [Column("DAYS")]
        public string Days { get; set; }

        [Column("PRICE")]
        public string Price { get; set; }

        [Column("LOCATIONS")]
        public string Locations { get; set; }

        [Column("TOUR_INFO")]
        public string TourInfo { get; set; }

        [Column("pic")]
        public string Pic { get; set; }
    }

    /// <summary>
    /// EF entity for the booking table.
    /// cr-dotnet-1034: Replaces synchronous SqlDataSource + GridView binding with
    /// async EF DbSet queries in BookingController.
    /// </summary>
    [Table("booking")]
    public class BookingEntity
    {
        [Key]
        [Column("TOUR_ID")]
        public int TourId { get; set; }

        [Column("TOUR_NAME")]
        public string TourName { get; set; }

        [Column("PLACE")]
        public string Place { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }
    }

    /// <summary>
    /// EF entity for the UserInfo table.
    /// cr-dotnet-1034: Replaces synchronous SqlDataSource + GridView binding with
    /// async EF DbSet queries in UserController.
    /// </summary>
    [Table("UserInfo")]
    public class UserInfoEntity
    {
        [Key]
        [Column("Email")]
        public string Email { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }

        [Column("Gender")]
        public string Gender { get; set; }

        [Column("Password")]
        public string Password { get; set; }

        [Column("dob")]
        public string DateOfBirth { get; set; }

        [Column("Street")]
        public string Street { get; set; }

        [Column("City")]
        public string City { get; set; }

        [Column("State")]
        public string State { get; set; }
    }
}
