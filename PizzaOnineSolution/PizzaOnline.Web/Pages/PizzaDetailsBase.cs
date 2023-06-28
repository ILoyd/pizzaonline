using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Web.Services;

namespace PizzaOnline.Web.Pages
{
    public class PizzaDetailsBase:ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IPizzaService PizzaService { get; set;}
        [Inject]
        public IShoppingCartService ShoppingCartService { get; set;}
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public ConfirmBox confirmBox = new ConfirmBox();

        public PizzaDto Pizza { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Pizza = await PizzaService.GetPizza(Id);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async void Detele_OnClick()
        {
            try
            {
                await PizzaService.DeletePizzaAsync(Id, false, Pizza);
                NavigationManager.NavigateTo($"/");
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateConcurrencyException)
                    confirmBox.Show("Someone else modifidy this Pizza. Do you still want to delete it?");
                else
                {
                    NavigationManager.NavigateTo($"/");
                }
            }
        }

        public async void AddToCart_OnClick()
        {
            var newPizza = await ShoppingCartService.AddItem(Pizza);
            Console.WriteLine(newPizza);
            NavigationManager.NavigateTo($"/ShoppingCart");
        }

        public async void Edit_OnClick()
        {
            NavigationManager.NavigateTo($"/EditPizza/{Id}");
        }

        public async Task Confirm_Delete(bool value)
        {
            if (value)
                await PizzaService.DeletePizzaAsync(Id, value, Pizza);
            NavigationManager.NavigateTo($"/");
        }
    }
}
