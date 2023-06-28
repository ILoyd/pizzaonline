using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;


namespace PizzaOnline.Bll.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Dal.Entities.Pizza, PizzaDto>()
                //.ForMember(
                //    p => p.Orders,
                //    opt => opt.MapFrom(p => p.PizzaOrders.Select(po => po.Order)))
                .ReverseMap();
            CreateMap<Dal.Entities.Order, OrderDto>()
                .ForMember(o => o.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(o => o.Pizzas, opt => opt.MapFrom(src => src.Pizzas))
                .ReverseMap();
            CreateMap<Dal.Entities.OrderItem, OrderItemDto>()
                .ReverseMap();
        }
    }
}
