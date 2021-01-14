using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.EntityFrameworkCore;
using Moq;
using ShoppingSystem.Data;
using ShoppingSystem.Models;
using ShoppingSystem.Services;
using Xunit;

namespace ShoppingSystemTests
{
    public class CustomerServiceTests
    {
        private List<Customer> data = new List<Customer>
        {
            new Customer {Id = 1, FirstName = "Ramil", LastName = "Naum", Address = "Los-Ang", Discount = "5"},
            new Customer {Id = 2, FirstName = "Bob", LastName = "Dillan", Address = "Berlin", Discount = "7"},
            new Customer {Id = 3, FirstName = "Kile", LastName = "Rise", Address = "London", Discount = "0"},
            new Customer {Id = 4, FirstName = "John", LastName = "Konor", Address = "Vashington", Discount = "3"},
            new Customer {Id = 5, FirstName = "Tony", LastName = "Stark", Address = "Florida", Discount = "5"},
            new Customer {Id = 6, FirstName = "Jamie", LastName = "Lanister", Address = "Westerros", Discount = "10"},
        };

        private Mock<DbSet<Customer>> CreateDbSet()
        {
            var dbSet = new Mock<DbSet<Customer>>();

            dbSet.As<IQueryable<Customer>>()
                .Setup(x => x.Provider)
                .Returns(data.AsQueryable().Provider);
            dbSet.As<IQueryable<Customer>>()
                .Setup(x => x.ElementType)
                .Returns(data.AsQueryable().ElementType);
            dbSet.As<IQueryable<Customer>>()
                .Setup(x => x.Expression)
                .Returns(data.AsQueryable().Expression);
            dbSet.As<IQueryable<Customer>>()
                .Setup(x => x.GetEnumerator())
                .Returns(data.GetEnumerator());
            dbSet.Setup(m => m.Remove(It.IsAny<Customer>())).Callback<Customer>((entity) => data.Remove(entity));
            return dbSet;
        }

        private Mock<ShoppingContext> CreateDbContext(Mock<DbSet<Customer>> dbSet)
        {
            var context = new Mock<ShoppingContext>();
            context.Setup(c => c.Customers).Returns(dbSet.Object);
            return context;
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnCutomers()
        {
            // Arrange
            var dbSet = CreateDbSet();
            var context = CreateDbContext(dbSet);
            var service = new CustomersService(context.Object);
            //Act
            var result = await service.GetAllAsync();
            // Assert
            Assert.Equal(data.Count, result.Count);
        }


        [Fact]
        public async Task GetByIdAsync_WithCorrectId_ShouldReturnCutomer()
        {
            var dbSet = CreateDbSet();
            var context = CreateDbContext(dbSet);
            var service = new CustomersService(context.Object);

            var customerId = 1;

            var customer = await service.GetByIdAsync(1);
            Assert.Equal(customerId, customer.Id);
        }
        [Fact]
        public async Task GetByIdAsync_WithInCorrectId_ShouldThrowException()
        {
            var dbSet = CreateDbSet();
            var context = CreateDbContext(dbSet);
            var service = new CustomersService(context.Object);

            var taken = Assert.ThrowsAsync<Exception>(async () => await service.GetByIdAsync(0));
            var ex = await taken;
            Assert.Equal("On this Id nothing found", ex.Message);
        }

        [Fact]
        public async Task AddAsync_ShouldCreateNewCustomer()
        {
            var dbSet = CreateDbSet();
            var dbContext = CreateDbContext(dbSet);
            var service = new CustomersService(dbContext.Object);
            var customer = data[1];
            await service.AddAsync(customer);
            dbSet.Verify(m => m.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once());
            dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteAsyncById_WithCorrectId_ShouldRemoveRecord()
        {
            var dbSet = CreateDbSet();
            var dbContext = CreateDbContext(dbSet);
            var service = new CustomersService(dbContext.Object);
            int idToDelete = 3;
            await service.DeleteAsync(idToDelete);

            dbSet.Verify(x => x.Remove(It.IsAny<Customer>()), Times.Once());
            dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
        [Fact]
        public async Task DeleteAsyncById_WithIncorrectId_ShouldThrowException()
        {
            var dbSet = CreateDbSet();
            var dbContext = CreateDbContext(dbSet);
            var service = new CustomersService(dbContext.Object);

            var removing = Assert.ThrowsAsync<Exception>(() => service.DeleteAsync(0));
            var ex = await removing;
            Assert.Equal("On this Id nothing found", ex.Message);
        }

        [Fact]
        public async Task EditAsync_WithIncorrectId_ShouldThrowException()
        {
            var dbSet = CreateDbSet();
            var dbContext = CreateDbContext(dbSet);
            var service = new CustomersService(dbContext.Object);
            var cust = new Customer
                {Id = 0, FirstName = "Baley", LastName = "Naum", Address = "Los-Ang", Discount = "5"};
            var edited = Assert.ThrowsAsync<Exception>(async () => await service.EditAsync(cust));
            var ex = await edited;
            Assert.Equal("On this Id nothing found", ex.Message);
        }

        [Fact]
        public async Task EditAsync_WithCorrectId_ShouldReturnUpdatedCustomer()
        {
            var dbSet = CreateDbSet();
            var dbContext = CreateDbContext(dbSet);
            var service = new CustomersService(dbContext.Object);

            await service.EditAsync(new Customer { Id = 1, FirstName = "Baley", LastName = "Hensorskyi", Address = "Los-Ang", Discount = "5" });

            dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            Assert.Equal("Baley", data[0].FirstName);
            Assert.Equal("Hensorskyi", data[0].LastName);
        }

    }
}