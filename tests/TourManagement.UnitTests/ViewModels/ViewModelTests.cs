using TourManagement.Web.ViewModels;
using Xunit;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.UnitTests.ViewModels;

/// <summary>
/// Unit tests for Tour ViewModels.
/// </summary>
public class TourViewModelTests
{
    // ─── TourViewModel ────────────────────────────────────────────────────────

    [Fact]
    public void TourViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new TourViewModel();

        vm.TourId.Should().Be(0);
        vm.TourName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.Days.Should().Be(0);
        vm.Price.Should().Be(0m);
        vm.Locations.Should().Be(string.Empty);
        vm.TourInfo.Should().Be(string.Empty);
        vm.Pic.Should().BeNull();
        vm.IsActive.Should().BeFalse();
    }

    [Fact]
    public void TourViewModel_SetProperties_ShouldRetainValues()
    {
        var vm = new TourViewModel
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

        vm.TourId.Should().Be(1);
        vm.TourName.Should().Be("Kashmir Tour");
        vm.IsActive.Should().BeTrue();
    }

    // ─── TourCreateViewModel ──────────────────────────────────────────────────

    [Fact]
    public void TourCreateViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new TourCreateViewModel();

        vm.TourName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.Days.Should().Be(0);
        vm.Price.Should().Be(0m);
        vm.Locations.Should().Be(string.Empty);
        vm.TourInfo.Should().Be(string.Empty);
        vm.PicFile.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void TourCreateViewModel_WithValidData_ShouldPassValidation()
    {
        var vm = new TourCreateViewModel
        {
            TourName = "Kashmir Tour",
            Place = "Kashmir",
            Days = 7,
            Price = 15000m,
            Locations = "Srinagar, Gulmarg",
            TourInfo = "Beautiful Kashmir tour"
        };

        var results = ValidateModel(vm);
        results.Should().BeEmpty();
    }

    [Fact]
    public void TourCreateViewModel_WithEmptyTourName_ShouldFailValidation()
    {
        var vm = new TourCreateViewModel
        {
            TourName = "",
            Place = "Kashmir",
            Days = 7,
            Price = 15000m,
            Locations = "Srinagar",
            TourInfo = "Info"
        };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("TourName"));
    }

    [Fact]
    public void TourCreateViewModel_WithZeroDays_ShouldFailValidation()
    {
        var vm = new TourCreateViewModel
        {
            TourName = "Tour",
            Place = "Place",
            Days = 0,
            Price = 1000m,
            Locations = "L",
            TourInfo = "I"
        };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("Days"));
    }

    [Fact]
    public void TourCreateViewModel_WithZeroPrice_ShouldFailValidation()
    {
        var vm = new TourCreateViewModel
        {
            TourName = "Tour",
            Place = "Place",
            Days = 3,
            Price = 0m,
            Locations = "L",
            TourInfo = "I"
        };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("Price"));
    }

    // ─── TourEditViewModel ────────────────────────────────────────────────────

    [Fact]
    public void TourEditViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new TourEditViewModel();

        vm.TourId.Should().Be(0);
        vm.TourName.Should().Be(string.Empty);
        vm.ExistingPic.Should().BeNull();
        vm.PicFile.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void TourEditViewModel_WithValidData_ShouldPassValidation()
    {
        var vm = new TourEditViewModel
        {
            TourId = 1,
            TourName = "Kashmir Tour",
            Place = "Kashmir",
            Days = 7,
            Price = 15000m,
            Locations = "Srinagar",
            TourInfo = "Beautiful Kashmir",
            IsActive = true
        };

        var results = ValidateModel(vm);
        results.Should().BeEmpty();
    }

    // ─── TourListViewModel ────────────────────────────────────────────────────

    [Fact]
    public void TourListViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new TourListViewModel();

        vm.Tours.Should().NotBeNull();
        vm.Tours.Should().BeEmpty();
        vm.SearchTerm.Should().BeNull();
        vm.SuccessMessage.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void TourListViewModel_SetTours_ShouldRetainValues()
    {
        var tours = new List<TourViewModel>
        {
            new TourViewModel { TourId = 1, TourName = "Tour A" },
            new TourViewModel { TourId = 2, TourName = "Tour B" }
        };

        var vm = new TourListViewModel { Tours = tours, SearchTerm = "test", SuccessMessage = "Done" };

        vm.Tours.Should().HaveCount(2);
        vm.SearchTerm.Should().Be("test");
        vm.SuccessMessage.Should().Be("Done");
    }

    // ─── TourDetailsViewModel ─────────────────────────────────────────────────

    [Fact]
    public void TourDetailsViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new TourDetailsViewModel();

        vm.Tour.Should().NotBeNull();
        vm.SuccessMessage.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}

/// <summary>
/// Unit tests for User ViewModels.
/// </summary>
public class UserViewModelTests
{
    // ─── LoginViewModel ───────────────────────────────────────────────────────

    [Fact]
    public void LoginViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new LoginViewModel();

        vm.Email.Should().Be(string.Empty);
        vm.Password.Should().Be(string.Empty);
        vm.ReturnUrl.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void LoginViewModel_WithValidData_ShouldPassValidation()
    {
        var vm = new LoginViewModel { Email = "user@test.com", Password = "Password123" };

        var results = ValidateModel(vm);
        results.Should().BeEmpty();
    }

    [Fact]
    public void LoginViewModel_WithEmptyEmail_ShouldFailValidation()
    {
        var vm = new LoginViewModel { Email = "", Password = "Password123" };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginViewModel_WithInvalidEmail_ShouldFailValidation()
    {
        var vm = new LoginViewModel { Email = "notanemail", Password = "Password123" };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginViewModel_WithEmptyPassword_ShouldFailValidation()
    {
        var vm = new LoginViewModel { Email = "user@test.com", Password = "" };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("Password"));
    }

    // ─── RegisterViewModel ────────────────────────────────────────────────────

    [Fact]
    public void RegisterViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new RegisterViewModel();

        vm.Email.Should().Be(string.Empty);
        vm.FirstName.Should().Be(string.Empty);
        vm.LastName.Should().Be(string.Empty);
        vm.Gender.Should().Be(string.Empty);
        vm.Password.Should().Be(string.Empty);
        vm.ConfirmPassword.Should().Be(string.Empty);
        vm.Street.Should().Be(string.Empty);
        vm.City.Should().Be(string.Empty);
        vm.State.Should().Be(string.Empty);
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void RegisterViewModel_WithValidData_ShouldPassValidation()
    {
        var vm = new RegisterViewModel
        {
            Email = "new@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "Password123",
            ConfirmPassword = "Password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "1 Main St",
            City = "New York",
            State = "NY"
        };

        var results = ValidateModel(vm);
        results.Should().BeEmpty();
    }

    [Fact]
    public void RegisterViewModel_WithShortPassword_ShouldFailValidation()
    {
        var vm = new RegisterViewModel
        {
            Email = "new@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "abc",
            ConfirmPassword = "abc",
            Dob = new DateTime(1990, 1, 1),
            Street = "1 Main St",
            City = "NY",
            State = "NY"
        };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("Password"));
    }

    // ─── UserProfileViewModel ─────────────────────────────────────────────────

    [Fact]
    public void UserProfileViewModel_FullName_ShouldCombineFirstAndLastName()
    {
        var vm = new UserProfileViewModel { FirstName = "John", LastName = "Doe" };

        vm.FullName.Should().Be("John Doe");
    }

    [Fact]
    public void UserProfileViewModel_FullName_WithEmptyNames_ShouldReturnSpace()
    {
        var vm = new UserProfileViewModel { FirstName = "", LastName = "" };

        vm.FullName.Should().Be(" ");
    }

    [Fact]
    public void UserProfileViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new UserProfileViewModel();

        vm.Email.Should().Be(string.Empty);
        vm.FirstName.Should().Be(string.Empty);
        vm.LastName.Should().Be(string.Empty);
        vm.Gender.Should().Be(string.Empty);
        vm.Street.Should().Be(string.Empty);
        vm.City.Should().Be(string.Empty);
        vm.State.Should().Be(string.Empty);
    }

    // ─── UserListViewModel ────────────────────────────────────────────────────

    [Fact]
    public void UserListViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new UserListViewModel();

        vm.Users.Should().NotBeNull();
        vm.Users.Should().BeEmpty();
        vm.SearchTerm.Should().BeNull();
        vm.SuccessMessage.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}

/// <summary>
/// Unit tests for Booking ViewModels.
/// </summary>
public class BookingViewModelTests
{
    // ─── BookingViewModel ─────────────────────────────────────────────────────

    [Fact]
    public void BookingViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new BookingViewModel();

        vm.BookingId.Should().Be(0);
        vm.TourName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.Email.Should().Be(string.Empty);
        vm.FirstName.Should().Be(string.Empty);
        vm.IsActive.Should().BeFalse();
    }

    [Fact]
    public void BookingViewModel_SetProperties_ShouldRetainValues()
    {
        var vm = new BookingViewModel
        {
            BookingId = 1,
            TourName = "Kerala Tour",
            Place = "Kerala",
            Email = "user@test.com",
            FirstName = "Priya",
            BookingDate = new DateTime(2024, 8, 10),
            IsActive = true
        };

        vm.BookingId.Should().Be(1);
        vm.TourName.Should().Be("Kerala Tour");
        vm.IsActive.Should().BeTrue();
    }

    // ─── BookingCreateViewModel ───────────────────────────────────────────────

    [Fact]
    public void BookingCreateViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new BookingCreateViewModel();

        vm.TourName.Should().Be(string.Empty);
        vm.Place.Should().Be(string.Empty);
        vm.Email.Should().Be(string.Empty);
        vm.FirstName.Should().Be(string.Empty);
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void BookingCreateViewModel_WithValidData_ShouldPassValidation()
    {
        var vm = new BookingCreateViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "traveler@test.com",
            FirstName = "John"
        };

        var results = ValidateModel(vm);
        results.Should().BeEmpty();
    }

    [Fact]
    public void BookingCreateViewModel_WithEmptyTourName_ShouldFailValidation()
    {
        var vm = new BookingCreateViewModel
        {
            TourName = "",
            Place = "Goa",
            Email = "traveler@test.com",
            FirstName = "John"
        };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("TourName"));
    }

    [Fact]
    public void BookingCreateViewModel_WithInvalidEmail_ShouldFailValidation()
    {
        var vm = new BookingCreateViewModel
        {
            TourName = "Tour",
            Place = "Place",
            Email = "notanemail",
            FirstName = "John"
        };

        var results = ValidateModel(vm);
        results.Should().Contain(r => r.MemberNames.Contains("Email"));
    }

    // ─── BookingListViewModel ─────────────────────────────────────────────────

    [Fact]
    public void BookingListViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new BookingListViewModel();

        vm.Bookings.Should().NotBeNull();
        vm.Bookings.Should().BeEmpty();
        vm.SearchTerm.Should().BeNull();
        vm.SuccessMessage.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
        vm.IsAdminView.Should().BeFalse();
    }

    [Fact]
    public void BookingListViewModel_SetProperties_ShouldRetainValues()
    {
        var bookings = new List<BookingViewModel>
        {
            new BookingViewModel { BookingId = 1, TourName = "Tour A" }
        };

        var vm = new BookingListViewModel
        {
            Bookings = bookings,
            SearchTerm = "test",
            IsAdminView = true,
            SuccessMessage = "Done"
        };

        vm.Bookings.Should().HaveCount(1);
        vm.IsAdminView.Should().BeTrue();
        vm.SuccessMessage.Should().Be("Done");
    }

    // ─── BookingDetailsViewModel ──────────────────────────────────────────────

    [Fact]
    public void BookingDetailsViewModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        var vm = new BookingDetailsViewModel();

        vm.Booking.Should().NotBeNull();
        vm.SuccessMessage.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
