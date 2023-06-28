using PizzaOnline.Bll.Dtos;
using System.Collections.ObjectModel;

namespace PizzaOnline.Web.Services
{
    public interface IShoppingCartService
    {
        Task<OrderDto> GetOrder(int orderId);
        Task<IEnumerable<OrderItemDto>> GetItems();
        Task<PizzaDto> AddItem(PizzaDto item);
        Task DeleteItem(OrderItemDto item);
        Task<OrderDto> EmptyCart(List<OrderItemDto> cartItems, List<PizzaDto> pizzas);
        Task LoadPizzas();
        List<PizzaDto> GetPizzas();
        Task LoadReceipt();
        IEnumerable<OrderItemDto> ReloadItems();
        Task UpdateItems(IEnumerable<OrderItemDto> items);
    }
}
 