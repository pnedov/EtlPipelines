using PNedov.IsiMarkets.EtlPipeline.Controllers;
using Microsoft.AspNetCore.Mvc;
using PNedov.IsiMarkets.EtlPipeline.Repositories;
using PNedov.IsiMarkets.EtlPipeline.Models;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace PNedov.IsiMarkets.EtlPipeline.UnitTests.Controllers
{
    namespace PNedov.IsiMarkets.EtlPipeline.UnitTests.Controllers
    {
        public class SystemControllerTests
        {
            private readonly Mock<ISystemRepository> _mockRepo;
            private readonly Mock<ILogger<SystemController>> _mockLogger;
            private readonly SystemController _controller;

            public SystemControllerTests()
            {
                _mockRepo = new Mock<ISystemRepository>();
                _mockLogger = new Mock<ILogger<SystemController>>();
                _controller = new SystemController(_mockRepo.Object, _mockLogger.Object);
            }

            [Fact]
            public async Task GetSystemConfiguration_ReturnsRedirectToActionResult()
            {
                // Arrange
                var cancellationToken = new CancellationToken();

                _mockRepo.Setup(repo => repo.GetSystemConfiguration(cancellationToken))
                    .ReturnsAsync(new SystemConfigurations());

                // Act
                var result = await _controller.GetSystemConfiguration(cancellationToken);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
                Assert.Equal("System", redirectToActionResult.ControllerName);
            }

            [Fact]
            public async Task UpsertSystemConfiguration_ReturnsRedirectToActionResult()
            {
                // Arrange
                var newConfig = new SystemConfigurations();
                var cancellationToken = new CancellationToken();

                _mockRepo.Setup(repo => repo.UpsertSystemConfiguration(newConfig, cancellationToken))
                         .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.UpsertSystemConfiguration(newConfig, cancellationToken);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
                Assert.Equal("System", redirectToActionResult.ControllerName);
            }

            [Fact]
            public async Task InitializeDatabase_ReturnsRedirectToActionResult()
            {
                // Arrange
                var cancellationToken = new CancellationToken();

                _mockRepo.Setup(repo => repo.InitializeDatabase(cancellationToken))
                         .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.InitializeDatabase(cancellationToken);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
                Assert.Equal("Customers", redirectToActionResult.ControllerName);
            }
        }
    }
}
