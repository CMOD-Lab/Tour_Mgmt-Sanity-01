using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.ViewModels
{
    public class TourViewModelTests
    {
        [Fact]
        public void TourViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new TourViewModel();

            // Assert
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(0, vm.Days);
            Assert.Equal(0m, vm.Price);
            Assert.Equal(string.Empty, vm.Locations);
            Assert.Equal(string.Empty, vm.TourInfo);
            Assert.Null(vm.PicturePath);
            Assert.False(vm.IsActive);
        }

        [Fact]
        public void TourViewModel_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var vm = new TourViewModel
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower, Louvre",
                TourInfo = "Amazing Paris tour",
                PicturePath = "paris.jpg",
                IsActive = true,
                CreatedDate = now
            };

            // Assert
            Assert.Equal(1, vm.Id);
            Assert.Equal("Paris Tour", vm.TourName);
            Assert.Equal("Paris", vm.Place);
            Assert.Equal(7, vm.Days);
            Assert.Equal(1500.00m, vm.Price);
            Assert.Equal("Eiffel Tower, Louvre", vm.Locations);
            Assert.Equal("Amazing Paris tour", vm.TourInfo);
            Assert.Equal("paris.jpg", vm.PicturePath);
            Assert.True(vm.IsActive);
            Assert.Equal(now, vm.CreatedDate);
        }

        [Fact]
        public void TourViewModel_PicturePath_CanBeNull()
        {
            // Arrange & Act
            var vm = new TourViewModel { PicturePath = null };

            // Assert
            Assert.Null(vm.PicturePath);
        }

        [Fact]
        public void TourViewModel_IsActive_CanBeSetToFalse()
        {
            // Arrange & Act
            var vm = new TourViewModel { IsActive = false };

            // Assert
            Assert.False(vm.IsActive);
        }
    }

    public class TourCreateViewModelTests
    {
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void TourCreateViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new TourCreateViewModel();

            // Assert
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(0, vm.Days);
            Assert.Equal(0m, vm.Price);
            Assert.Equal(string.Empty, vm.Locations);
            Assert.Equal(string.Empty, vm.TourInfo);
            Assert.Null(vm.PictureFile);
        }

        [Fact]
        public void TourCreateViewModel_ValidModel_PassesValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void TourCreateViewModel_EmptyTourName_FailsValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "",
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void TourCreateViewModel_TourNameExceedsMaxLength_FailsValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = new string('A', 201),
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void TourCreateViewModel_DaysOutOfRange_FailsValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 0,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Days"));
        }

        [Fact]
        public void TourCreateViewModel_DaysMaxRange_FailsValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 366,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Days"));
        }

        [Fact]
        public void TourCreateViewModel_PriceZero_FailsValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 0m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Price"));
        }

        [Fact]
        public void TourCreateViewModel_EmptyPlace_FailsValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Place"));
        }

        [Fact]
        public void TourCreateViewModel_EmptyLocations_FailsValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Locations"));
        }

        [Fact]
        public void TourCreateViewModel_EmptyTourInfo_FailsValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = ""
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("TourInfo"));
        }

        [Fact]
        public void TourCreateViewModel_ValidDaysMinBoundary_PassesValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 1,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void TourCreateViewModel_ValidDaysMaxBoundary_PassesValidation()
        {
            // Arrange
            var vm = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 365,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }
    }

    public class TourEditViewModelTests
    {
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void TourEditViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new TourEditViewModel();

            // Assert
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(0, vm.Days);
            Assert.Equal(0m, vm.Price);
            Assert.Equal(string.Empty, vm.Locations);
            Assert.Equal(string.Empty, vm.TourInfo);
            Assert.Null(vm.PictureFile);
            Assert.Null(vm.ExistingPicturePath);
            Assert.False(vm.IsActive);
        }

        [Fact]
        public void TourEditViewModel_ValidModel_PassesValidation()
        {
            // Arrange
            var vm = new TourEditViewModel
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour",
                IsActive = true
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void TourEditViewModel_EmptyTourName_FailsValidation()
        {
            // Arrange
            var vm = new TourEditViewModel
            {
                Id = 1,
                TourName = "",
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing Paris tour"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void TourEditViewModel_ExistingPicturePath_CanBeSet()
        {
            // Arrange & Act
            var vm = new TourEditViewModel { ExistingPicturePath = "existing.jpg" };

            // Assert
            Assert.Equal("existing.jpg", vm.ExistingPicturePath);
        }

        [Fact]
        public void TourEditViewModel_IsActive_CanBeToggled()
        {
            // Arrange
            var vm = new TourEditViewModel { IsActive = true };

            // Assert
            Assert.True(vm.IsActive);

            // Act
            vm.IsActive = false;

            // Assert
            Assert.False(vm.IsActive);
        }
    }
}
