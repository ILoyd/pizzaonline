using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PizzaOnline.Api.Controllers;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Bll.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaOnline.Tests.Controllers
{
    public class OrderControllerTest
    {
        private Mock<IOrderService> _orderService;

        public OrderControllerTest()
        {
            _orderService = new Mock<IOrderService>();
        }

        [Fact]
        public async Task PostOrder_ReturnOk()
        {
            //Arrange
            var order = new OrderDto();
            var pizzas = new List<PizzaDto>();
            order.Pizzas = pizzas;
            _orderService.Setup(o => o.InsertOrderAsync(order)).Returns(Task.FromResult(order));
            var controller = new OrderController(_orderService.Object);
            var spy = new Mock<OrderController>(controller);

            //Act
            var result = await controller.PostOrder(order);

            //Assert
            result.Result.Should().BeOfType(typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            var value = createdResult?.Value as OrderDto;
            Assert.Equal(order, value);
            _orderService.Verify(s => s.SaveLastOrderToXml(order), Times.Exactly(1));
        }

        [Fact]
        public async Task PostOrder_ReturnBadRequest()
        {
            //Arrange
            OrderDto order = null!;
            _orderService.Setup(s => s.InsertOrderAsync(order)).Throws(new ArgumentException());
            var controller = new OrderController(_orderService.Object);

            //Act
            var result = await controller.PostOrder(order);

            //Assert
            result.Result.Should().BeOfType(typeof(BadRequestResult));
            _orderService.Verify(s => s.SaveLastOrderToXml(order), Times.Never());
        }

        [Fact]
        public async Task GetOrder_ReturnOk()
        {
            //Arrange
            int orderId = 1;
            var order = new OrderDto() { Id=orderId};
            _orderService.Setup(s => s.GetOrderAsync(orderId)).Returns(Task.FromResult(order));
            var controller = new OrderController(_orderService.Object);

            //Act
            var result = await controller.GetOrder(orderId);

            //Assert
            result.Result.Should().BeOfType(typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var value = okResult?.Value as OrderDto;
            Assert.Equal(orderId, value?.Id);
        }

        [Fact]
        public async Task GetOrder_ReturnNotFound()
        {
            //Arrange
            int orderId = 1;
            var order = new OrderDto() {};
            _orderService.Setup(s => s.GetOrderAsync(orderId)).Throws(new EntityNotFoundException());
            var controller = new OrderController(_orderService.Object);

            //Act
            var result = await controller.GetOrder(orderId);

            //Assert
            result.Result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Fact]
        public void GetLastOrder_ReturnOk()
        {
            //Arrange
            var orderId = 1;
            var order = new OrderDto() { Id=orderId};
            _orderService.Setup(s => s.LoadLastOrderFromXml()).Returns(order);
            var controller = new OrderController(_orderService.Object);

            //Act
            var result = controller.GetLastOrder();

            //Assert
            result.Result.Should().BeOfType(typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var value = okResult?.Value as OrderDto;
            Assert.Equal(orderId, value?.Id);
        }

        [Fact]
        public void GetLastOrder_ReturnNotFound()
        {
            //Arrange
            OrderDto order = null!;
            _orderService.Setup(s => s.LoadLastOrderFromXml()).Returns(order);
            var controller = new OrderController(_orderService.Object);

            //Act
            var result = controller.GetLastOrder();

            //Assert
            result.Result.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}
