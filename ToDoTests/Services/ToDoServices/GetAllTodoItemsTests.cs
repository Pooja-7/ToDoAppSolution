using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoApi.Services;

namespace ToDoTests.Services.ToDoServices
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
        public async Task GetAllToDoItemsAsync_ShouldReturnSuccess_WhenItemsAreRetrieved()
        {
            // Arrange
            var toDoItems = new List<ToDoItem>
            {
                new ToDoItem { Id = 1, Title = "Test ToDo 1", IsCompleted = false, UserId = "test-user-id" },
                new ToDoItem { Id = 2, Title = "Test ToDo 2", IsCompleted = true, UserId = "test-user-id" }
            };
            _context.ToDoItems.AddRange(toDoItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _toDoService.GetAllToDoItemsAsync();

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.Data.Count()); // Ensure the correct number of items
            Assert.AreEqual("Success", result.ErrorDescription); // Ensure the success message
            Assert.AreEqual("Test ToDo 1", result.Data.First().Title); // Verify the first item
            Assert.AreEqual("Test ToDo 2", result.Data.Last().Title); // Verify the second item
        }
    }
}
