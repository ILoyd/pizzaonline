using PizzaOnline.Dal.Entities;
using System.ComponentModel.DataAnnotations;

namespace PizzaOnline.Web.Pages
{
    public class NewPizza
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public Size Size { get; set; } = Size.Medium;
        public StuffedCrust StuffedCrust { get; set; } = StuffedCrust.Normal;
    }
}
