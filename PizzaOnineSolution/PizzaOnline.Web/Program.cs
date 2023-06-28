using Blazored.LocalStorage;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PizzaOnline.Web;
using PizzaOnline.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7058/") });

builder.Services.AddTransient<IPizzaService, PizzaService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredModal();

await builder.Build().RunAsync();
