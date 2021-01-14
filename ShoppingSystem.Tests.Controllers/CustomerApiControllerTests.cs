using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ShoppingSystem.Api;
using ShoppingSystem.Models;
using ShoppingSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingSystem.Tests.Controllers
{
    [TestFixture]
    class CustomerApiControllerTests
    {
        private CustomerController _customerApiController;
        private Mock<ICustomers> _mockCustomerService;

        [SetUp]
        public void Setup()
        {
            _mockCustomerService = new Mock<ICustomers>();
            _customerApiController = new CustomerController(_mockCustomerService.Object);
            _mockCustomerService.Setup(service => service.GetAllAsync())
                .ReturnsAsync(new List<Customer>(new Customer[]
                {
                    new Customer {FirstName = "Ramil", LastName = "Naum", Address = "Los-Ang", Discount = "5"},
                    new Customer {FirstName = "Bob", LastName = "Dillan", Address = "Berlin", Discount = "7"},
                }));
        }

        #region Get 
        [Test]
        public async Task Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = await _customerApiController.GetAllCustomers();
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(okResult.Result);
        }

        [Test]
        public async Task Get_WhenCalled_ReturnsAllCustomers()
        {
            // Assert
            var okResult = (await _customerApiController.GetAllCustomers()).Result as OkObjectResult;
            // Assert
            Assert.IsInstanceOf<IEnumerable<Customer>>(okResult.Value);
            Assert.AreEqual(2, ((IEnumerable<Customer>)okResult.Value).Count());
        }

        #endregion

        #region GetById
        [TestCase(5)]
        public async Task GetById_UnknownIdPassed_ReturnsNotFoundResult(int id)
        {
            _mockCustomerService.Setup(service => service.GetByIdAsync(id))
                .ReturnsAsync((Customer)null);

            var result = await _customerApiController.GetCustomer(id);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        #endregion
    }
}
