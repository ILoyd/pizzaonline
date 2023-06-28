using Moq;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Bll.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PizzaOnline.Api.Controllers;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace PizzaOnline.Tests.Controllers
{
    public class PizzaControllerTest
    {
        private Mock<IPizzaService> _pizzaService;

        public PizzaControllerTest()
        {
            _pizzaService = new Mock<IPizzaService>();
        }

        [Fact]
        public async Task GetPizzas_ReturnOk()
        {
            //Arrange
            var pizzas = new List<PizzaDto>();
            var pizza = new PizzaDto();
            pizzas.Add(pizza);
            _pizzaService.Setup(s => s.GetPizzasAsync()).Returns(Task.FromResult(pizzas.AsEnumerable()));
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.GetPizzas();

            //Assert
            result.Result.Should().BeOfType(typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var value = okResult?.Value as IEnumerable<PizzaDto>;
            Assert.Equal(pizzas, value);
        }

        [Fact]
        public async Task GetPizzas_ReturnNotFound()
        {
            //Arrange
            var pizzas = new List<PizzaDto>();
            _pizzaService.Setup(s => s.GetPizzasAsync()).Returns(Task.FromResult(pizzas.AsEnumerable()));
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.GetPizzas();

            //Assert
            result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            var value = notFoundResult?.Value as IEnumerable<PizzaDto>;
            Assert.Equal(pizzas, value);
        }

        [Fact]
        public async Task GetPizza_ReturnOk()
        {
            //Arrange
            int pizzaId = 1;
            var pizza = new PizzaDto() { Id = pizzaId };
            _pizzaService.Setup(s => s.GetPizzaAsync(pizzaId)).Returns(Task.FromResult(pizza));
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.GetPizza(pizzaId);

            //Assert
            result.Result.Should().BeOfType(typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            if (okResult != null) 
            {
                var value = okResult.Value as PizzaDto;
                Assert.Equal(pizzaId, value != null ? value.Id : 0);
            }
        }

        [Fact]
        public async Task GetPizza_ReturnNotFound()
        {
            //Arrange
            int pizzaId = 1;
            var pizza = new PizzaDto() { Id = pizzaId };
            _pizzaService.Setup(s => s.GetPizzaAsync(pizzaId)).Throws(new EntityNotFoundException());
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.GetPizza(pizzaId);

            //Assert
            result.Result.Should().BeOfType(typeof(NotFoundResult));
            _pizzaService.Verify(s => s.GetPizzaAsync(pizzaId),Times.Exactly(1));
        }

        [Fact]
        public async Task PostPizza_ReturnOk()
        {
            //Arrange
            var pizza = new PizzaDto();
            _pizzaService.Setup(s => s.InsertPizzaAsync(pizza)).Returns(Task.FromResult(pizza));
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.PostPizza(pizza);

            //Assert
            result.Result.Should().BeOfType(typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            var value = createdResult?.Value as PizzaDto;
            Assert.Equal(pizza, value);
        }

        [Fact]
        public async Task PostPizza_ReturnBadRequest()
        {
            //Arrange
            var pizza = new PizzaDto();
            _pizzaService.Setup(s => s.InsertPizzaAsync(pizza)).Throws(new DbUpdateException());
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.PostPizza(pizza);

            //Assert
            result.Result.Should().BeOfType(typeof(BadRequestResult));
            _pizzaService.Verify(s => s.InsertPizzaAsync(pizza), Times.Exactly(1));
        }

        [Fact]
        public async Task PutPizza_ReturnNoContent_WhenForceUpdateIsFalse()
        {
            //Arrange
            int pizzaId = 1;
            var pizza = new PizzaDto() { Id = pizzaId };
            var forceUpdate = false;
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.PutPizza(pizzaId,pizza, forceUpdate);

            //Assert
            result.Should().BeOfType(typeof(NoContentResult));
            _pizzaService.Verify(s => s.UpdatePizzaAsync(pizzaId, pizza, forceUpdate), Times.Exactly(1));
        }

        [Fact]
        public async Task PutPizza_ReturnNoContent_WhenForceUpdateIsTrue()
        {
            //Arrange
            int pizzaId = 1;
            var pizza = new PizzaDto() { Id = pizzaId };
            var forceUpdate = true;
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.PutPizza(pizzaId, pizza, forceUpdate);

            //Assert
            result.Should().BeOfType(typeof(NoContentResult));
            _pizzaService.Verify(s => s.UpdatePizzaAsync(pizzaId, pizza, forceUpdate), Times.Exactly(1));
        }

        [Fact]
        public async Task PutPizza_ReturnNotFound_WhenForceUpdateIsFalse()
        {
            //Arrange
            int pizzaId = 1;
            var pizza = new PizzaDto();
            var forceUpdate = false;
            _pizzaService.Setup(s => s.UpdatePizzaAsync(pizzaId, pizza, forceUpdate)).Throws(new EntityNotFoundException());
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.PutPizza(pizzaId, pizza, forceUpdate);

            //Assert
            result.Should().BeOfType(typeof(NotFoundResult));
            _pizzaService.Verify(s => s.UpdatePizzaAsync(pizzaId, pizza, forceUpdate), Times.Exactly(1));
        }

        [Fact]
        public async Task PutPizza_ReturnNotFound_WhenForceUpdateIsTrue()
        {
            //Arrange
            int pizzaId = 1;
            var pizza = new PizzaDto();
            var forceUpdate = true;
            _pizzaService.Setup(s => s.UpdatePizzaAsync(pizzaId, pizza, forceUpdate)).Throws(new EntityNotFoundException());
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.PutPizza(pizzaId, pizza, forceUpdate);

            //Assert
            result.Should().BeOfType(typeof(NotFoundResult));
            _pizzaService.Verify(s => s.UpdatePizzaAsync(pizzaId, pizza, forceUpdate), Times.Exactly(1));
        }

        [Fact]
        public async Task PutPizza_ReturnBadRequest()
        {
            //Arrange
            int pizzaId = 1;
            var pizza = new PizzaDto();
            var forceUpdate = false;
            _pizzaService.Setup(s => s.UpdatePizzaAsync(pizzaId, pizza, false)).Throws(new DbUpdateConcurrencyException());
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.PutPizza(pizzaId, pizza, forceUpdate);

            //Assert
            result.Should().BeOfType(typeof(BadRequestResult));
            _pizzaService.Verify(s => s.UpdatePizzaAsync(pizzaId, pizza, forceUpdate), Times.Exactly(1));
        }

        [Fact]
        public async Task DeletePizza_ReturnNotFound()
        {
            //Arrange
            int pizzaId = 1;
            long version = 1;
            var forceDelete = false;
            _pizzaService.Setup(s => s.SoftDeleteAsync(pizzaId, forceDelete, version)).Throws(new EntityNotFoundException());
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.DeletePizza(pizzaId, forceDelete, version);

            //Assert
            result.Should().BeOfType(typeof(NotFoundResult));
            _pizzaService.Verify(s => s.SoftDeleteAsync(pizzaId, forceDelete, version), Times.Exactly(1));
        }

        [Fact]
        public async Task DeletePizza_ReturnBadRequest()
        {
            //Arrange
            int pizzaId = 1;
            long version = 1;
            var forceDelete = false;
            _pizzaService.Setup(s => s.SoftDeleteAsync(pizzaId, forceDelete, version)).Throws(new DbUpdateConcurrencyException());
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.DeletePizza(pizzaId, forceDelete, version);

            //Assert
            result.Should().BeOfType(typeof(BadRequestResult));
            _pizzaService.Verify(s => s.SoftDeleteAsync(pizzaId, forceDelete, version), Times.Exactly(1));
        }

        [Fact]
        public async Task DeletePizza_ReturnNoContent_WhenForceUpdateIsFalse()
        {
            //Arrange
            int pizzaId = 1;
            long version = 1;
            var forceDelete = false;
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.DeletePizza(pizzaId, forceDelete, version);

            //Assert
            result.Should().BeOfType(typeof(NoContentResult));
            _pizzaService.Verify(s => s.SoftDeleteAsync(pizzaId, forceDelete, version), Times.Exactly(1));
        }

        [Fact]
        public async Task DeletePizza_ReturnNoContent_WhenForceUpdateIsTrue()
        {
            //Arrange
            int pizzaId = 1;
            long version = 1;
            var forceDelete = true;
            var controller = new PizzaController(_pizzaService.Object);

            //Act
            var result = await controller.DeletePizza(pizzaId, forceDelete, version);

            //Assert
            result.Should().BeOfType(typeof(NoContentResult));
            _pizzaService.Verify(s => s.SoftDeleteAsync(pizzaId, forceDelete, version), Times.Exactly(1));
        }
    }
}
