using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Tests.ViewModels
{
    public class UserViewModelsTests
    {
        // ─── RegisterViewModel ───────────────────────────────────────────────

        [Fact]
        public void RegisterViewModel_DefaultValues_AreEmpty()
        {
            var vm = new RegisterViewModel();
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Equal(string.Empty, vm.LastName);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(string.Empty, vm.Password);
            Assert.Equal(string.Empty, vm.ConfirmPassword);
            Assert.Null(vm.DateOfBirth);
            Assert.Equal(string.Empty, vm.Street);
            Assert.Equal(string.Empty, vm.City);
            Assert.Equal(string.Empty, vm.State);
        }

        [Fact]
        public void RegisterViewModel_SetProperties_RetainsValues()
        {
            var dob = new DateTime(1990, 5, 15);
            var vm = new RegisterViewModel
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "Password123",
                ConfirmPassword = "Password123",
                DateOfBirth = dob,
                Street = "123 Main St",
                City = "Springfield",
                State = "IL"
            };

            Assert.Equal("test@example.com", vm.Email);
            Assert.Equal("John", vm.FirstName);
            Assert.Equal("Doe", vm.LastName);
            Assert.Equal("Male", vm.Gender);
            Assert.Equal("Password123", vm.Password);
            Assert.Equal("Password123", vm.ConfirmPassword);
            Assert.Equal(dob, vm.DateOfBirth);
            Assert.Equal("123 Main St", vm.Street);
            Assert.Equal("Springfield", vm.City);
            Assert.Equal("IL", vm.State);
        }

        [Fact]
        public void RegisterViewModel_Validation_RequiredEmail_Fails_WhenEmpty()
        {
            var vm = new RegisterViewModel { Email = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void RegisterViewModel_Validation_InvalidEmail_Fails()
        {
            var vm = new RegisterViewModel { Email = "not-an-email" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void RegisterViewModel_Validation_ValidEmail_Passes()
        {
            var vm = new RegisterViewModel
            {
                Email = "valid@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };
            var results = ValidateModel(vm);
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void RegisterViewModel_Validation_PasswordTooShort_Fails()
        {
            var vm = new RegisterViewModel { Password = "abc" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void RegisterViewModel_Validation_RequiredFirstName_Fails_WhenEmpty()
        {
            var vm = new RegisterViewModel { FirstName = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void RegisterViewModel_Validation_RequiredLastName_Fails_WhenEmpty()
        {
            var vm = new RegisterViewModel { LastName = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("LastName"));
        }

        [Fact]
        public void RegisterViewModel_Validation_RequiredGender_Fails_WhenEmpty()
        {
            var vm = new RegisterViewModel { Gender = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Gender"));
        }

        // ─── LoginViewModel ──────────────────────────────────────────────────

        [Fact]
        public void LoginViewModel_DefaultValues_AreEmpty()
        {
            var vm = new LoginViewModel();
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
        }

        [Fact]
        public void LoginViewModel_SetProperties_RetainsValues()
        {
            var vm = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "secret"
            };
            Assert.Equal("user@example.com", vm.Email);
            Assert.Equal("secret", vm.Password);
        }

        [Fact]
        public void LoginViewModel_Validation_RequiredEmail_Fails_WhenEmpty()
        {
            var vm = new LoginViewModel { Email = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void LoginViewModel_Validation_RequiredPassword_Fails_WhenEmpty()
        {
            var vm = new LoginViewModel { Password = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void LoginViewModel_Validation_InvalidEmail_Fails()
        {
            var vm = new LoginViewModel { Email = "bad-email" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        // ─── AdminLoginViewModel ─────────────────────────────────────────────

        [Fact]
        public void AdminLoginViewModel_DefaultValues_AreEmpty()
        {
            var vm = new AdminLoginViewModel();
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
        }

        [Fact]
        public void AdminLoginViewModel_SetProperties_RetainsValues()
        {
            var vm = new AdminLoginViewModel
            {
                Email = "admin@example.com",
                Password = "adminpass"
            };
            Assert.Equal("admin@example.com", vm.Email);
            Assert.Equal("adminpass", vm.Password);
        }

        [Fact]
        public void AdminLoginViewModel_Validation_RequiredEmail_Fails_WhenEmpty()
        {
            var vm = new AdminLoginViewModel { Email = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void AdminLoginViewModel_Validation_RequiredPassword_Fails_WhenEmpty()
        {
            var vm = new AdminLoginViewModel { Password = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        // ─── UserViewModel ───────────────────────────────────────────────────

        [Fact]
        public void UserViewModel_DefaultValues_AreCorrect()
        {
            var vm = new UserViewModel();
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Equal(string.Empty, vm.LastName);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Null(vm.DateOfBirth);
            Assert.Equal(string.Empty, vm.Street);
            Assert.Equal(string.Empty, vm.City);
            Assert.Equal(string.Empty, vm.State);
            Assert.Equal("User", vm.Role);
            Assert.False(vm.IsActive);
        }

        [Fact]
        public void UserViewModel_FullName_ConcatenatesFirstAndLastName()
        {
            var vm = new UserViewModel { FirstName = "Jane", LastName = "Smith" };
            Assert.Equal("Jane Smith", vm.FullName);
        }

        [Fact]
        public void UserViewModel_FullName_WithEmptyNames_ReturnsSpace()
        {
            var vm = new UserViewModel { FirstName = "", LastName = "" };
            Assert.Equal(" ", vm.FullName);
        }

        [Fact]
        public void UserViewModel_SetAllProperties_RetainsValues()
        {
            var created = new DateTime(2023, 1, 1);
            var dob = new DateTime(1985, 3, 20);
            var vm = new UserViewModel
            {
                Id = 42,
                Email = "jane@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                Gender = "Female",
                DateOfBirth = dob,
                Street = "456 Oak Ave",
                City = "Chicago",
                State = "IL",
                Role = "Admin",
                CreatedDate = created,
                IsActive = true
            };

            Assert.Equal(42, vm.Id);
            Assert.Equal("jane@example.com", vm.Email);
            Assert.Equal("Jane", vm.FirstName);
            Assert.Equal("Smith", vm.LastName);
            Assert.Equal("Female", vm.Gender);
            Assert.Equal(dob, vm.DateOfBirth);
            Assert.Equal("456 Oak Ave", vm.Street);
            Assert.Equal("Chicago", vm.City);
            Assert.Equal("IL", vm.State);
            Assert.Equal("Admin", vm.Role);
            Assert.Equal(created, vm.CreatedDate);
            Assert.True(vm.IsActive);
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
