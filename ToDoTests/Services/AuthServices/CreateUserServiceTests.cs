using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoTests.Services.AuthServices
{
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    namespace ToDoTests.Services.AuthServices
    {
        [TestClass]
        public class AuthServiceTests
        {
            private ToDoContext _context;
            private AuthService _authService;

            [TestCleanup]
            public void Cleanup()
            {
                if (_context != null)
                {
                    _context.Database.EnsureDeleted(); // Clean up after each test
                    _context.Dispose();
                    _context = null; // Set the context to null to prevent further use
                }
            }

            [TestInitialize]
            public void Setup()
            {
                var options = new DbContextOptionsBuilder<ToDoContext>()
                                .UseInMemoryDatabase(databaseName: "ToDoTestDb")
                                .Options;

                _context = new ToDoContext(options);

                _authService = new AuthService(_context);
            }

            [TestMethod]
            public async Task CreateUserAsync_ShouldReturnError_WhenFirstNameIsNullOrEmpty()
            {
                // Arrange
                var request = new CreateUserRequest
                {
                    FirstName = "",
                    LastName = "Doe",
                    Email = "johndoe@example.com",
                    Password = "password123"
                };

                // Act
                var result = await _authService.CreateUserAsync(request);

                // Assert
                Assert.IsFalse(result.Success);
                Assert.AreEqual(1000, result.ErrorNumber);
                Assert.AreEqual("First name is required.", result.ErrorDescription);
            }

            [TestMethod]
            public async Task CreateUserAsync_ShouldReturnError_WhenLastNameIsNullOrEmpty()
            {
                // Arrange
                var request = new CreateUserRequest
                {
                    FirstName = "John",
                    LastName = "",
                    Email = "johndoe@example.com",
                    Password = "password123"
                };

                // Act
                var result = await _authService.CreateUserAsync(request);

                // Assert
                Assert.IsFalse(result.Success);
                Assert.AreEqual(1001, result.ErrorNumber);
                Assert.AreEqual("Last name is required.", result.ErrorDescription);
            }

            [TestMethod]
            public async Task CreateUserAsync_ShouldReturnError_WhenEmailIsInvalid()
            {
                // Arrange
                var request = new CreateUserRequest
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "invalid-email",
                    Password = "password123"
                };

                // Act
                var result = await _authService.CreateUserAsync(request);

                // Assert
                Assert.IsFalse(result.Success);
                Assert.AreEqual(1002, result.ErrorNumber);
                Assert.AreEqual("A valid email address is required.", result.ErrorDescription);
            }

            [TestMethod]
            public async Task CreateUserAsync_ShouldReturnError_WhenPasswordIsNullOrEmpty()
            {
                // Arrange
                var request = new CreateUserRequest
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "johndoe@example.com",
                    Password = ""
                };

                // Act
                var result = await _authService.CreateUserAsync(request);

                // Assert
                Assert.IsFalse(result.Success);
                Assert.AreEqual(1003, result.ErrorNumber);
                Assert.AreEqual("Password is required.", result.ErrorDescription);
            }

            [TestMethod]
            public async Task CreateUserAsync_ShouldReturnError_WhenEmailAlreadyExists()
            {
                // Arrange
                var existingUser = new User
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "johndoe@example.com",
                    UserId = Guid.NewGuid().ToString(),
                    Password = "hashedpassword"
                };

                _context.Users.Add(existingUser);
                _context.SaveChanges();

                var request = new CreateUserRequest
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "johndoe@example.com",
                    Password = "password123"
                };

                // Act
                var result = await _authService.CreateUserAsync(request);

                // Assert
                Assert.IsFalse(result.Success);
                Assert.AreEqual(400, result.ErrorNumber);
                Assert.AreEqual("User with this email already exists.", result.ErrorDescription);
            }

            [TestMethod]
            public async Task CreateUserAsync_ShouldReturnSuccess_WhenUserIsCreatedSuccessfully()
            {
                // Arrange
                var request = new CreateUserRequest
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "newuser@example.com",
                    Password = "password123"
                };

                // Act
                var result = await _authService.CreateUserAsync(request);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.UserId);
            }
        }
    }

}
