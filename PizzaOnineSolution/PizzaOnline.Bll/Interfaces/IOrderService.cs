using PizzaOnline.Bll.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaOnline.Bll.Interfaces
{
    public interface IOrderService
    {
        public Task<OrderDto> GetOrderAsync(int orderId);
        public Task<OrderDto> InsertOrderAsync(OrderDto newOrder);
        public void SaveLastOrderToXml(OrderDto newOrder);
        public OrderDto LoadLastOrderFromXml();
    }
}
