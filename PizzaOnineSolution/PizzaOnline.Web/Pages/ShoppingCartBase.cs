using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Web.Services;
using System.Collections.ObjectModel;

namespace PizzaOnline.Web.Pages
{
    public class ShoppingCartBase : ComponentBase, IDisposable
    {
        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }        
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public int Id { get; set; }

        public bool orderPlaced = false;
        public bool lastorderAsk = false;

        public List<OrderItemDto> cartItems = new List<OrderItemDto>();
        public List<PizzaDto> pizzas = new List<PizzaDto>();
        

        protected override async Task OnInitializedAsync()
        {
            cartItems = (List<OrderItemDto>)await ShoppingCartService.GetItems();
            NavigationManager.LocationChanged += QuantityChanged;
            if (cartItems.Count > 0)
                pizzas = ShoppingCartService.GetPizzas();
        }

        public async Task DeleteItem(OrderItemDto item)
        {
            await ShoppingCartService.DeleteItem(item);
            cartItems = (List<OrderItemDto>)await ShoppingCartService.GetItems();
        }

        public async Task PlaceOrder()
        {
            orderPlaced = true;
            await ShoppingCartService.EmptyCart(cartItems, pizzas);
            cartItems.Clear();
            pizzas.Clear();
        }

        public PizzaDto GetPizza(int pizzaId) => pizzas.Find(p => p.Id == pizzaId);

        public async Task GetLastOrder()
        {
            lastorderAsk = true;
            await ShoppingCartService.LoadReceipt();
            cartItems = (List<OrderItemDto>) ShoppingCartService.ReloadItems();
            pizzas = ShoppingCartService.GetPizzas();
        }

        public void QuantityChanged(object sender, LocationChangedEventArgs e)
        {
            ShoppingCartService.UpdateItems(cartItems);
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= QuantityChanged;
        }
    }
}
