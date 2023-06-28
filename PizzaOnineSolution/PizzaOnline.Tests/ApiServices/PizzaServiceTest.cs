using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PizzaOnline.Bll;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaOnline.Tests.ApiServices
{
    public class PizzaServiceTest : IDisposable
    {
        private static DbContextOptions<AppDbContext> dbContextOptions= new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=../../../testdatabase.db")
            .Options;

        private AppDbContext context;
        private readonly IMapper mapper;

        public void Dispose()
        {
            context.Database.EnsureDeleted();
        }

        public PizzaServiceTest()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();
            var profile = new MappingProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            mapper = new Mapper(config);     
        }

        [Fact]
        public async Task GetPizzaAsync_ReturnsPizza()
        {
            //Arrange
            int pizzaid = 1;
            var pizzaService = new PizzaService(context, mapper);

            //Act
            var result = await pizzaService.GetPizzaAsync(pizzaid);

            //Assert
            Assert.Equal(result.Id, pizzaid);
            
        }

        [Fact]
        public async Task GetPizzaAsync_ThrowsEntityNotFound()
        {
            //Arrange
            int pizzaid = 8;
            var pizzaService = new PizzaService(context, mapper);

            //Act,Assert
            await Assert.ThrowsAsync<EntityNotFoundException>( async () =>
            {
                await pizzaService.GetPizzaAsync(pizzaid);
            });
        }

        [Fact]
        public async Task GetPizzasAsync_ReturnsListOfPizzas()
        {
            //Arrange
            var pizzaService = new PizzaService(context, mapper);

            //Act
            var result = await pizzaService.GetPizzasAsync();

            //Assert
            result.Should().BeOfType(typeof(List<PizzaDto>));
            Assert.Equal(7, result.Count());
        }

        [Fact]
        public async Task InsertPizzaAsync_ReturnsInsertedPizza()
        {
            //Arrange
            var newPizza = new PizzaDto() { Name = "lorem ipsum", Description = "lorem ipsum", ImageUrl = "lorem ipsum", IsDeleted = false, Size = Dal.Entities.Size.Small, StuffedCrust = Dal.Entities.StuffedCrust.Normal, UnitPrice = 1, RowVersion = BitConverter.GetBytes(2) };
            var pizzaService = new PizzaService(context, mapper);

            //Act
            var result = await pizzaService.InsertPizzaAsync(newPizza);

            //Assert
            result.Should().BeOfType(typeof(PizzaDto));
            result.Id.Should().Be(8);
            Assert.True(context.Pizzas.Any(p => p.Id == 8));
        }

        [Fact]
        public async Task InsertPizzaAsync_ThrowsDbUpdateException()
        {
            //Arrange
            var newPizza = new PizzaDto();
            var pizzaService = new PizzaService(context, mapper);

            //Act, Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await pizzaService.InsertPizzaAsync(newPizza);
            });
        }
    }
}
