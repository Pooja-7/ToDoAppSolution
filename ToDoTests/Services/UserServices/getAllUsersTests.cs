using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToDoTests.Services.UserServices
{
    [TestClass]
    public class UserServiceTests
    {
        private ToDoContext _context; // Assuming you have a DbContext for your application
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use an in-memory database for testing
                .Options;

            _context = new ToDoContext(options);
            _userService = new UserService(_context);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ShouldReturnSuccess_WhenUsersAreRetrieved()
        {
            // Arrange
            var users = new List<User>
            {
                 new User { UserId = "1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Password = BCrypt.Net.BCrypt.HashPassword("password1") },
                 new User { UserId = "2", Email = "user2@example.com", FirstName = "Jane", LastName = "Doe", Password = BCrypt.Net.BCrypt.HashPassword("password2") }
             };
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.IsTrue(result.Success); // Check if the operation was successful
            Assert.AreEqual(2, result.Data.Count); // Ensure the correct number of users
            Assert.AreEqual("Success", result.ErrorDescription); // Assuming you set this property in your ServiceResponse
            Assert.AreEqual("user1@example.com", result.Data.First().Email); // Verify the first user
            Assert.AreEqual("user2@example.com", result.Data.Last().Email); // Verify the second user
        }

    }
}
