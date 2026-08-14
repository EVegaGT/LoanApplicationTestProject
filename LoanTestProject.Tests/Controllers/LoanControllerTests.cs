using Application.DTOS;
using Application.Services.Interfaces;
using LoanTestProject.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LoanTestProject.Tests.Controllers
{
    public class LoanControllerTests
    {
        private readonly Mock<ILoanService> _mockLoanService;
        private readonly LoanController _controller;

        public LoanControllerTests()
        {
            _mockLoanService = new Mock<ILoanService>();
            _controller = new LoanController(_mockLoanService.Object);
        }

        #region RequestLoan - Deny State Tests

        [Fact]
        public async Task RequestLoan_WithDeniedState_ReturnUnprocessableEntity()
        {
            // Arrange
            var request = new RequestLoanApplication(
                FirstName: "John",
                LastName: "Doe",
                Ssn: "111-11-1111",
                Address: "123 Main St",
                State: "NY",
                CompanyName: "Acme Corp",
                RequestedAmount: 50000
            );

            var denialReason = "We do not operate in this state.";
            _mockLoanService
                .Setup(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()))
                .ReturnsAsync(ResponseResult.Failure(denialReason));

            // Act
            var result = await _controller.RequestLoan(request);

            // Assert
            var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal(422, unprocessableResult.StatusCode);

            var responseObject = unprocessableResult.Value;
            Assert.NotNull(responseObject);

            var objType = responseObject.GetType();
            var statusProperty = objType.GetProperty("status");
            var reasonProperty = objType.GetProperty("reason");

            Assert.NotNull(statusProperty);
            Assert.NotNull(reasonProperty);
            Assert.Equal("Denied", statusProperty?.GetValue(responseObject));
            Assert.Equal(denialReason, reasonProperty?.GetValue(responseObject));

            _mockLoanService.Verify(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()), Times.Once);
        }

        [Theory]
        [InlineData("NY")]
        [InlineData("ny")]
        [InlineData("Ny")]
        public async Task RequestLoan_WithVariousDeniedStateFormats_ReturnUnprocessableEntity(string deniedState)
        {
            // Arrange
            var request = new RequestLoanApplication(
                FirstName: "Jane",
                LastName: "Smith",
                Ssn: "222-22-2222",
                Address: "456 Oak Ave",
                State: deniedState,
                CompanyName: "Tech Solutions",
                RequestedAmount: 75000
            );

            var denialReason = "We do not operate in this state.";
            _mockLoanService
                .Setup(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()))
                .ReturnsAsync(ResponseResult.Failure(denialReason));

            // Act
            var result = await _controller.RequestLoan(request);

            // Assert
            var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal(422, unprocessableResult.StatusCode);

            _mockLoanService.Verify(x => x.ProcessApplicationAsync(request), Times.Once);
        }

        #endregion

        #region RequestLoan - Blacklist SSN Tests

        [Fact]
        public async Task RequestLoan_WithBlacklistedSSN_ReturnUnprocessableEntity()
        {
            // Arrange
            var blacklistedSSN = "123-45-6789";
            var request = new RequestLoanApplication(
                FirstName: "Bob",
                LastName: "Johnson",
                Ssn: blacklistedSSN,
                Address: "789 Pine Rd",
                State: "CA",
                CompanyName: "Finance Corp",
                RequestedAmount: 100000
            );

            var denialReason = "This SSN is blacklisted.";
            _mockLoanService
                .Setup(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()))
                .ReturnsAsync(ResponseResult.Failure(denialReason));

            // Act
            var result = await _controller.RequestLoan(request);

            // Assert
            var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal(422, unprocessableResult.StatusCode);

            var responseObject = unprocessableResult.Value;
            Assert.NotNull(responseObject);

            var objType = responseObject.GetType();
            var statusProperty = objType.GetProperty("status");
            var reasonProperty = objType.GetProperty("reason");

            Assert.NotNull(statusProperty);
            Assert.NotNull(reasonProperty);
            Assert.Equal("Denied", statusProperty?.GetValue(responseObject));
            Assert.Equal(denialReason, reasonProperty?.GetValue(responseObject));

            _mockLoanService.Verify(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()), Times.Once);
        }

        [Theory]
        [InlineData("123-45-6789")]
        [InlineData("987-65-4321")]
        public async Task RequestLoan_WithAnyBlacklistedSSN_ReturnUnprocessableEntity(string blacklistedSSN)
        {
            // Arrange
            var request = new RequestLoanApplication(
                FirstName: "Test",
                LastName: "User",
                Ssn: blacklistedSSN,
                Address: "999 Test St",
                State: "TX",
                CompanyName: "Test Co",
                RequestedAmount: 50000
            );

            var denialReason = "This SSN is blacklisted.";
            _mockLoanService
                .Setup(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()))
                .ReturnsAsync(ResponseResult.Failure(denialReason));

            // Act
            var result = await _controller.RequestLoan(request);

            // Assert
            var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal(422, unprocessableResult.StatusCode);

            _mockLoanService.Verify(x => x.ProcessApplicationAsync(request), Times.Once);
        }

        #endregion

        #region RequestLoan - Success Tests

        [Fact]
        public async Task RequestLoan_WithValidApplicationAndApprovedState_ReturnOk()
        {
            // Arrange
            var request = new RequestLoanApplication(
                FirstName: "Alice",
                LastName: "Williams",
                Ssn: "333-33-3333",
                Address: "321 Elm St",
                State: "CA",
                CompanyName: "Good Corp",
                RequestedAmount: 50000
            );

            _mockLoanService
                .Setup(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()))
                .ReturnsAsync(ResponseResult.Success());

            // Act
            var result = await _controller.RequestLoan(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            _mockLoanService.Verify(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()), Times.Once);
        }

        [Fact]
        public async Task RequestLoan_WithNonBlacklistedSSNAndAllowedState_ReturnOk()
        {
            // Arrange
            var request = new RequestLoanApplication(
                FirstName: "Charlie",
                LastName: "Brown",
                Ssn: "555-55-5555",
                Address: "555 Maple Dr",
                State: "FL",
                CompanyName: "Happy Inc",
                RequestedAmount: 60000
            );

            _mockLoanService
                .Setup(x => x.ProcessApplicationAsync(request))
                .ReturnsAsync(ResponseResult.Success());

            // Act
            var result = await _controller.RequestLoan(request);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockLoanService.Verify(x => x.ProcessApplicationAsync(request), Times.Once);
        }

        #endregion

        #region RequestLoan - Exception Handling Tests

        [Fact]
        public async Task RequestLoan_WhenServiceThrowsException_ReturnProblemResult()
        {
            // Arrange
            var request = new RequestLoanApplication(
                FirstName: "Error",
                LastName: "Test",
                Ssn: "000-00-0000",
                Address: "Error Lane",
                State: "CO",
                CompanyName: "Error Inc",
                RequestedAmount: 25000
            );

            var exceptionMessage = "Database connection failed";
            _mockLoanService
                .Setup(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.RequestLoan(request);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);

            _mockLoanService.Verify(x => x.ProcessApplicationAsync(It.IsAny<RequestLoanApplication>()), Times.Once);
        }

        #endregion
    }
}
