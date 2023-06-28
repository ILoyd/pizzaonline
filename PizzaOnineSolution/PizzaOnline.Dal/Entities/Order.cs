namespace PizzaOnline.Dal.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
        public ICollection<Pizza> Pizzas { get; } = new List<Pizza>();
    }
}
