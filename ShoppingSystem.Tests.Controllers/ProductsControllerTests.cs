using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ShoppingSystem.Controllers;
using ShoppingSystem.Models;
using ShoppingSystem.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingSystem.Tests.Controllers
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private ProductsController _productController;
        private Mock<IProducts> _mockProductsService;

        [SetUp]
        public void Setup()
        {
            _mockProductsService = new Mock<IProducts>();
            _productController = new ProductsController(_mockProductsService.Object);
        }

        #region Index
        [Test]
        public async Task Index_WithData_ReturnsView()
        {
            _mockProductsService.Setup(service => service.GetAllAsync())
                .ReturnsAsync(new List<Product>(new Product[]
                {
                    new Product { Id=1, Name="first", Price=10 },
                    new Product { Id=2, Name="second", Price=10 }
                }));

            var result = await _productController.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Index_WithNull_ReturnsView()
        {
            _mockProductsService.Setup(service => service.GetAllAsync())
                .ReturnsAsync((IList<Product>)null);

            var result = await _productController.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Index_UsesServiceGetAll()
        {
            _mockProductsService.Setup(service => service.GetAllAsync());

            await _productController.Index();

            _mockProductsService.Verify(service => service.GetAllAsync(), Times.Once());
        }
        #endregion

        #region Details
        [TestCase(2)]
        public async Task Details_ProductExists_ReturnsView(int id)
        {
            _mockProductsService.Setup(service => service.GetByIdAsync(id))
                .ReturnsAsync(new Product { Id = id });

            var result = await _productController.Details(id);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(2)]
        public async Task Details_ProductNotFound_ReturnsBadRequest(int id)
        {
            _mockProductsService.Setup(service => service.GetByIdAsync(id))
                .ReturnsAsync((Product)null);

            var result = await _productController.Details(id);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase(2)]
        public async Task Details_WithException_ReturnsBadRequest(int id)
        {
            _mockProductsService.Setup(service => service.GetByIdAsync(id))
                .ThrowsAsync(new Exception());

            var result = await _productController.Details(id);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase(2)]
        public async Task Details_UsesServiceGetById(int id)
        {
            _mockProductsService.Setup(service => service.GetByIdAsync(id));

            await _productController.Details(id);

            _mockProductsService.Verify(service => service.GetByIdAsync(id), Times.Once);
        }
        #endregion

        #region Create
        [Test]
        public void GET_Create_ReturnsView()
        {
            var result = _productController.Create();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Create_WithValidData_ReturnsRedirectToAction(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.AddAsync(product));

            var result = await _productController.Create(product);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Create_WithInvalidData_ReturnsView(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.AddAsync(product));
            _productController.ModelState.AddModelError(nameof(name), "Invalid name!");

            var result = await _productController.Create(product);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Create_ThrowsException_ReturnsView(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.AddAsync(product))
                .ThrowsAsync(new Exception());

            var result = await _productController.Create(product);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Create_WithData_UsesService(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.AddAsync(product));

            var result = await _productController.Create(product);

            _mockProductsService.Verify(service => service.AddAsync(product), Times.Once);
        }
        #endregion

        #region Edit
        [TestCase(2)]
        public async Task GET_Edit_ProductExists_ReturnsView(int id)
        {
            _mockProductsService.Setup(service => service.GetByIdAsync(id))
                .ReturnsAsync(new Product { Id = id });

            var result = await _productController.Edit(id);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(2)]
        public async Task GET_Edit_ThrowsException_ReturnsBadRequest(int id)
        {
            _mockProductsService.Setup(service => service.GetByIdAsync(id))
                .ThrowsAsync(new Exception());

            var result = await _productController.Edit(id);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase(2)]
        public async Task GET_Edit_ProductNotFound_ReturnsBadRequest(int id)
        {
            _mockProductsService.Setup(service => service.GetByIdAsync(id))
                .ReturnsAsync((Product)null);

            var result = await _productController.Edit(id);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase(2)]
        public async Task GET_Edit_UsesServiceGetById(int id)
        {
            _mockProductsService.Setup(service => service.GetByIdAsync(id));

            await _productController.Edit(id);

            _mockProductsService.Verify(service => service.GetByIdAsync(id), Times.Once);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Edit_WithValidData_ReturnsRedirectToAction(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.EditAsync(product));

            var result = await _productController.Edit(id, product);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Edit_ThrowsException_ReturnsBadRequest(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.EditAsync(product))
                .ThrowsAsync(new Exception());

            var result = await _productController.Edit(id, product);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Edit_WithInvalidData_ReturnsView(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.EditAsync(product));
            _productController.ModelState.AddModelError(nameof(name), "Invalid name!");

            var result = await _productController.Edit(id, product);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Edit_WithDifferentIDs_ReturnsBadRequest(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.EditAsync(product));

            var result = await _productController.Edit(id + 1, product);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase(1, "hello world", 2.2f)]
        public async Task POST_Edit_WithData_UsesServiceEdit(int id, string name, float price)
        {
            Product product = new Product { Id = id, Name = name, Price = price };
            _mockProductsService.Setup(service => service.EditAsync(product));

            await _productController.Edit(id, product);

            _mockProductsService.Verify(service => service.EditAsync(product), Times.Once);
        }
        #endregion

        #region Delete
        [TestCase(2)]
        public async Task Delete_ReturnsRedirectToAction(int id)
        {
            _mockProductsService.Setup(service => service.DeleteAsync(id));

            var result = await _productController.Delete(id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [TestCase(2)]
        public async Task Delete_ReturnsView(int id)
        {
            _mockProductsService.Setup(service => service.DeleteAsync(id)).ThrowsAsync(new Exception());

            var result = await _productController.Delete(id);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(2)]
        public async Task GET_Delete_DontUseServiceDelete(int id)
        {
            _mockProductsService.Setup(service => service.DeleteAsync(id));

            await _productController.Delete(id);

            _mockProductsService.Verify(service => service.DeleteAsync(id), Times.Never);
        }

        [TestCase(2)]
        public async Task POST_DeleteConfirmed_ReturnsRedirectToAction(int id)
        {
            _mockProductsService.Setup(service => service.DeleteAsync(id));

            var result = await _productController.DeleteConfirmed(id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [TestCase(2)]
        public async Task POST_DeleteConfirmed_ReturnsView(int id)
        {
            _mockProductsService.Setup(service => service.DeleteAsync(id)).Throws(new Exception());

            var result = await _productController.DeleteConfirmed(id);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(2)]
        public async Task POST_DeleteConfirmed_UsesServiceDelete(int id)
        {
            _mockProductsService.Setup(service => service.DeleteAsync(id)).Throws(new Exception());

            var result = await _productController.DeleteConfirmed(id);

            _mockProductsService.Verify(service => service.DeleteAsync(id));
        }
        #endregion
    }
}