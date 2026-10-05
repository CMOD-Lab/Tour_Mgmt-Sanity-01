using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Tests.ViewModels
{
    public class BookingViewModelsTests
    {
        // ─── BookingViewModel ────────────────────────────────────────────────

        [Fact]
        public void BookingViewModel_DefaultValues_AreCorrect()
        {
            var vm = new BookingViewModel();
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Equal(default(DateTime), vm.BookingDate);
            Assert.False(vm.IsActive);
        }

        [Fact]
        public void BookingViewModel_SetAllProperties_RetainsValues()
        {
            var bookingDate = new DateTime(2024, 6, 15);
            var vm = new BookingViewModel
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "Alice",
                BookingDate = bookingDate,
                IsActive = true
            };

            Assert.Equal(1, vm.Id);
            Assert.Equal("Paris Tour", vm.TourName);
            Assert.Equal("Paris", vm.Place);
            Assert.Equal("user@example.com", vm.Email);
            Assert.Equal("Alice", vm.FirstName);
            Assert.Equal(bookingDate, vm.BookingDate);
            Assert.True(vm.IsActive);
        }

        [Fact]
        public void BookingViewModel_IsActive_CanBeSetToFalse()
        {
            var vm = new BookingViewModel { IsActive = false };
            Assert.False(vm.IsActive);
        }

        // ─── BookingCreateViewModel ──────────────────────────────────────────

        [Fact]
        public void BookingCreateViewModel_DefaultValues_AreCorrect()
        {
            var vm = new BookingCreateViewModel();
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Null(vm.TourId);
        }

        [Fact]
        public void BookingCreateViewModel_SetAllProperties_RetainsValues()
        {
            var vm = new BookingCreateViewModel
            {
                TourName = "Rome Tour",
                Place = "Rome",
                Email = "bob@example.com",
                FirstName = "Bob",
                TourId = 7
            };

            Assert.Equal("Rome Tour", vm.TourName);
            Assert.Equal("Rome", vm.Place);
            Assert.Equal("bob@example.com", vm.Email);
            Assert.Equal("Bob", vm.FirstName);
            Assert.Equal(7, vm.TourId);
        }

        [Fact]
        public void BookingCreateViewModel_TourId_CanBeNull()
        {
            var vm = new BookingCreateViewModel { TourId = null };
            Assert.Null(vm.TourId);
        }

        [Fact]
        public void BookingCreateViewModel_Validation_RequiredTourName_Fails_WhenEmpty()
        {
            var vm = new BookingCreateViewModel { TourName = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_RequiredPlace_Fails_WhenEmpty()
        {
            var vm = new BookingCreateViewModel { Place = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Place"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_RequiredEmail_Fails_WhenEmpty()
        {
            var vm = new BookingCreateViewModel { Email = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_InvalidEmail_Fails()
        {
            var vm = new BookingCreateViewModel { Email = "not-an-email" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_RequiredFirstName_Fails_WhenEmpty()
        {
            var vm = new BookingCreateViewModel { FirstName = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_TourNameMaxLength_Fails_WhenTooLong()
        {
            var vm = new BookingCreateViewModel { TourName = new string('X', 201) };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_PlaceMaxLength_Fails_WhenTooLong()
        {
            var vm = new BookingCreateViewModel { Place = new string('Y', 201) };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Place"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_EmailMaxLength_Fails_WhenTooLong()
        {
            var vm = new BookingCreateViewModel { Email = new string('a', 252) + "@b.com" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_FirstNameMaxLength_Fails_WhenTooLong()
        {
            var vm = new BookingCreateViewModel { FirstName = new string('Z', 101) };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void BookingCreateViewModel_Validation_ValidModel_Passes()
        {
            var vm = new BookingCreateViewModel
            {
                TourName = "Valid Tour",
                Place = "Valid Place",
                Email = "valid@example.com",
                FirstName = "ValidName"
            };
            var results = ValidateModel(vm);
            Assert.Empty(results);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, results, true);
            return results;
        }
    }
}
