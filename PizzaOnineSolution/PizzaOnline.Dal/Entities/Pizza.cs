using System.ComponentModel.DataAnnotations;

namespace PizzaOnline.Dal.Entities
{
    public class Pizza
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UnitPrice { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<OrderItem> PizzaOrders { get; } = new List<OrderItem>();
        public ICollection<Order> Orders { get; } = new List<Order>();
        public Size Size { get; set; }
        public StuffedCrust StuffedCrust { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp()]
        public byte[] RowVersion { get; set; }

        public Pizza(string name)
        {
            Name = name;
        }
    }
}
