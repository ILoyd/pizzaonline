using AutoMapper;
using AutoMapper.QueryableExtensions;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Dal;
using PizzaOnline.Bll.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace PizzaOnline.Bll
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderDto Order { get; set; }

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<OrderDto> GetOrderAsync(int orderId)
        {
            return await _context.Orders
               .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
               .SingleOrDefaultAsync(o => o.Id == orderId)
               ?? throw new EntityNotFoundException("Pizza Not Found");
        }

        public async Task<OrderDto> InsertOrderAsync(OrderDto newOrder)
        {
            var entity = _mapper.Map<Dal.Entities.Order>(newOrder);
            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();
            return await GetOrderAsync(entity.Id);
        }

        public void SaveLastOrderToXml(OrderDto newOrder)
        {
            using (var stream = new FileStream("lastOrder.xml", FileMode.Create))
            {
                XmlSerializer xml = new XmlSerializer(typeof(OrderDto));
                xml.Serialize(stream, newOrder);
            }
        }

        public OrderDto LoadLastOrderFromXml()
        {
            using (var stream = new FileStream("lastOrder.xml", FileMode.Open))
            {
                XmlSerializer xml = new XmlSerializer(typeof(OrderDto));
                return (OrderDto)xml.Deserialize(stream);
            }
        }
    }
}
