using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using ToDoApi.Services;

namespace YourProjectNamespace.Services
{
    [TestClass]
    public class GetToDoItemByIdServiceTests
    {
        private ToDoContext _context;
        private ToDoService _toDoService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ToDoContext(options);
            _toDoService = new ToDoService(_context);
        }

        [TestMethod]
        public async Task GetToDoItemByIdAsync_ShouldReturnError_WhenItemDoesNotExist()
        {
            // Arrange
            int nonExistingId = 999;

            // Act
            var result = await _toDoService.GetToDoItemByIdAsync(nonExistingId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(301, result.ErrorNumber);
            Assert.AreEqual($"ToDo item with ID {nonExistingId} not found.", result.ErrorDescription);
        }

        [TestMethod]
        public async Task GetToDoItemByIdAsync_ShouldReturnSuccess_WhenItemExists()
        {
            // Arrange
            var toDoItem = new ToDoItem
            {
                Id = 1,
                Title = "Test ToDo Item",
                IsCompleted = false,
                UserId = "some-user-id" // Set the required UserId property
            };
            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await _toDoService.GetToDoItemByIdAsync(toDoItem.Id);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(toDoItem.Id, result.Data.Id);
            Assert.AreEqual(toDoItem.Title, result.Data.Title);
        }
    }
}
