using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Web.Services;

namespace PizzaOnline.Web.Pages
{
    public class AddPizzaBase : ComponentBase
    {
        public PizzaDto newPizza = new PizzaDto();

        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IPizzaService PizzaService { get; set; }
        public async Task HandleAddNewPizza() 
        {
            if(String.IsNullOrEmpty(newPizza.ImageUrl))
                newPizza.ImageUrl = "/Images/pizza_1.jpg";
            await PizzaService.PostPizza(newPizza);
            NavigationManager.NavigateTo($"/");
        }

        public async Task OnFileChange(InputFileChangeEventArgs e)
        {
            var format = "image/png";
            var resizedImage = await e.File.RequestImageFileAsync(format, 1000, 666);
            var buffer = new byte[resizedImage.Size];
            await resizedImage.OpenReadStream().ReadAsync(buffer);
            var imageData = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            newPizza.ImageUrl = imageData;
        } 
    }
}
