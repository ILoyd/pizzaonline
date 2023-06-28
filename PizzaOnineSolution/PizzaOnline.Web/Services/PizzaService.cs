using Blazored.LocalStorage;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Dal.Entities;
using System.Net.Http.Json;
using System.Text;

namespace PizzaOnline.Web.Services
{
    public class PizzaService : IPizzaService
    {

        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public PizzaService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task DeletePizzaAsync(int id, bool forceDelete, PizzaDto pizza)
        {
            try
            {
                long lastVersion = BitConverter.ToInt64(pizza.RowVersion, 0);

                var response = await _httpClient.DeleteAsync($"api/Pizza/{id}/{forceDelete}/{lastVersion}");

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new DbUpdateConcurrencyException();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new EntityNotFoundException();
                else
                {
                    var order = await _localStorage.GetItemAsync<OrderDto>("order");

                    if (order != null)
                    {
                        var searchedItem = order.OrderItems
                        .FirstOrDefault(oi => oi.PizzaId == id);

                        if(searchedItem != null)
                            order.OrderItems.Remove(searchedItem);

                        var searchedPizza = order.Pizzas
                            .FirstOrDefault(p => p.Id == id);

                        if (searchedPizza != null)
                            order.Pizzas.Remove(searchedPizza);

                        if (order.OrderItems.Count == 0)
                            await _localStorage.RemoveItemAsync("order");
                        else
                            await _localStorage.SetItemAsync("order", order);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PizzaDto> GetPizza(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Pizza/{id}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(PizzaDto);
                    }

                    return await response.Content.ReadFromJsonAsync<PizzaDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PizzaDto>> GetPizzas()
        {
            try
            {
                var pizzas = await _httpClient.GetFromJsonAsync<IEnumerable<PizzaDto>>("api/Pizza");
                return pizzas ?? new List<PizzaDto>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task PostPizza(PizzaDto newPizza)
        {
            try
            {
                newPizza.RowVersion = BitConverter.GetBytes(DateTime.Now.Ticks);
                var pizza = await _httpClient.PostAsJsonAsync("api/Pizza", newPizza);

                var str = pizza.Content.ReadAsStringAsync();
                Console.WriteLine(str);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdatePizza(int id, PizzaDto updatedPizza, bool forceUpdate)
        {
            try
            {
                string json = JsonConvert.SerializeObject(updatedPizza);
                StringContent sc = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/Pizza/{id}/{forceUpdate}", sc);
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new DbUpdateConcurrencyException();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new EntityNotFoundException();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
