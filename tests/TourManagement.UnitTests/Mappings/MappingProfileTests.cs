using AutoMapper;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.Mappings;

/// <summary>
/// Unit tests for AutoMapper MappingProfile.
/// </summary>
public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    // ─── Configuration Validity ───────────────────────────────────────────────

    [Fact]
    public void MappingProfile_ConfigurationShouldBeValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        config.AssertConfigurationIsValid();
    }

    // ─── UserInfo → UserDto ───────────────────────────────────────────────────

    [Fact]
    public void Map_UserInfo_To_UserDto_ShouldMapAllFields()
    {
        var user = new UserInfo
        {
            Email = "alice@test.com",
            FirstName = "Alice",
            LastName = "Smith",
            Gender = "Female",
            Password = "hashed",
            Dob = new DateTime(1990, 1, 15),
            Street = "1 Main St",
            City = "New York",
            State = "NY"
        };

        var dto = _mapper.Map<UserDto>(user);

        dto.Email.Should().Be("alice@test.com");
        dto.FirstName.Should().Be("Alice");
        dto.LastName.Should().Be("Smith");
        dto.Gender.Should().Be("Female");
        dto.Dob.Should().Be(new DateTime(1990, 1, 15));
        dto.Street.Should().Be("1 Main St");
        dto.City.Should().Be("New York");
        dto.State.Should().Be("NY");
    }

    // ─── UserCreateDto → UserInfo ─────────────────────────────────────────────

    [Fact]
    public void Map_UserCreateDto_To_UserInfo_ShouldMapAllFields()
    {
        var dto = new UserCreateDto
        {
            Email = "bob@test.com",
            FirstName = "Bob",
            LastName = "Jones",
            Gender = "Male",
            Password = "plaintext",
            Dob = new DateTime(1985, 5, 20),
            Street = "2 Oak Ave",
            City = "Los Angeles",
            State = "CA"
        };

        var user = _mapper.Map<UserInfo>(dto);

        user.Email.Should().Be("bob@test.com");
        user.FirstName.Should().Be("Bob");
        user.LastName.Should().Be("Jones");
        user.Gender.Should().Be("Male");
        user.Dob.Should().Be(new DateTime(1985, 5, 20));
        user.Street.Should().Be("2 Oak Ave");
        user.City.Should().Be("Los Angeles");
        user.State.Should().Be("CA");
        user.Bookings.Should().BeEmpty();
    }

    // ─── UserUpdateDto → UserInfo ─────────────────────────────────────────────

    [Fact]
    public void Map_UserUpdateDto_To_UserInfo_ShouldMapAllFields()
    {
        var existing = new UserInfo { Email = "alice@test.com", Password = "existingHash" };
        var dto = new UserUpdateDto
        {
            FirstName = "Alicia",
            LastName = "Smith",
            Gender = "Female",
            Dob = new DateTime(1990, 1, 15),
            Street = "1 Main St",
            City = "NY",
            State = "NY"
        };

        _mapper.Map(dto, existing);

        existing.Email.Should().Be("alice@test.com"); // Email should not change
        existing.FirstName.Should().Be("Alicia");
        existing.LastName.Should().Be("Smith");
    }

    // ─── Tour → TourDto ───────────────────────────────────────────────────────

    [Fact]
    public void Map_Tour_To_TourDto_ShouldMapAllFields()
    {
        var tour = new Tour
        {
            TourId = 1,
            TourName = "Kashmir Tour",
            Place = "Kashmir",
            Days = 7,
            Price = 15000m,
            Locations = "Srinagar, Gulmarg",
            TourInfo = "Beautiful Kashmir",
            Pic = "kashmir.jpg",
            IsActive = true,
            CreatedDate = new DateTime(2024, 1, 1)
        };

        var dto = _mapper.Map<TourDto>(tour);

        dto.TourId.Should().Be(1);
        dto.TourName.Should().Be("Kashmir Tour");
        dto.Place.Should().Be("Kashmir");
        dto.Days.Should().Be(7);
        dto.Price.Should().Be(15000m);
        dto.Locations.Should().Be("Srinagar, Gulmarg");
        dto.TourInfo.Should().Be("Beautiful Kashmir");
        dto.Pic.Should().Be("kashmir.jpg");
        dto.IsActive.Should().BeTrue();
        dto.CreatedDate.Should().Be(new DateTime(2024, 1, 1));
    }

    // ─── TourCreateDto → Tour ─────────────────────────────────────────────────

    [Fact]
    public void Map_TourCreateDto_To_Tour_ShouldSetIsActiveTrue()
    {
        var dto = new TourCreateDto { TourName = "New Tour", Place = "P", Days = 3, Price = 5000, Locations = "L", TourInfo = "I" };

        var tour = _mapper.Map<Tour>(dto);

        tour.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Map_TourCreateDto_To_Tour_ShouldIgnoreTourId()
    {
        var dto = new TourCreateDto { TourName = "New Tour", Place = "P", Days = 3, Price = 5000, Locations = "L", TourInfo = "I" };

        var tour = _mapper.Map<Tour>(dto);

        tour.TourId.Should().Be(0);
    }

    [Fact]
    public void Map_TourCreateDto_To_Tour_ShouldMapAllFields()
    {
        var dto = new TourCreateDto
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Days = 5,
            Price = 12000m,
            Locations = "North Goa, South Goa",
            TourInfo = "Beach holiday",
            Pic = "goa.jpg"
        };

        var tour = _mapper.Map<Tour>(dto);

        tour.TourName.Should().Be("Goa Tour");
        tour.Place.Should().Be("Goa");
        tour.Days.Should().Be(5);
        tour.Price.Should().Be(12000m);
        tour.Locations.Should().Be("North Goa, South Goa");
        tour.TourInfo.Should().Be("Beach holiday");
        tour.Pic.Should().Be("goa.jpg");
    }

    // ─── TourUpdateDto → Tour ─────────────────────────────────────────────────

    [Fact]
    public void Map_TourUpdateDto_To_Tour_ShouldNotChangeTourId()
    {
        var existing = new Tour { TourId = 5, TourName = "Old", Place = "P", Days = 1, Price = 100, Locations = "L", TourInfo = "I" };
        var dto = new TourUpdateDto { TourName = "New", Place = "P2", Days = 2, Price = 200, Locations = "L2", TourInfo = "I2", IsActive = true };

        _mapper.Map(dto, existing);

        existing.TourId.Should().Be(5);
        existing.TourName.Should().Be("New");
    }

    // ─── Booking → BookingDto ─────────────────────────────────────────────────

    [Fact]
    public void Map_Booking_To_BookingDto_ShouldMapAllFields()
    {
        var booking = new Booking
        {
            BookingId = 1,
            TourName = "Kerala Tour",
            Place = "Kerala",
            Email = "user@test.com",
            FirstName = "Priya",
            BookingDate = new DateTime(2024, 8, 10),
            IsActive = true
        };

        var dto = _mapper.Map<BookingDto>(booking);

        dto.BookingId.Should().Be(1);
        dto.TourName.Should().Be("Kerala Tour");
        dto.Place.Should().Be("Kerala");
        dto.Email.Should().Be("user@test.com");
        dto.FirstName.Should().Be("Priya");
        dto.BookingDate.Should().Be(new DateTime(2024, 8, 10));
        dto.IsActive.Should().BeTrue();
    }

    // ─── BookingCreateDto → Booking ───────────────────────────────────────────

    [Fact]
    public void Map_BookingCreateDto_To_Booking_ShouldSetIsActiveTrue()
    {
        var dto = new BookingCreateDto { TourName = "T", Place = "P", Email = "u@t.com", FirstName = "J" };

        var booking = _mapper.Map<Booking>(dto);

        booking.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Map_BookingCreateDto_To_Booking_ShouldMapEmailToUserEmail()
    {
        var dto = new BookingCreateDto { TourName = "T", Place = "P", Email = "user@test.com", FirstName = "J" };

        var booking = _mapper.Map<Booking>(dto);

        booking.UserEmail.Should().Be("user@test.com");
        booking.Email.Should().Be("user@test.com");
    }

    [Fact]
    public void Map_BookingCreateDto_To_Booking_ShouldIgnoreBookingId()
    {
        var dto = new BookingCreateDto { TourName = "T", Place = "P", Email = "u@t.com", FirstName = "J" };

        var booking = _mapper.Map<Booking>(dto);

        booking.BookingId.Should().Be(0);
    }

    // ─── BookingUpdateDto → Booking ───────────────────────────────────────────

    [Fact]
    public void Map_BookingUpdateDto_To_Booking_ShouldNotChangeBookingId()
    {
        var existing = new Booking { BookingId = 10, TourName = "Old", Place = "P", Email = "u@t.com", FirstName = "J", IsActive = true };
        var dto = new BookingUpdateDto { TourName = "New", Place = "P2", Email = "u@t.com", FirstName = "J", IsActive = false };

        _mapper.Map(dto, existing);

        existing.BookingId.Should().Be(10);
        existing.TourName.Should().Be("New");
        existing.IsActive.Should().BeFalse();
    }

    // ─── Collection Mappings ──────────────────────────────────────────────────

    [Fact]
    public void Map_IEnumerable_Tours_To_TourDtos_ShouldMapAll()
    {
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Tour A", Place = "P1", Days = 3, Price = 5000, Locations = "L1", TourInfo = "I1", IsActive = true },
            new Tour { TourId = 2, TourName = "Tour B", Place = "P2", Days = 5, Price = 8000, Locations = "L2", TourInfo = "I2", IsActive = true }
        };

        var dtos = _mapper.Map<IEnumerable<TourDto>>(tours).ToList();

        dtos.Should().HaveCount(2);
        dtos[0].TourName.Should().Be("Tour A");
        dtos[1].TourName.Should().Be("Tour B");
    }

    [Fact]
    public void Map_IEnumerable_Users_To_UserDtos_ShouldMapAll()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "a@t.com", FirstName = "A", LastName = "B", Gender = "M", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s" },
            new UserInfo { Email = "b@t.com", FirstName = "C", LastName = "D", Gender = "F", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s" }
        };

        var dtos = _mapper.Map<IEnumerable<UserDto>>(users).ToList();

        dtos.Should().HaveCount(2);
        dtos[0].Email.Should().Be("a@t.com");
        dtos[1].Email.Should().Be("b@t.com");
    }
}
