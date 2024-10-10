using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using ToDoApi.Services;

namespace ToDoTests.Services.ToDoServices
{
    [TestClass]
    public class CreateToDoServiceTests
    {
        private ToDoContext _context; // Replace the mock context with the actual context
        private ToDoService _toDoService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use an in-memory database
                .Options;

            _context = new ToDoContext(options);
            _toDoService = new ToDoService(_context);
        }

        [TestMethod]
        public async Task CreateToDoItemAsync_ShouldReturnSuccess_WhenItemIsAdded()
        {
            // Arrange
            var newToDoItem = new ToDoItem
            {
                Title = "Test ToDo",
                IsCompleted = false,
                UserId = "test-user-id"
                // Do not set Id; let it be auto-generated
            };

            // Act
            var result = await _toDoService.CreateToDoItemAsync(newToDoItem);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data); // Ensure the Data is not null
            Assert.AreEqual(newToDoItem.Title, result.Data.Title);
            Assert.AreNotEqual(0, result.Data.Id); // Ensure that Id is generated and not zero
        }


        [TestMethod]
        public async Task CreateToDoItemAsync_ShouldReturnError_WhenExceptionOccurs()
        {
            // Arrange
            var newToDoItem = new ToDoItem { Id = 1, Title = "Test ToDo", IsCompleted = false, UserId = "test-user-id" };

            // Simulate exception
            _context.Database.EnsureDeleted(); // Reset the database
            _context.Database.EnsureCreated();

            _context.Add(newToDoItem);
            await _context.SaveChangesAsync(); // This should succeed

            // Act
            var result = await _toDoService.CreateToDoItemAsync(newToDoItem); // This should not throw an exception

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(303, result.ErrorNumber);
        }

        [TestMethod]
        public async Task CreateToDoItemAsync_ShouldReturnError_WhenGeneralExceptionOccurs()
        {
            // Arrange
            var newToDoItem = new ToDoItem { Id = 1, Title = "Test ToDo", IsCompleted = false, UserId = "test-user-id" };

            // Simulate an exception by throwing directly in the context
            _context.Database.EnsureDeleted(); // Reset the database
            _context.Database.EnsureCreated();

            _context.Add(newToDoItem);
            await _context.SaveChangesAsync(); // This should succeed

            // Force a general exception
            _context = null; // Set context to null to simulate failure

            // Act
            var result = await _toDoService.CreateToDoItemAsync(newToDoItem); // This should throw an exception

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(303, result.ErrorNumber); // Ensure the error code matches what you expect
        }

    }
}
