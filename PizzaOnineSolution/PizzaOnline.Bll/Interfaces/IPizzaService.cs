using PizzaOnline.Bll.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaOnline.Bll.Interfaces
{
    public interface IPizzaService
    {
        public Task<PizzaDto> GetPizzaAsync(int pizzaId);
        public Task<IEnumerable<PizzaDto>> GetPizzasAsync();
        public Task<PizzaDto> InsertPizzaAsync(PizzaDto newPizza);
        public Task UpdatePizzaAsync(int pizzaId, PizzaDto updatedPizza, bool forceUpdate);
        public Task SoftDeleteAsync(int pizzaId, bool forceDelete, long rowVerison);
    }
}
