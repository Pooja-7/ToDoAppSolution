using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoApi.Services;

namespace ToDoTests.Services.ToDoServices
{
    [TestClass]
    public class DeleteToDoItemServiceTests
    {
        private ToDoContext _context;
        private ToDoService _toDoService;

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
            // Ensure database is fresh by deleting it before each test
            var options = new DbContextOptionsBuilder<ToDoContext>()
                            .UseInMemoryDatabase(databaseName: "ToDoTestDb")
                            .Options;

            _context = new ToDoContext(options);

            // Seed some data with unique IDs
            _context.ToDoItems.AddRange(new List<ToDoItem>
            {
                new ToDoItem { Id = 1, Title = "Test ToDo 1", IsCompleted = false, UserId = "test-user-id" },
                new ToDoItem { Id = 2, Title = "Test ToDo 2", IsCompleted = true, UserId = "test-user-id-2" }
            });

            _context.SaveChanges();

            _toDoService = new ToDoService(_context);
        }

        [TestMethod]
        public async Task DeleteToDoItemByUserIdAsync_ShouldReturnSuccess_WhenItemIsDeleted()
        {
            // Arrange
            string userId = "test-user-id";
            int toDoItemId = 1;

            // Act
            var result = await _toDoService.DeleteToDoItemByUserIdAsync(userId, toDoItemId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Deleted Successfully", result.ErrorDescription);
            Assert.AreEqual(0, result.ErrorNumber);
        }

        [TestMethod]
        public async Task DeleteToDoItemByUserIdAsync_ShouldReturnError_WhenItemIsNotFound()
        {
            // Arrange
            string userId = "test-user-id";
            int nonExistentItemId = 999;

            // Act
            var result = await _toDoService.DeleteToDoItemByUserIdAsync(userId, nonExistentItemId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(307, result.ErrorNumber); // ToDo item not found
            Assert.AreEqual("ToDo item not found.", result.ErrorDescription);
        }

        [TestMethod]
        public async Task DeleteToDoItemByUserIdAsync_ShouldReturnError_WhenUserIdDoesNotMatch()
        {
            // Arrange
            string wrongUserId = "wrong-user-id";
            int toDoItemId = 1;

            // Act
            var result = await _toDoService.DeleteToDoItemByUserIdAsync(wrongUserId, toDoItemId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(308, result.ErrorNumber); // User ID does not match
            Assert.AreEqual("User ID does not match. Cannot delete ToDo item.", result.ErrorDescription);
        }

        [TestMethod]
        public async Task DeleteToDoItemByUserIdAsync_ShouldReturnError_WhenUserIdIsNullOrEmpty()
        {
            // Arrange
            string emptyUserId = "";
            int toDoItemId = 1;

            // Act
            var result = await _toDoService.DeleteToDoItemByUserIdAsync(emptyUserId, toDoItemId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(400, result.ErrorNumber); // Error for empty user ID
            Assert.AreEqual("User ID cannot be null or empty.", result.ErrorDescription);
        }
    }
}
