using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoApi.Services;

namespace ToDoTests.Services
{
    [TestClass]
    public class ToDoServiceTests
    {
        private ToDoContext _context;
        private ToDoService _toDoService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase" + Guid.NewGuid())
                .Options;

            _context = new ToDoContext(options);
            _toDoService = new ToDoService(_context);
        }

        [TestMethod]
        public async Task UpdateToDoItemAsync_ShouldReturnSuccess_WhenItemIsUpdated()
        {
            // Arrange
            var userId = "test-user-id";
            var existingToDoItem = new ToDoItem { Id = 1, Title = "Old Title", IsCompleted = false, UserId = userId };
            _context.ToDoItems.Add(existingToDoItem);
            await _context.SaveChangesAsync();

            var updatedToDoItem = new ToDoItem { Id = 1, Title = "New Title", IsCompleted = true, UserId = userId };

            // Act
            var result = await _toDoService.UpdateToDoItemAsync(userId, 1, updatedToDoItem);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("New Title", existingToDoItem.Title);
            Assert.IsTrue(existingToDoItem.IsCompleted);
            Assert.AreEqual(0, result.ErrorNumber);
            Assert.AreEqual("Sucess", result.ErrorDescription);
        }

        [TestMethod]
        public async Task UpdateToDoItemAsync_ShouldReturnError_WhenIdMismatch()
        {
            // Arrange
            var userId = "test-user-id";
            var existingToDoItem = new ToDoItem { Id = 1, Title = "Old Title", IsCompleted = false, UserId = userId };
            _context.ToDoItems.Add(existingToDoItem);
            await _context.SaveChangesAsync();

            var updatedToDoItem = new ToDoItem { Id = 2, Title = "New Title", IsCompleted = true, UserId = userId };

            // Act
            var result = await _toDoService.UpdateToDoItemAsync(userId, 1, updatedToDoItem);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(304, result.ErrorNumber);
            Assert.AreEqual("ID mismatch. Cannot update ToDo item.", result.ErrorDescription);
        }

        [TestMethod]
        public async Task UpdateToDoItemAsync_ShouldReturnError_WhenToDoItemNotFound()
        {
            // Arrange
            var userId = "test-user-id";
            var updatedToDoItem = new ToDoItem { Id = 1, Title = "New Title", IsCompleted = true, UserId = userId };

            // Act
            var result = await _toDoService.UpdateToDoItemAsync(userId, 1, updatedToDoItem);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(301, result.ErrorNumber);
            Assert.AreEqual("ToDo item not found.", result.ErrorDescription);
        }

        [TestMethod]
        public async Task UpdateToDoItemAsync_ShouldReturnError_WhenUserIdDoesNotMatch()
        {
            // Arrange
            var userId = "test-user-id";
            var existingToDoItem = new ToDoItem { Id = 1, Title = "Old Title", IsCompleted = false, UserId = "another-user-id" };
            _context.ToDoItems.Add(existingToDoItem);
            await _context.SaveChangesAsync();

            var updatedToDoItem = new ToDoItem { Id = 1, Title = "New Title", IsCompleted = true, UserId = userId };

            // Act
            var result = await _toDoService.UpdateToDoItemAsync(userId, 1, updatedToDoItem);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(302, result.ErrorNumber);
            Assert.AreEqual("User ID does not match. Cannot update ToDo item.", result.ErrorDescription);
        }
    }
}
