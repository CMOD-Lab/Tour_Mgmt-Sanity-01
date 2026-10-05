// Tour model class - used by DisplayToursModel Razor Page
// cr-dotnet-1034: Updated for Entity Framework Core async data binding on Amazon RDS
// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tour_Management.Models
{
    // cr-dotnet-1034: EF Core entity mapped to Tour table on Amazon RDS
    [Table("Tour")]
    public class Tour
    {
        [Key]
        [Column("TOUR_ID")]
        public int TourId { get; set; }

        [Column("TOUR_NAME")]
        public string TourName { get; set; }

        [Column("pic")]
        public string Pic { get; set; }

        [Column("PRICE")]
        public decimal Price { get; set; }

        [Column("DAYS")]
        public int Days { get; set; }

        [Column("LOCATIONS")]
        public string Locations { get; set; }
    }
}
