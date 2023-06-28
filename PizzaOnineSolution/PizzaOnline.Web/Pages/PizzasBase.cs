using Microsoft.AspNetCore.Components;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Web.Services;

namespace PizzaOnline.Web.Pages
{
    public class PizzasBase : ComponentBase
    {
        [Inject]
        public IPizzaService PizzaService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        public List<PizzaDto> Pizzas { get; set; }

        public string ErrorMessage { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            var enumerable = await PizzaService.GetPizzas();
            Pizzas = enumerable.ToList();
            if (Pizzas == null)
                ErrorMessage = "There are no pizzas in the database.";
        }
    }
}
