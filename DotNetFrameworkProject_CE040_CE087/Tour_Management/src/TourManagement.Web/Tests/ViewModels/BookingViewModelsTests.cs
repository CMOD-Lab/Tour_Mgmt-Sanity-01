using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.ViewModels
{
    public class BookingViewModelTests
    {
        [Fact]
        public void BookingViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new BookingViewModel();

            // Assert
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.False(vm.IsActive);
        }

        [Fact]
        public void BookingViewModel_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var vm = new BookingViewModel
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John",
                IsActive = true,
                CreatedDate = now
            };

            // Assert
            Assert.Equal(1, vm.Id);
            Assert.Equal("Paris Tour", vm.TourName);
            Assert.Equal("Paris", vm.Place);
            Assert.Equal("user@example.com", vm.Email);
            Assert.Equal("John", vm.FirstName);
            Assert.True(vm.IsActive);
            Assert.Equal(now, vm.CreatedDate);
        }

        [Fact]
        public void BookingViewModel_IsActive_CanBeSetToFalse()
        {
            // Arrange & Act
            var vm = new BookingViewModel { IsActive = false };

            // Assert
            Assert.False(vm.IsActive);
        }

        [Fact]
        public void BookingViewModel_CreatedDate_CanBeSet()
        {
            // Arrange
            var date = new DateTime(2024, 1, 15);

            // Act
            var vm = new BookingViewModel { CreatedDate = date };

            // Assert
            Assert.Equal(date, vm.CreatedDate);
        }
    }

    public class BookingCreateViewModelTests
    {
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void BookingCreateViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new BookingCreateViewModel();

            // Assert
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Null(vm.TourId);
        }

        [Fact]
        public void BookingCreateViewModel_ValidModel_PassesValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void BookingCreateViewModel_EmptyTourName_FailsValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = "",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void BookingCreateViewModel_EmptyPlace_FailsValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "",
                Email = "user@example.com",
                FirstName = "John"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Place"));
        }

        [Fact]
        public void BookingCreateViewModel_EmptyEmail_FailsValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "",
                FirstName = "John"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void BookingCreateViewModel_InvalidEmail_FailsValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "not-an-email",
                FirstName = "John"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void BookingCreateViewModel_EmptyFirstName_FailsValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = ""
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void BookingCreateViewModel_TourId_CanBeNull()
        {
            // Arrange & Act
            var vm = new BookingCreateViewModel { TourId = null };

            // Assert
            Assert.Null(vm.TourId);
        }

        [Fact]
        public void BookingCreateViewModel_TourId_CanBeSet()
        {
            // Arrange & Act
            var vm = new BookingCreateViewModel { TourId = 5 };

            // Assert
            Assert.Equal(5, vm.TourId);
        }

        [Fact]
        public void BookingCreateViewModel_TourNameExceedsMaxLength_FailsValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = new string('A', 201),
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void BookingCreateViewModel_FirstNameExceedsMaxLength_FailsValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = new string('A', 101)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void BookingCreateViewModel_ValidModelWithTourId_PassesValidation()
        {
            // Arrange
            var vm = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John",
                TourId = 1
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }
    }
}
