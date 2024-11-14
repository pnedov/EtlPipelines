using PNedov.IsiMarkets.EtlPipeline.Controllers;
using Microsoft.AspNetCore.Mvc;
using PNedov.IsiMarkets.EtlPipeline.Repositories;
using PNedov.IsiMarkets.EtlPipeline.Models;
using Moq;
using Xunit;

namespace PNedov.IsiMarkets.EtlPipeline.UnitTests.Controllers
{
    public class CustomersControllerTests
    {
        private readonly CustomersController _controller;
        private readonly Mock<ICustomersRepository> _mockRepository;

        public CustomersControllerTests()
        {
            _mockRepository = new Mock<ICustomersRepository>();
            _controller = new CustomersController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetCustomers_ReturnsOkResult_WithListOfCustomers()
        {
            // Arrange
            var customers = new List<Customers> { new Customers { Id = 1, FirstName = "John", LastName = "Doe" } };
            _mockRepository.Setup(repo => repo.GetCustomersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(customers);

            // Act
            var result = await _controller.GetCustomers(0, 10, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Customers>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetCustomerTransactions_ReturnsOkResult_WithListOfTransactions()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var transactions = new List<CustomerTransactions> { new CustomerTransactions { Id = 1, CustomerId = 1 } };
            _mockRepository.Setup(repo => repo.GetCustomerTransactionsAsync(customerId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetCustomerTransactions(customerId, 0, 10, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CustomerTransactions>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task UpsertCustomer_ReturnsOkResult_WithResult()
        {
            // Arrange
            var customer = new Customers { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockRepository.Setup(repo => repo.UpsertCustomerAsync(customer, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(1);

            // Act
            var result = await _controller.UpsertCustomer(customer, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<int>(okResult.Value);
            Assert.Equal(1, returnValue);
        }

        [Fact]
        public async Task UpsertTransaction_ReturnsOkResult_WithResult()
        {
            // Arrange
            var transaction = new CustomerTransactions { Id = 1, CustomerId = 1 };
            _mockRepository.Setup(repo => repo.UpsertTransctionAsync(transaction, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(1);

            // Act
            var result = await _controller.UpsertTransaction(Guid.NewGuid(), Guid.NewGuid(), transaction, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<int>(okResult.Value);
            Assert.Equal(1, returnValue);
        }
    }
}
