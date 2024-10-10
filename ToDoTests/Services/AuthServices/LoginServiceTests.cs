using Microsoft.EntityFrameworkCore;
using Moq;


namespace ToDoTests.Services.AuthServices
{
    [TestClass]
    public class LoginServiceTests
    {
        private Mock<ToDoContext> _mockContext;
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            // Set up the mock context for testing
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<ToDoContext>(options);
            _authService = new AuthService(_mockContext.Object);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnError_WhenEmailIsInvalid()
        {
            // Arrange
            var request = new LoginRequest { Email = "", Password = "password123" };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(1002, result.ErrorNumber);
            Assert.AreEqual("A valid email address is required.", result.ErrorDescription);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldReturnError_WhenPasswordIsEmpty()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "" };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(1003, result.ErrorNumber);
            Assert.AreEqual("Password is required.", result.ErrorDescription);
        }

        //Need to work on following tests in the end if time permits.

        //[TestMethod]
        //public void IsValidEmail_ShouldReturnTrue_WhenEmailIsValid()
        //{
        //    // Arrange
        //    var validEmail = "test@example.com";

        //    // Act
        //    var result = _authService.IsValidEmail(validEmail);

        //    // Assert
        //    Assert.IsTrue(result);
        //}

        //[TestMethod]
        //public void IsValidEmail_ShouldReturnFalse_WhenEmailIsInvalid()
        //{
        //    // Arrange
        //    var invalidEmail = "invalid-email";

        //    // Act
        //    var result = _authService.IsValidEmail(invalidEmail);

        //    // Assert
        //    Assert.IsFalse(result);
        //}

        //[TestMethod]
        //public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatchesHashedPassword()
        //{
        //    // Arrange
        //    var password = "password123";
        //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        //    // Act
        //    var result = _authService.VerifyPassword(password, hashedPassword);

        //    // Assert
        //    Assert.IsTrue(result);
        //}

        //[TestMethod]
        //public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatchHashedPassword()
        //{
        //    // Arrange
        //    var password = "password123";
        //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword("differentPassword");

        //    // Act
        //    var result = _authService.VerifyPassword(password, hashedPassword);

        //    // Assert
        //    Assert.IsFalse(result);
        //}

        //[TestMethod]
        //public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        //{
        //    // Arrange
        //    var request = new LoginRequest { Email = "test@example.com", Password = "password123" };
        //    var user = new User
        //    {
        //        UserId = "123",
        //        Email = "test@example.com",
        //        Password = BCrypt.Net.BCrypt.HashPassword("password123")
        //    };

        //    // Set up the mock to return the user when searching by email
        //    var data = new List<User> { user }.AsQueryable();
        //    _mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
        //    _mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
        //    _mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
        //    _mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        //    // Act
        //    var result = await _authService.LoginAsync(request);

        //    // Assert
        //    Assert.IsTrue(result.Success);
        //    Assert.AreEqual("123", result.UserId); // Ensure the returned UserId matches
        //}
    }
}
