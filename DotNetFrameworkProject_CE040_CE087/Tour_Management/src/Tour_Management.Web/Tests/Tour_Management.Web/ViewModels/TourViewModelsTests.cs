using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Tests.ViewModels
{
    public class TourViewModelsTests
    {
        // ─── TourViewModel ───────────────────────────────────────────────────

        [Fact]
        public void TourViewModel_DefaultValues_AreCorrect()
        {
            var vm = new TourViewModel();
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(0, vm.Days);
            Assert.Equal(0m, vm.Price);
            Assert.Equal(string.Empty, vm.Locations);
            Assert.Equal(string.Empty, vm.TourInfo);
            Assert.Null(vm.Pic);
            Assert.False(vm.IsActive);
        }

        [Fact]
        public void TourViewModel_SetAllProperties_RetainsValues()
        {
            var created = new DateTime(2024, 1, 15);
            var vm = new TourViewModel
            {
                Id = 10,
                TourName = "Paris Adventure",
                Place = "Paris",
                Days = 7,
                Price = 1500.00m,
                Locations = "Eiffel Tower, Louvre",
                TourInfo = "A wonderful trip to Paris",
                Pic = "paris.jpg",
                CreatedDate = created,
                IsActive = true
            };

            Assert.Equal(10, vm.Id);
            Assert.Equal("Paris Adventure", vm.TourName);
            Assert.Equal("Paris", vm.Place);
            Assert.Equal(7, vm.Days);
            Assert.Equal(1500.00m, vm.Price);
            Assert.Equal("Eiffel Tower, Louvre", vm.Locations);
            Assert.Equal("A wonderful trip to Paris", vm.TourInfo);
            Assert.Equal("paris.jpg", vm.Pic);
            Assert.Equal(created, vm.CreatedDate);
            Assert.True(vm.IsActive);
        }

        [Fact]
        public void TourViewModel_Pic_CanBeNull()
        {
            var vm = new TourViewModel { Pic = null };
            Assert.Null(vm.Pic);
        }

        // ─── TourCreateViewModel ─────────────────────────────────────────────

        [Fact]
        public void TourCreateViewModel_DefaultValues_AreCorrect()
        {
            var vm = new TourCreateViewModel();
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(0, vm.Days);
            Assert.Equal(0m, vm.Price);
            Assert.Equal(string.Empty, vm.Locations);
            Assert.Equal(string.Empty, vm.TourInfo);
            Assert.Null(vm.PicFile);
        }

        [Fact]
        public void TourCreateViewModel_Validation_RequiredTourName_Fails_WhenEmpty()
        {
            var vm = new TourCreateViewModel { TourName = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_RequiredPlace_Fails_WhenEmpty()
        {
            var vm = new TourCreateViewModel { Place = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Place"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_DaysRange_Fails_WhenZero()
        {
            var vm = new TourCreateViewModel { Days = 0 };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Days"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_DaysRange_Fails_WhenOver365()
        {
            var vm = new TourCreateViewModel { Days = 366 };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Days"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_DaysRange_Passes_WhenValid()
        {
            var vm = new TourCreateViewModel
            {
                TourName = "Test Tour",
                Place = "Test Place",
                Days = 5,
                Price = 100m
            };
            var results = ValidateModel(vm);
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Days"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_PriceRange_Fails_WhenZero()
        {
            var vm = new TourCreateViewModel { Price = 0m };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Price"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_PriceRange_Fails_WhenNegative()
        {
            var vm = new TourCreateViewModel { Price = -1m };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Price"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_TourNameMaxLength_Fails_WhenTooLong()
        {
            var vm = new TourCreateViewModel { TourName = new string('A', 201) };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_PlaceMaxLength_Fails_WhenTooLong()
        {
            var vm = new TourCreateViewModel { Place = new string('B', 201) };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Place"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_LocationsMaxLength_Fails_WhenTooLong()
        {
            var vm = new TourCreateViewModel { Locations = new string('C', 501) };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Locations"));
        }

        [Fact]
        public void TourCreateViewModel_Validation_TourInfoMaxLength_Fails_WhenTooLong()
        {
            var vm = new TourCreateViewModel { TourInfo = new string('D', 2001) };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("TourInfo"));
        }

        // ─── TourEditViewModel ───────────────────────────────────────────────

        [Fact]
        public void TourEditViewModel_DefaultValues_AreCorrect()
        {
            var vm = new TourEditViewModel();
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.TourName);
            Assert.Equal(string.Empty, vm.Place);
            Assert.Equal(0, vm.Days);
            Assert.Equal(0m, vm.Price);
            Assert.Equal(string.Empty, vm.Locations);
            Assert.Equal(string.Empty, vm.TourInfo);
            Assert.Null(vm.ExistingPic);
            Assert.Null(vm.PicFile);
            Assert.False(vm.IsActive);
        }

        [Fact]
        public void TourEditViewModel_SetAllProperties_RetainsValues()
        {
            var vm = new TourEditViewModel
            {
                Id = 5,
                TourName = "Rome Tour",
                Place = "Rome",
                Days = 10,
                Price = 2000m,
                Locations = "Colosseum, Vatican",
                TourInfo = "Explore ancient Rome",
                ExistingPic = "rome.jpg",
                IsActive = true
            };

            Assert.Equal(5, vm.Id);
            Assert.Equal("Rome Tour", vm.TourName);
            Assert.Equal("Rome", vm.Place);
            Assert.Equal(10, vm.Days);
            Assert.Equal(2000m, vm.Price);
            Assert.Equal("Colosseum, Vatican", vm.Locations);
            Assert.Equal("Explore ancient Rome", vm.TourInfo);
            Assert.Equal("rome.jpg", vm.ExistingPic);
            Assert.True(vm.IsActive);
        }

        [Fact]
        public void TourEditViewModel_Validation_RequiredTourName_Fails_WhenEmpty()
        {
            var vm = new TourEditViewModel { TourName = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
        }

        [Fact]
        public void TourEditViewModel_Validation_RequiredPlace_Fails_WhenEmpty()
        {
            var vm = new TourEditViewModel { Place = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Place"));
        }

        [Fact]
        public void TourEditViewModel_Validation_DaysRange_Fails_WhenZero()
        {
            var vm = new TourEditViewModel { Days = 0 };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Days"));
        }

        [Fact]
        public void TourEditViewModel_Validation_PriceRange_Fails_WhenZero()
        {
            var vm = new TourEditViewModel { Price = 0m };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Price"));
        }

        [Fact]
        public void TourEditViewModel_ExistingPic_CanBeNull()
        {
            var vm = new TourEditViewModel { ExistingPic = null };
            Assert.Null(vm.ExistingPic);
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
