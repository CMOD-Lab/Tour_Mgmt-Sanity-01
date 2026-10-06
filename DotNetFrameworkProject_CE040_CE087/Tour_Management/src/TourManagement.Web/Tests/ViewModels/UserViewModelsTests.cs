using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.ViewModels
{
    public class LoginViewModelTests
    {
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void LoginViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new LoginViewModel();

            // Assert
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
        }

        [Fact]
        public void LoginViewModel_ValidModel_PassesValidation()
        {
            // Arrange
            var vm = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "password123"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void LoginViewModel_EmptyEmail_FailsValidation()
        {
            // Arrange
            var vm = new LoginViewModel
            {
                Email = "",
                Password = "password123"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void LoginViewModel_InvalidEmail_FailsValidation()
        {
            // Arrange
            var vm = new LoginViewModel
            {
                Email = "not-an-email",
                Password = "password123"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void LoginViewModel_EmptyPassword_FailsValidation()
        {
            // Arrange
            var vm = new LoginViewModel
            {
                Email = "user@example.com",
                Password = ""
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void LoginViewModel_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var vm = new LoginViewModel
            {
                Email = "test@test.com",
                Password = "securePass"
            };

            // Assert
            Assert.Equal("test@test.com", vm.Email);
            Assert.Equal("securePass", vm.Password);
        }
    }

    public class RegisterViewModelTests
    {
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void RegisterViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new RegisterViewModel();

            // Assert
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Equal(string.Empty, vm.LastName);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(string.Empty, vm.Password);
            Assert.Equal(string.Empty, vm.ConfirmPassword);
            Assert.Equal(string.Empty, vm.Street);
            Assert.Equal(string.Empty, vm.City);
            Assert.Equal(string.Empty, vm.State);
        }

        [Fact]
        public void RegisterViewModel_ValidModel_PassesValidation()
        {
            // Arrange
            var vm = new RegisterViewModel
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1),
                Street = "123 Main St",
                City = "New York",
                State = "NY"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void RegisterViewModel_EmptyEmail_FailsValidation()
        {
            // Arrange
            var vm = new RegisterViewModel
            {
                Email = "",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void RegisterViewModel_InvalidEmail_FailsValidation()
        {
            // Arrange
            var vm = new RegisterViewModel
            {
                Email = "invalid-email",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void RegisterViewModel_PasswordTooShort_FailsValidation()
        {
            // Arrange
            var vm = new RegisterViewModel
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "abc",
                ConfirmPassword = "abc",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void RegisterViewModel_PasswordMismatch_FailsValidation()
        {
            // Arrange
            var vm = new RegisterViewModel
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "differentpassword",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("ConfirmPassword"));
        }

        [Fact]
        public void RegisterViewModel_EmptyFirstName_FailsValidation()
        {
            // Arrange
            var vm = new RegisterViewModel
            {
                Email = "user@example.com",
                FirstName = "",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void RegisterViewModel_EmptyLastName_FailsValidation()
        {
            // Arrange
            var vm = new RegisterViewModel
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("LastName"));
        }

        [Fact]
        public void RegisterViewModel_EmptyGender_FailsValidation()
        {
            // Arrange
            var vm = new RegisterViewModel
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Gender"));
        }
    }

    public class UserProfileViewModelTests
    {
        [Fact]
        public void UserProfileViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new UserProfileViewModel();

            // Assert
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Equal(string.Empty, vm.LastName);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(string.Empty, vm.Street);
            Assert.Equal(string.Empty, vm.City);
            Assert.Equal(string.Empty, vm.State);
            Assert.False(vm.IsAdmin);
        }

        [Fact]
        public void UserProfileViewModel_FullName_CombinesFirstAndLastName()
        {
            // Arrange
            var vm = new UserProfileViewModel
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var fullName = vm.FullName;

            // Assert
            Assert.Equal("John Doe", fullName);
        }

        [Fact]
        public void UserProfileViewModel_FullName_WithEmptyLastName()
        {
            // Arrange
            var vm = new UserProfileViewModel
            {
                FirstName = "Administrator",
                LastName = ""
            };

            // Act
            var fullName = vm.FullName;

            // Assert
            Assert.Equal("Administrator ", fullName);
        }

        [Fact]
        public void UserProfileViewModel_FullName_WithEmptyFirstName()
        {
            // Arrange
            var vm = new UserProfileViewModel
            {
                FirstName = "",
                LastName = "Doe"
            };

            // Act
            var fullName = vm.FullName;

            // Assert
            Assert.Equal(" Doe", fullName);
        }

        [Fact]
        public void UserProfileViewModel_SetAllProperties_ReturnsCorrectValues()
        {
            // Arrange
            var dob = new DateTime(1990, 5, 15);
            var created = DateTime.UtcNow;
            var vm = new UserProfileViewModel
            {
                Id = 1,
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = dob,
                Street = "123 Main St",
                City = "New York",
                State = "NY",
                IsAdmin = false,
                CreatedDate = created
            };

            // Assert
            Assert.Equal(1, vm.Id);
            Assert.Equal("john@example.com", vm.Email);
            Assert.Equal("John", vm.FirstName);
            Assert.Equal("Doe", vm.LastName);
            Assert.Equal("Male", vm.Gender);
            Assert.Equal(dob, vm.DateOfBirth);
            Assert.Equal("123 Main St", vm.Street);
            Assert.Equal("New York", vm.City);
            Assert.Equal("NY", vm.State);
            Assert.False(vm.IsAdmin);
            Assert.Equal(created, vm.CreatedDate);
        }

        [Fact]
        public void UserProfileViewModel_IsAdmin_CanBeSetToTrue()
        {
            // Arrange & Act
            var vm = new UserProfileViewModel { IsAdmin = true };

            // Assert
            Assert.True(vm.IsAdmin);
        }
    }

    public class UserEditViewModelTests
    {
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void UserEditViewModel_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var vm = new UserEditViewModel();

            // Assert
            Assert.Equal(0, vm.Id);
            Assert.Equal(string.Empty, vm.FirstName);
            Assert.Equal(string.Empty, vm.LastName);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(string.Empty, vm.Street);
            Assert.Equal(string.Empty, vm.City);
            Assert.Equal(string.Empty, vm.State);
        }

        [Fact]
        public void UserEditViewModel_ValidModel_PassesValidation()
        {
            // Arrange
            var vm = new UserEditViewModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                Street = "123 Main St",
                City = "New York",
                State = "NY"
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void UserEditViewModel_EmptyFirstName_FailsValidation()
        {
            // Arrange
            var vm = new UserEditViewModel
            {
                Id = 1,
                FirstName = "",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void UserEditViewModel_EmptyLastName_FailsValidation()
        {
            // Arrange
            var vm = new UserEditViewModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("LastName"));
        }

        [Fact]
        public void UserEditViewModel_EmptyGender_FailsValidation()
        {
            // Arrange
            var vm = new UserEditViewModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Gender = "",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Gender"));
        }

        [Fact]
        public void UserEditViewModel_FirstNameExceedsMaxLength_FailsValidation()
        {
            // Arrange
            var vm = new UserEditViewModel
            {
                Id = 1,
                FirstName = new string('A', 101),
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // Act
            var results = ValidateModel(vm);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
        }
    }
}
