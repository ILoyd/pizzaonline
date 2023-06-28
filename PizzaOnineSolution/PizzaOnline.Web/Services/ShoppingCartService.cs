using Blazored.LocalStorage;
using PizzaOnline.Bll.Dtos;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Xml.Serialization;

namespace PizzaOnline.Web.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        public OrderDto OrderDto { get; set; }

        public ShoppingCartService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<PizzaDto> AddItem(PizzaDto item)
        {
            OrderDto = await _localStorage.GetItemAsync<OrderDto>("order");

            if (OrderDto == null)
            {
                OrderDto = new OrderDto()
                {
                    OrderItems = new List<OrderItemDto>(),
                    Pizzas = new List<PizzaDto>()
                };
            }

            var sameItem = OrderDto.OrderItems
                .FirstOrDefault(oi => oi.PizzaId == item.Id);

            if (sameItem == null)
            {
                var orderItem = new OrderItemDto()
                {
                    PizzaId = item.Id,
                    Quantity = 1
                };
                OrderDto.OrderItems.Add(orderItem);
                OrderDto.Pizzas.Add(item);
            }
            else
            {
                sameItem.Quantity++;
            }

            await _localStorage.SetItemAsync("order", OrderDto);

            return item;
        }

        public async Task DeleteItem(OrderItemDto item)
        {

            if (OrderDto == null)
            {
                return;
            }

            var searchedItem = OrderDto.OrderItems
                .FirstOrDefault(oi => oi.PizzaId == item.PizzaId);
            OrderDto.OrderItems.Remove(searchedItem);

            var searchedPizza = OrderDto.Pizzas
                .FirstOrDefault(p => p.Id == item.PizzaId);
            OrderDto.Pizzas.Remove(searchedPizza);

            if (OrderDto.OrderItems.Count == 0)
                await _localStorage.RemoveItemAsync("order");
            else
                await _localStorage.SetItemAsync("order", OrderDto);
        }

        public async Task<IEnumerable<OrderItemDto>> GetItems()
        {
            OrderDto = await _localStorage.GetItemAsync<OrderDto>("order");

            if(OrderDto == null)
            {
                return new List<OrderItemDto>();
            }

            return OrderDto.OrderItems;
        }

        public async Task<OrderDto> GetOrder(int orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Order/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(OrderDto);
                    }

                    OrderDto = await response.Content.ReadFromJsonAsync<OrderDto>();

                    return OrderDto;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<OrderDto> EmptyCart(List<OrderItemDto> cartItems, List<PizzaDto> pizzas)
        {
            await _localStorage.RemoveItemAsync("order");
            try
            {
                OrderDto.OrderDate = DateTime.Now;
                var response = await _httpClient.PostAsJsonAsync($"api/Order", OrderDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(OrderDto);
                    }

                    var _order = await response.Content.ReadFromJsonAsync<OrderDto>();

                    return _order;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task LoadPizzas()
        {
            OrderDto.Pizzas.Clear();
            foreach (var orderItem in OrderDto.OrderItems)
            {
                var response = await _httpClient.GetAsync($"api/Pizza/{orderItem.PizzaId}");

                var pizza =await response.Content.ReadFromJsonAsync<PizzaDto>();

                OrderDto.Pizzas.Add(pizza);
            }
        }

        public List<PizzaDto> GetPizzas()
        {
            return OrderDto.Pizzas;
        }

        public async Task LoadReceipt()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Order");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        OrderDto = default(OrderDto);
                    }

                    var _order = await response.Content.ReadFromJsonAsync<OrderDto>();

                    OrderDto = _order;
                    await LoadPizzas();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<OrderItemDto> ReloadItems()
        {
            return OrderDto.OrderItems;
        }

        public async Task UpdateItems(IEnumerable<OrderItemDto> items)
        {
            OrderDto = await _localStorage.GetItemAsync<OrderDto>("order");
            OrderDto.OrderItems = (List<OrderItemDto>) items;
            await _localStorage.SetItemAsync("order",OrderDto);

        }
    }
}
