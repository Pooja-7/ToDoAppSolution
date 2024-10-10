using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoApi.Services;

namespace ToDoTests.Services.ToDoServices
{
    [TestClass]
    public class GetAllToDoItemsByUserIdServiceTests
    {
        private ToDoContext _context;
        private ToDoService _toDoService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase" + Guid.NewGuid()) // Unique name for each test run
                .Options;

            _context = new ToDoContext(options);
            _toDoService = new ToDoService(_context);
        }

        [TestMethod]
        public async Task GetAllToDoItemsByUserIdAsync_ShouldReturnError_WhenUserIdIsNull()
        {
            // Arrange
            string userId = null;

            // Act
            var result = await _toDoService.GetAllToDoItemsByUserIdAsync(userId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(400, result.ErrorNumber);
            Assert.AreEqual("User ID cannot be null or empty.", result.ErrorDescription);
        }

        [TestMethod]
        public async Task GetAllToDoItemsByUserIdAsync_ShouldReturnError_WhenUserIdIsEmpty()
        {
            // Arrange
            string userId = string.Empty;

            // Act
            var result = await _toDoService.GetAllToDoItemsByUserIdAsync(userId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(400, result.ErrorNumber);
            Assert.AreEqual("User ID cannot be null or empty.", result.ErrorDescription);
        }

        [TestMethod]
        public async Task GetAllToDoItemsByUserIdAsync_ShouldReturnSuccess_WhenItemsAreRetrieved()
        {
            // Arrange
            var userId = "test-user-id";
            var toDoItems = new List<ToDoItem>
            {
                new ToDoItem { Title = "Test ToDo 1", IsCompleted = false, UserId = userId },
                new ToDoItem { Title = "Test ToDo 2", IsCompleted = true, UserId = userId }
            };
            _context.ToDoItems.AddRange(toDoItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _toDoService.GetAllToDoItemsByUserIdAsync(userId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.Data.Count()); // Ensure the correct number of items
            Assert.AreEqual("Success", result.ErrorDescription); // Ensure the success message
            Assert.AreEqual("Test ToDo 1", result.Data.First().Title); // Verify the first item
            Assert.AreEqual("Test ToDo 2", result.Data.Last().Title); // Verify the second item
        }

        [TestMethod]
        public async Task GetAllToDoItemsByUserIdAsync_ShouldReturnEmptyList_WhenNoItemsExistForUserId()
        {
            // Arrange
            var userId = "non-existing-user-id";

            // Act
            var result = await _toDoService.GetAllToDoItemsByUserIdAsync(userId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(0, result.Data.Count()); // Ensure the list is empty
            Assert.AreEqual("Success", result.ErrorDescription); // Ensure the success message
        }

    }
}
