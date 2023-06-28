using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PizzaOnline.Dal.Entities;

namespace PizzaOnline.Bll.Dtos
{
    public record OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = null!;
        public List<PizzaDto> Pizzas { get; set; } = null!;

    }

    public record PizzaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the name.")]
        public string Name { get; set; } = null!;

        [Range(1, Double.PositiveInfinity,ErrorMessage = "Please enter the price.")]
        public int UnitPrice { get; set; }

        [Required(ErrorMessage = "Please enter the description.")]
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public Size Size { get; set; } = Size.Medium;
        public StuffedCrust StuffedCrust { get; set; } = StuffedCrust.Normal;
        public bool IsDeleted { get; set; }=false;
        public byte[] RowVersion { get; set; }=null!;

    }

    public record OrderItemDto
    {
        public int Id { get; init; } = 0;
        public int PizzaId { get; set; }
        public int Quantity { get; set; }
    }
}
