using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Web.Services;

namespace PizzaOnline.Web.Pages
{
    public class EditPizzaBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        public PizzaDto editedPizza = new PizzaDto();

        public ConfirmBox confirmBox = new ConfirmBox();

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IPizzaService PizzaService { get; set; }

        public string ErrorMessage { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                editedPizza = await PizzaService.GetPizza(Id);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task OnFileChange(InputFileChangeEventArgs e)
        {
            var format = "image/png";
            var resizedImage = await e.File.RequestImageFileAsync(format, 1000, 666);
            var buffer = new byte[resizedImage.Size];
            await resizedImage.OpenReadStream().ReadAsync(buffer);
            var imageData = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            editedPizza.ImageUrl = imageData;
        }

        public async Task HandleEditPizza()
        {
            try
            {
                await PizzaService.UpdatePizza(Id, editedPizza, false);
                NavigationManager.NavigateTo($"/PizzaDetails/{Id}");
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateConcurrencyException)
                    confirmBox.Show("Someone else modifidy this Pizza. Do you want to override the values?");
                else
                {
                    ErrorMessage = "This pizza was already deleted by someone else.";
                }
            }
        }

        public async Task Confirm_Click(bool value)
        {
            if(value)
                await PizzaService.UpdatePizza(Id, editedPizza, value);
            NavigationManager.NavigateTo($"/PizzaDetails/{Id}");
        }
    }
}
