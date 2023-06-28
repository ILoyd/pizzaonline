using PizzaOnline.Bll.Dtos;

namespace PizzaOnline.Web.Services
{
    public interface IPizzaService
    {
        Task<IEnumerable<PizzaDto>> GetPizzas();
        Task<PizzaDto> GetPizza(int id);
        Task PostPizza(PizzaDto newPizza);
        Task DeletePizzaAsync(int id, bool forceDelete, PizzaDto deletedPizza);
        Task UpdatePizza(int id,PizzaDto updatedPizza, bool forceUpdate);
    }
}
